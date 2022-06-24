#version 330
 
struct Material {
    vec3 color;
    float specular;
    
    sampler2D albedoMap;
    sampler2D normalMap;
    sampler2D specularMap;           

    float normalStrength;
    float parallaxStrength;
};
struct Light {
    vec3 position;
    vec3 color;
    float intensity;
};

in vec2 uv;
in vec3 fragPos;
in mat3 TBN;

out vec4 outputColor;

uniform Material material;
uniform Light light;
uniform vec3 cameraPos;
uniform vec3 ambientColor; 
uniform float ambientIntensity;


// https://learnopengl.com/Advanced-Lighting/Normal-Mapping
vec3 sampleNormalMap(in sampler2D tex, vec2 texCoords, in float strength) {
    vec3 n = texture(tex, texCoords).rgb;    
    n = 2.0 * n - 1.0;
    n = mix(vec3(0,0,1), n, strength);
    return normalize(TBN * n); 
}

float shadeDiffuse(in vec3 l, in vec3 n) {
    return max(0, -dot(n, l));
}
float shadeSpecular(in vec3 n, in vec3 l, in vec3 v, in float power) {
    vec3 r = reflect(l, n);
    float rdv = max(0, -dot(r, v));
    return pow(rdv, power);
}

// fragment shader
void main()
{   
    vec3 viewDir = normalize(fragPos - cameraPos);

    vec3 lightToFrag = fragPos - light.position;
    vec3 lightDir = normalize(lightToFrag);
    float attenuation = light.intensity / dot(lightToFrag, lightToFrag);
    
    // texture sampling ---------------------------------------------------------
    vec3 n = sampleNormalMap(material.normalMap, uv, material.normalStrength);
    vec3 color = texture( material.albedoMap, uv).rgb * material.color;
    float specSample = texture(material.specularMap, uv).x;
    // --------------------------------------------------------------------------

    // shading ------------------------------------------------------------------
    vec3 ambient = ambientColor * ambientIntensity * color;
    
    vec3 diffuse = shadeDiffuse(lightDir, n) * attenuation * light.color * color;

    vec3 specular = light.color * shadeSpecular(n, lightDir, viewDir, 32);
    specular *= specSample * material.specular * attenuation;
    // --------------------------------------------------------------------------

    outputColor = vec4(ambient + diffuse + specular, 1.0);
}