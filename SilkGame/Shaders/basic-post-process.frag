#version 410 core

in vec2 texCoords;
out vec4 FragColor;

uniform sampler2D uMod1;
uniform sampler2D uMod2;

uniform float uTime;
void main()
{
    vec3 color = texture(uMod1, texCoords).rgb;
    vec3 color2 = texture(uMod2, texCoords).rgb;

    FragColor = vec4((color.r + color2.r)* sin(uTime), (color.g + color2.g) * cos(uTime), (color.b + color2.b), 1);
}