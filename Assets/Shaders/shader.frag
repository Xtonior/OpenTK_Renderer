#version 330 core

uniform vec2 u_resolution; // Screen resolution
uniform float u_time;

out vec4 outputColor;

in vec2 texCoord;

vec2 sphIntersect( in vec3 ro, in vec3 rd, float ra )
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
float plaIntersect( in vec3 ro, in vec3 rd, in vec4 p )
{
    return -(dot(ro, p.xyz) + p.w) / dot(rd, p.xyz);
}

vec3 calcPhong(vec3 rd, vec3 lightDir, vec3 normal, vec3 sun, vec3 col, vec3 ambient)
{
    float diff = max(0.0, dot(lightDir, normal));
    vec3 diffuse = diff * sun;

    vec3 reflectDir = reflect(-lightDir, normal);  
    float spec = pow(max(0.0, dot(rd, reflectDir)), 32);
    vec3 specular = 0.5 * spec * sun;  

    return vec3(ambient + diffuse) * col;
}

vec3 castRay(vec3 cpherePos, vec3 boxPos, vec4 plane, vec3 ro, vec3 rd)
{
    vec3 lightDir = normalize(vec3(cos(u_time), 0.75, sin(u_time)));

    vec3 sky = vec3(0.33, 0.43, 0.7);
    vec3 colS = vec3(0.93, 0.1, 0.21);
    vec3 colB = vec3(0.33, 0.93, 0.7);
    vec3 colP = vec3(0.9, 0.9, 0.2);
    vec3 sun = vec3(0.8, 0.8, 0.96);

    vec3 ambient = sky;

    vec2 ct = sphIntersect(cpherePos + ro, rd, 1.0); 

    vec3 boxN;
    vec2 bt = boxIntersection(boxPos + ro, rd, vec3(1.0), boxN);

    float pt = plaIntersect(ro, rd, plane);

    if (ct.x > 0.0 || ct.y > 0.0) 
    {
        if (ct.x > 0.0)
        {
            vec3 tPos = ro + rd * ct.x;
            vec3 n = tPos;
            return calcPhong(rd, lightDir, n, sun, colS, ambient);
        }
    
        return ambient * colS;
    }
    else if (bt.x > 0.0 || bt.y > 0.0)
    {
        if (bt.x > 0.0)
        {
            return calcPhong(rd, lightDir, boxN, sun, colB, ambient);
        }
        
        return ambient * colB;
    }
    else if (pt > 0.0)
    {
        return calcPhong(rd, lightDir, vec3(0.0, 0.0, 1.0), sun, colP, ambient);
    }

    return vec3(sky + (pow(max(0.0, dot(rd, lightDir)), 64.0) * vec3(1.0)));
}

void main()
{
    vec2 uv = ((texCoord.xy)) * vec2(u_resolution / u_resolution.y);

    vec3 c = vec3(0.0, 1.0, 0.0);
    vec3 b = vec3(0.0, -1.0, 0.0);
    vec4 p = vec4(0.0, 0.0, 1.0, 5.0);

    vec3 ro = vec3(-5.0, 2.0, 2.0);
    vec3 rd = normalize(vec3(1.0, uv));
    vec3 col = castRay(c, b, p, ro, rd);

    outputColor = vec4(col, 1.0);
}
