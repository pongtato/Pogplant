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

out vec3 FragPos;
out vec3 Normal;
out vec3 ColorTint;
out vec3 EmissiveTint;
out vec2 TexCoords;
flat out int UseLight;
flat out int UseTexture;
flat out int ToDraw;

uniform mat4 m4_View;
uniform mat4 m4_Projection;
uniform int i_EditorMode;

void main()
{
    if(i_EditorOnly == 0 || i_EditorOnly == 1 && i_EditorMode == 1)
    {
        ToDraw = 1;
    }
    else
    {
        ToDraw = 0;
    }

    ColorTint = v3_Color;
    EmissiveTint = v3_EmissiveColor;
    UseLight = i_UseLight;
    UseTexture = i_UseTexture;

    vec4 viewPos = m4_View * m4_Model * vec4(v3_Pos, 1.0);
    FragPos = viewPos.xyz; 

    mat3 normalMatrix = transpose(inverse(mat3(m4_View * m4_Model)));
    Normal = normalMatrix * v3_Normal;

    TexCoords = v2_TexCoords;  

    gl_Position = m4_Projection * viewPos;
}