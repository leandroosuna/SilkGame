#version 410 core

out vec4 FragColor;

in VS_OUT {
    vec3 worldPos;
    vec3 normal;
    vec2 texCoords;
} fs_in;

uniform vec3 uColor;
uniform vec3 uLightPos;
uniform vec3 uViewPos;
uniform int uUseTexture;

uniform sampler2D uTex;

void main()
{
    vec3 col;
    if(uUseTexture == 1)
    {
        col = texture(uTex, fs_in.texCoords * 4).rgb;
    }
    else
    {
        col = uColor;
    }

    // ambient
    vec3 ambient = 0.05 * col;
    // diffuse
    vec3 lightDir = normalize(uLightPos - fs_in.worldPos);
    vec3 normal = normalize(fs_in.normal);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * col;
    // specular
    vec3 viewDir = normalize(uViewPos - fs_in.worldPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);
    vec3 specular = vec3(0.3) * spec; // assuming bright white light color
       
    FragColor = vec4(ambient + diffuse + specular, 1.0);

}