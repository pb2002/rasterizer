#version 330
 
in vec3 vPosition;
in vec3 vNormal;	
in vec2 vUV;
in vec3 vTangent;
in vec3 vBitangent;

out vec3 fragPos; 
out vec2 uv;		
out mat3 TBN;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

// vertex shader
void main()
{
	gl_Position = projection * view 
		* model * vec4(vPosition, 1.0);
		
	fragPos = vec3(model * vec4(vPosition, 1.0));
	
	mat3 m3 = mat3(model);
	uv = vUV;

	// Tangent space matrix ----------------------
	// https://learnopengl.com/Advanced-Lighting/Normal-Mapping
    vec3 T = normalize(m3 * vTangent);
    vec3 B = normalize(m3 * vBitangent);
    vec3 N = normalize(m3 * vNormal);

	TBN = mat3(T, B, N);
	// -------------------------------------------
}