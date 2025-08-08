#version 410 core

in vec2 texCoords;
out vec4 FragColor;

uniform sampler2D uTexture;
uniform float uTime;
void main()
{
    vec3 color = texture(uTexture, texCoords).rgb;
    FragColor = vec4(color.r * sin(uTime), color.g * cos(uTime), color.b, 1);
}