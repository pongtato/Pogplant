#version 450 core

layout (location = 0) in vec3 v3_Pos;
layout (location = 1) in vec3 v3_Normal;
layout (location = 4) in vec2 v2_TexCoords;

uniform mat4 m4_Model;
uniform mat4 m4_View;
uniform mat4 m4_Projection;

void main()
{
    gl_Position = m4_Projection * m4_View * m4_Model * vec4(v3_Pos, 1.0);
}