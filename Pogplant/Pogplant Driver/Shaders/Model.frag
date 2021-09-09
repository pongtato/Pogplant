#version 450 core

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D s2d_texture;

void main()
{    
    FragColor = texture(s2d_texture, TexCoords);
    FragColor = vec4(0.3f,0.3f,0.3f,1.0f);
}