#version 450 core

layout (location = 0) in vec3 v3_Pos;
layout (location = 1) in vec3 v3_Normal;
layout (location = 4) in vec2 v2_TexCoords;
layout (location = 5) in mat4 m4_Model;
layout (location = 9) in vec3 v3_Color;
layout (location = 10) in vec3 v3_EmissiveColor;
layout (location = 11) in int i_UseLight;
layout (location = 12) in int i_UseTexture;
layout (location = 13) in int i_EditorOnly;

void main()
{
    // Dont cast shadows for such objects
    if(i_EditorOnly == 1)
    {
        gl_Position = vec4(0);
    }
    else
    {
        gl_Position = m4_Model * vec4(v3_Pos, 1.0);
    }
}