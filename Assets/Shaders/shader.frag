#version 330 core

uniform vec2 u_resolution; // Screen resolution

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

vec3 castRay(vec3 ro, vec3 rd)
{
    vec2 t = sphIntersect(ro, rd, 1.0); 
    if (t.x < 0.0) return vec3(0.0);
    vec3 tPos = ro + rd * t.x;
    vec3 n = tPos;
    vec3 lightDir = normalize(vec3(-0.5, 0.75, -1.0));
    float diffuse = dot(-lightDir, n);
    return vec3(diffuse);
}

void main()
{
    vec2 uv = ((texCoord.xy)) * vec2(u_resolution / u_resolution.y);

    vec3 ro = vec3(-5.0, 0.0, 0.0);
    vec3 rd = normalize(vec3(1.0, uv));
    vec3 col = castRay(ro, rd);

    outputColor = vec4(col, 1.0);
}
