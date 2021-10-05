#version 450 core

layout (location = 3) out vec4 fragColor;

uniform vec3 colorTint;

void main()
{    
    fragColor = vec4(colorTint,1);
}