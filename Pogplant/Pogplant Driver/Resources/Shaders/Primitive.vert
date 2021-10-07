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
out mat3 nmMtx;

uniform mat4 m4_Model;
uniform mat4 m4_View;
uniform mat4 m4_Projection;
uniform vec3 v3_ViewPosition;

void main()
{
    vec4 worldPos = m4_Model * vec4(v3_Pos, 1.0);
    FragPos = worldPos.xyz;

    nmMtx = transpose(inverse(mat3(m4_Model)));
    Normal = nmMtx * v3_Normal;
    
    vec3 T = normalize(nmMtx * v3_Tangent);
    vec3 N = normalize(Normal);
    T = normalize(T - dot(T,N) * N);
    vec3 B = cross(N,T);
    TBN = mat3(T,B,N);
    TexCoords = v2_TexCoords; 

    Color = v3_Color;

    Height = v3_Pos.y;

    gl_Position = m4_Projection * m4_View * worldPos;
}