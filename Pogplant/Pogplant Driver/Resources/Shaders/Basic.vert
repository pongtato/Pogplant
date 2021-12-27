#version 450 core

layout (location = 0) in vec2 v2_Pos;
layout (location = 1) in vec2 v2_TexCoords;
layout (location = 2) in vec4 v4_Color;
layout (location = 3) in mat4 m4_Model;
layout (location = 7) in int i_TexID;
layout (location = 8) in int b_Ortho;
layout (location = 9) in int b_GameOnly;

out vec4 ColorCoords;
out vec2 TexCoords;

flat out int TexID;

uniform mat4 m4_Projection;
uniform mat4 m4_View;
uniform float f_Aspect;
uniform bool b_Editor;

void main() 
{
    ColorCoords = v4_Color;
    TexCoords = v2_TexCoords;
    TexID = i_TexID;
    
    mat4 proj = m4_Projection;
    mat4 view = m4_View;
    float yScaled = v2_Pos.y;
    if(b_Ortho == 1)
    {
        proj = mat4(1.0f);
        view = mat4(1.0f);
        yScaled = yScaled * f_Aspect;
    }

    if(b_GameOnly == 1 && b_Editor)
    {
        gl_Position = vec4(0);
    }
    else
    {
        gl_Position = proj * view * m4_Model * vec4(v2_Pos.x, yScaled, 0.0f, 1.0f);
    }
}
