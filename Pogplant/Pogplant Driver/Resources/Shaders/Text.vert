#version 450 core

layout (location = 0) in vec4 vertex;
out vec2 TexCoords;

uniform mat4 m4_Projection;
uniform mat4 m4_View;
uniform mat4 m4_Model;
uniform bool b_Editor;

void main()
{
    TexCoords = vertex.zw;

    if(b_Editor)
    {
        gl_Position = vec4(0);
    }
    else
    {
        gl_Position = m4_Projection * m4_View * m4_Model * vec4(vertex.x, vertex.y, 0.0f, 1.0f);
    }
}