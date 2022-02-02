#version 450 core

layout (location = 3) out vec4 FragColor;
layout (location = 5) out vec4 gShaft;

in vec3 TexCoords;

uniform samplerCube skybox;

void main () 
{
    FragColor = texture(skybox, TexCoords);
    gShaft = vec4(0.0f);
    
    //float brightness = dot(FragColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    //if(brightness > 0.42f)
    //{
    //    gShaft = texture(skybox, TexCoords);
    //}
    //else
    //{
    //    gShaft = vec4(0.0f);
    //}
}
