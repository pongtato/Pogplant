#version 450 core

layout (location = 0) in vec2 v2_Pos;
layout (location = 1) in vec2 v2_TexCoords;
layout (location = 2) in vec4 v4_Color;
layout (location = 3) in mat4 m4_Model;
layout (location = 7) in int  i_TexID;
layout (location = 8) in int b_Ortho;

out vec4 ColorCoords;
out vec2 TexCoords;

flat out int TexID;

uniform mat4 m4_Projection;
uniform mat4 m4_View;

void main() 
{
    ColorCoords = v4_Color;
    TexCoords = v2_TexCoords;
    TexID = i_TexID;
    
    mat4 proj = m4_Projection;
    mat4 view = m4_View;
    if(b_Ortho == 1)
    {
        view = mat4(1.0f);
    }

    gl_Position = proj * view * m4_Model * vec4(v2_Pos, 0.0f, 1.0f);
}
