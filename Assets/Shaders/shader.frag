#version 330 core

uniform vec2 u_resolution; // Screen resolution
uniform vec4 u_pos;        // Sphere center and radius
// u_pos.x, u_pos.y, u_pos.z are center.x, center.y, radius respectively

out vec4 outputColor;

// sphere of size ra centered at point ce
vec2 sphIntersect( in vec3 ro, in vec3 rd, in vec3 ce, float ra )
{
    vec3 oc = ro - ce;
    float b = dot( oc, rd );
    float c = dot( oc, oc ) - ra*ra;
    float h = b*b - c;
    if( h<0.0 ) return vec2(-1.0); // no intersection
    h = sqrt( h );
    return vec2( -b-h, -b+h );
}

void main()
{
    vec3 ro = u_pos;

    vec2 uv = (gl_TexCoord[0].xy - 0.5) * u_resolution / u_resolution.y;
	vec2 uvRes = hash22(uv + 1.0) * u_resolution + u_resolution;

    vec3 rayDirection = normalize(vec3(1.0, uv));

    vec3 center = vec3(0.0f, 0.0f, 0.0f); // Sphere center
    float radius = u_pos.w;                   // Sphere radius

    vec2 t = sphIntersect( ro, rayDirection, center, radius ); 

    if (t.y < 0.0) 
    {
        outputColor = vec4(0.0, 0.0, 0.0, 1.0); // No intersection
    } 
    else 
    {
        outputColor = vec4(t.x / 10.0, t.y / 10.0, 1.0, 1.0); // Debug gradient
    }
}
