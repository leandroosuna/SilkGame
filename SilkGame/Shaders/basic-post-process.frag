#version 410 core

in vec2 texCoords;
out vec4 FragColor;

uniform sampler2D uR;
uniform sampler2D uG;
uniform sampler2D uB;

//uniform float uTime;

void main()
{
    float r = texture(uR, texCoords).r;
    float g = texture(uG, texCoords).g;
    float b = texture(uB, texCoords).b;

    FragColor = vec4(r,g,b, 1);
}