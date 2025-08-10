#version 410 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vTexCoords;
layout (location = 2) in vec3 vNormal;

out VS_OUT {
    vec3 worldPos;
    vec3 normal;
    vec2 texCoords;
} vs_out;

uniform mat4 uWorld;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    vec4 world = uWorld * vec4(vPos, 1.0);
    vs_out.worldPos = world.xyz;
    vs_out.normal = mat3(transpose(inverse(uWorld))) * vNormal;
    vs_out.texCoords = vTexCoords;
    
    gl_Position = uProjection * uView * uWorld * vec4(vPos, 1.0);;
    
}