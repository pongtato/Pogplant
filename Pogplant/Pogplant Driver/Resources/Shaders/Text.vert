#version 450 core

layout (location = 0) in vec4 vertex;
out vec2 TexCoords;

uniform mat4 m4_Projection;
uniform mat4 m4_View;
uniform mat4 m4_Model;

void main()
{
    gl_Position = m4_Projection * m4_View * m4_Model * vec4(vertex.xy, 0.0, 1.0);
    TexCoords = vertex.zw;
}