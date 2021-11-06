#version 450 core

layout (location = 0) in vec4 vertex;
out vec2 TexCoords;

uniform mat4 m4_Projection;
uniform mat4 m4_View;
uniform mat4 m4_Model;
uniform float f_Aspect;

void main()
{
    gl_Position = m4_Projection * m4_View * m4_Model * vec4(vertex.x, vertex.y * f_Aspect, 0.0f, 1.0f);
    TexCoords = vertex.zw;
}