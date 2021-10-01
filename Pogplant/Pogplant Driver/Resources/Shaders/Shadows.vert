#version 450 core

layout (location = 0) in vec3 v3_Pos;

uniform mat4 m4_LightProjection;
uniform mat4 m4_Model;

void main()
{
    gl_Position = m4_LightProjection * m4_Model * vec4(v3_Pos, 1.0);
}