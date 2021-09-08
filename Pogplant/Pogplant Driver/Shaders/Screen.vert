#version 450 core

layout (location = 0) in vec2 v2_Pos;
layout (location = 1) in vec2 v2_TexCoords;

out vec2 TexCoords;

void main() 
{
    TexCoords = v2_TexCoords;
    gl_Position = vec4(v2_Pos, 0.0f, 1.0f);
}
