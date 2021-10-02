#version 450 core

//out vec4 FragColor;

layout (location = 0) out vec4 fragColor;

uniform vec3 colorTint;

void main()
{    
    fragColor = vec4(colorTint,1);
}