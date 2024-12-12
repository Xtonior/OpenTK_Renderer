#version 330 core

uniform vec2 u_resolution; // Screen resolution
uniform float u_time;
uniform vec2 u_seed1;
uniform vec2 u_seed2;
uniform vec3 u_pos;
uniform mat3 u_rot;
uniform int u_maxrefs = 8;
uniform int u_maxsamples = 4;
uniform samplerCube u_cubemap;

out vec4 outputColor;

in vec2 texCoord;

const float MAX_DIST = 99999.0;

vec3 sun = vec3(1.0, 1.0, 1.0);

uvec4 R_STATE;

uint TausStep(uint z, int S1, int S2, int S3, uint M)
{
	uint b = (((z << S1) ^ z) >> S2);
	return (((z & M) << S3) ^ b);	
}

uint LCGStep(uint z, uint A, uint C)
{
	return (A * z + C);	
}

vec2 hash22(vec2 p)
{
	p += u_seed1.x;
	vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
	p3 += dot(p3, p3.yzx+33.33);
	return fract((p3.xx+p3.yz)*p3.zy);
}

float random()
{
	R_STATE.x = TausStep(R_STATE.x, 13, 19, 12, uint(4294967294));
	R_STATE.y = TausStep(R_STATE.y, 2, 25, 4, uint(4294967288));
	R_STATE.z = TausStep(R_STATE.z, 3, 11, 17, uint(4294967280));
	R_STATE.w = LCGStep(R_STATE.w, uint(1664525), uint(1013904223));
	return 2.3283064365387e-10 * float((R_STATE.x ^ R_STATE.y ^ R_STATE.z ^ R_STATE.w));
}

mat2 rot(float a) {
	float s = sin(a);
	float c = cos(a);
	return mat2(c, -s, s, c);
}

vec3 randomOnSphere() {
	vec3 rand = vec3(random(), random(), random());
	float theta = rand.x * 2.0 * 3.14159265;
	float v = rand.y;
	float phi = acos(2.0 * v - 1.0);
	float r = pow(rand.z, 1.0 / 3.0);
	float x = r * sin(phi) * cos(theta);
	float y = r * sin(phi) * sin(theta);
	float z = r * cos(phi);
	return vec3(x, y, z);
}

vec2 sphIntersect(in vec3 ro, in vec3 rd, float ra)
{
    float b = dot(ro, rd);
    float c = dot(ro, ro) - ra*ra;
    float h = b * b - c;
    if( h<0.0 ) return vec2(-1.0); // no intersection
    h = sqrt( h );
    return vec2( -b-h, -b+h );
}

vec2 boxIntersection(in vec3 ro, in vec3 rd, in vec3 boxSize, out vec3 outNormal)  
{
	vec3 m = 1.0 / rd;
	vec3 n = m * ro;
	vec3 k = abs(m) * boxSize;
	vec3 t1 = -n - k;
	vec3 t2 = -n + k;
	float tN = max(max(t1.x, t1.y), t1.z);
	float tF = min(min(t2.x, t2.y), t2.z);
	if(tN > tF || tF < 0.0) return vec2(-1.0);
	outNormal = -sign(rd) * step(t1.yzx, t1.xyz) * step(t1.zxy, t1.xyz);
	return vec2(tN, tF);
}

// plane degined by p (p.xyz must be normalized)
float plaIntersect(in vec3 ro, in vec3 rd, in vec4 p)
{
    return -(dot(ro, p.xyz) + p.w) / dot(rd, p.xyz);
}

vec3 getSky(vec3 rd, vec3 lightDir)
{
	vec3 correctedRd = vec3(rd.x, -rd.z, rd.y);
	vec3 sky = texture(u_cubemap, correctedRd).rgb;
	// vec3 skybox = vec3(sky + (pow(max(0.0, dot(rd, lightDir)), 16.0) * vec3(2.0)));
    return sky; //mix(sky, sun, 0.1); 
}

vec4 castRay(inout vec3 ro, inout vec3 rd, vec3 lightDir) 
{
	vec4 col;
	vec2 minIt = vec2(MAX_DIST);
	vec2 it;
	vec3 n;

	// Triangle Intersection
	/*vec3 triangleV1 = vec3(5.0, -8.0, 1.0);
	vec3 triangleV2 = vec3(5.0, -8.0, 2.0);
	vec3 triangleV3 = vec3(5.0, -6.0, 1.0);
	vec4 triCol = vec4(1.0, 0.5, 0.3, 0.8);

	if(triIntersect(ro, rd, triangleV1, triangleV2, triangleV3))
	{
		minIt = triangleV1;
		col = triCol;
	}*/

    // Sphere Intersection
    vec4 sphereGlass = vec4(1.0, -2.0, 1.5, 1.0);

    it = sphIntersect(ro - sphereGlass.xyz, rd, sphereGlass.w);

	if(it.x > 0.0 && it.x < minIt.x) 
    {
		minIt = it;
		vec3 itPos = ro + rd * it.x;
		n = normalize(itPos - sphereGlass.xyz);
		col = vec4(0.9, 0.9, 0.9, -1.5);
	}

	vec4 sphereMirror = vec4(-2.0, 1.0, 1.0, 1.0);

	it = sphIntersect(ro - sphereMirror.xyz, rd, sphereMirror.w);

	if(it.x > 0.0 && it.x < minIt.x) 
    {
		minIt = it;
		vec3 itPos = ro + rd * it.x;
		n = normalize(itPos - sphereMirror.xyz);
		col = vec4(1.0, 1.0, 1.0, 1.0);
	}

	vec4 sphereLight = vec4(3.0, -4.0, 1.0, 0.5);

	it = sphIntersect(ro - sphereLight.xyz, rd, sphereLight.w);

	if(it.x > 0.0 && it.x < minIt.x) 
    {
		minIt = it;
		vec3 itPos = ro + rd * it.x;
		n = normalize(itPos - sphereLight.xyz);
		col = vec4(10.0, 10.0, 8.0, -2.0);
	}

    // Box Intersection
    vec3 boxN;
	vec3 boxPos = vec3(3.0, 6.0, 1.0);
	it = boxIntersection(ro - boxPos, rd, vec3(1.0), boxN);

	if(it.x > 0.0 && it.x < minIt.x) 
    {
		minIt = it;
		n = boxN;
		col = vec4(0.9, 0.9, 0.9, 0.1);
	}

	vec3 extraBoxN;
	vec3 extraBoxPos = vec3(12.0, 1.0, 5.0);
	it = boxIntersection(ro - extraBoxPos, rd, vec3(5.0), extraBoxN);

	if(it.x > 0.0 && it.x < minIt.x) 
    {
		minIt = it;
		n = extraBoxN;
		col = vec4(1.0, 1.0, 1.0, 0.79);
	}

    // Plane Intersection
	vec3 planeNormal = vec3(0.0, 0.0, 1.0);
	it = vec2(plaIntersect(ro, rd, vec4(planeNormal, 0.0)));

	if(it.x > 0.0 && it.x < minIt.x) 
    {
		minIt = it;
		n = planeNormal;
		col = vec4(0.6, 0.4, 0.3, 0.0);
	}

    // Ray trace
	if(minIt.x == MAX_DIST) return vec4(getSky(rd, lightDir), -2.0);
	if(col.a == -2.0) return col;

	vec3 reflected = reflect(rd, n);

	if(col.a < 0.0) 
    {
		float fresnel = 1.0 - abs(dot(-rd, n));

		if(random() - 0.1 < fresnel * fresnel) 
        {
			rd = reflected;
			return col;
		}

		ro += rd * (minIt.y + 0.001);
		rd = refract(rd, n, 1.0 / (1.0 - col.a));

		return col;
	}

    // Color
    vec3 itPos = ro + rd * it.x;
	vec3 r = randomOnSphere();
	vec3 diffuse = normalize(r * dot(r, n));
	ro += rd * (minIt.x - 0.001);
	rd = mix(diffuse, reflected, col.a);
	return col;
}

vec3 traceRay(vec3 ro, vec3 rd, vec3 lightDir)
{
    vec3 col = vec3(1.0);
	for(int i = 0; i < u_maxrefs; i++)
	{
		vec4 refCol = castRay(ro, rd, lightDir);
		col *= refCol.rgb;
		if(refCol.a == -2.0) return col;
	}
	return vec3(0.0);
}

void main()
{
    vec2 uv = ((texCoord.xy)) * vec2(u_resolution / u_resolution.y);
	vec2 uvRes = hash22(uv + 1.0) * u_resolution + u_resolution;

	R_STATE.x = uint(u_seed1.x + uvRes.x);
	R_STATE.y = uint(u_seed1.y + uvRes.x);
	R_STATE.z = uint(u_seed2.x + uvRes.y);
	R_STATE.w = uint(u_seed2.y + uvRes.y);

    vec3 rayOrigin = u_pos;
	vec3 rayDirection = normalize(vec3(1.0, uv));
	rayDirection *= u_rot;

    vec3 lightDir = normalize(vec3(cos(u_time), 0.75, sin(u_time)));

	vec3 col = vec3(0.0);

	for(int i = 0; i < u_maxsamples; i++) 
	{
		col += traceRay(rayOrigin, rayDirection, lightDir);
	}

	col /= u_maxsamples;

    outputColor = vec4(col, 1.0);
}
