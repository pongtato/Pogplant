#version 450 core

layout (location = 0) in vec3 v3_Pos;
layout (location = 1) in vec3 v3_Normal;
layout (location = 4) in vec2 v2_TexCoords;

out vec3 FragPos;
out vec2 TexCoords;
out vec3 Normal;

uniform mat4 m4_Model;
uniform mat4 m4_View;
uniform mat4 m4_Projection;

void main()
{
    vec4 worldPos = m4_Model * vec4(v3_Pos, 1.0);
    FragPos = worldPos.xyz; 

    mat3 normalMatrix = transpose(inverse(mat3(m4_Model)));
    Normal = normalMatrix * v3_Normal;

    TexCoords = v2_TexCoords;  

    gl_Position = m4_Projection * m4_View * worldPos;
}