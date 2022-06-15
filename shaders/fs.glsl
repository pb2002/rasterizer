#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec3 normal;			// interpolated normal
in vec3 vPosition;
in mat3 TBN;

uniform sampler2D texAlbedo;	// texture sampler
uniform sampler2D texSpecular;
uniform sampler2D texNormal;
uniform vec3 lightDir;
uniform vec3 cameraPos;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    vec3 n = normalize(normal);   
    float specFactor = texture(texSpecular, uv).x;
     
    // vec3 n = texture(texNormal, uv).rgb;
    
    // n = n * 2.0 - 1.0;
    // n = normalize(TBN * n);

    vec3 color = texture( texAlbedo, uv ).rgb;
    vec3 diffuse = max(0, -dot(n, lightDir)) * color;
    
    vec3 viewDir = normalize(vPosition - cameraPos);
    vec3 refl = reflect(lightDir, n);
    float rdv = max(0, -dot(refl, viewDir));

    vec3 specular = pow(rdv, 64) * (1-specFactor) * vec3(0.2f);

    vec3 ambient = vec3(0.07, 0.12, 0.15) * color;
    outputColor = vec4(ambient + diffuse + specular, 1.0);
}