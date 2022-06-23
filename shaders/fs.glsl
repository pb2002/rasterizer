#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec3 fragPos;
in mat3 TBN;

struct Material {
    sampler2D albedoMap;
    sampler2D normalMap;
    sampler2D specularMap;        
    
    vec3 color;
    float roughness;
};

uniform Material material;
uniform vec3 lightDir;
uniform vec3 cameraPos;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{   
    vec3 viewDir = normalize(fragPos - cameraPos);
     
    vec3 n = texture(material.normalMap, uv).rgb;
    n = n * 2.0 - 1.0;
    n = normalize(TBN * n);

    vec3 color = texture( material.albedoMap, uv ).rgb * material.color;

    vec3 diffuse = max(0, -dot(n, lightDir)) * color * 2;
    
    float specFactor = texture(material.specularMap, uv).x;
    vec3 refl = reflect(lightDir, n);
    float rdv = max(0, -dot(refl, viewDir));

    vec3 specular = pow(rdv, 24) * specFactor * (1-material.roughness) * vec3(6f);

    vec3 ambient = vec3(0.07, 0.12, 0.15) * color;
    outputColor = vec4(ambient + diffuse + specular, 1.0);
}