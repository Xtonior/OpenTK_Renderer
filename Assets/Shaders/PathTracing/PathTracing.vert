#version 330 core

layout(location = 0) in vec3 aPosition;  

out vec2 texCoord;

void main(void)
{
    texCoord = vec2(aPosition);

    gl_Position = vec4(aPosition, 1.0);
}