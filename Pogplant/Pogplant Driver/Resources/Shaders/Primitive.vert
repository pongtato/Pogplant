#version 450 core

layout (location = 0) in vec3 v3_Pos;
layout (location = 1) in vec3 v3_Normal;
layout (location = 2) in vec3 v3_Color;
layout (location = 3) in vec2 v2_TexCoords;
layout (location = 3) in vec3 v3_Tangent;
layout (location = 3) in vec3 v3_BiTangent;

out vec3 FragPos;
out vec2 TexCoords;
out vec3 Normal;
out vec3 Color;
out float Height;
out mat3 TBN;

uniform mat4 m4_Model;
uniform mat4 m4_View;
uniform mat4 m4_Projection;

void main()
{
    vec4 worldPos = m4_Model * vec4(v3_Pos, 1.0);
    FragPos = worldPos.xyz; 

    mat3 normalMatrix = transpose(inverse(mat3(m4_Model)));
    vec3 T = normalize(normalMatrix * v3_Tangent);
    vec3 N = normalize(normalMatrix * v3_Normal);
    T = normalize(T - dot(T,N) * N);
    vec3 B = cross(N,T);
    TBN = mat3(T,B,N);
    Normal = normalMatrix * v3_Normal;

    TexCoords = v2_TexCoords; 

    Color = v3_Color;

    Height = v3_Pos.y;

    gl_Position = m4_Projection * m4_View * worldPos;
}