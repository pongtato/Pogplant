#version 450 core
layout (location = 0) in vec3 v3_Pos;

out vec3 TexCoords;

uniform mat4 m4_View;
uniform mat4 m4_Projection;

void main() 
{
    TexCoords = v3_Pos;
    vec4 pos = m4_Projection * m4_View * vec4(v3_Pos, 1.0);
    gl_Position = pos.xyww;
}
