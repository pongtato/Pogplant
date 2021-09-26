#version 450 core

layout (location = 3) out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube skybox;

void main () 
{
    FragColor = texture(skybox, TexCoords);
    //FragColor = vec4(1,0,0,1);
}