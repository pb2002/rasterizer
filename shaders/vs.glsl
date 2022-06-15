	#version 330
 
// shader input
in vec3 vPosition;		// untransformed vertex position
in vec3 vNormal;		// untransformed vertex normal
in vec2 vUV;			// vertex uv coordinate
in vec3 vTangent;
in vec3 vBitangent;

// shader output
out vec3 normal;		// transformed vertex normal
out vec2 uv;		
out mat3 TBN;
uniform mat4 view;
uniform mat4 model;
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = view * model * vec4(vPosition, 1.0);
	
	
	mat3 modelVector = transpose(inverse(mat3(model)));
	normal = normalize(modelVector * vNormal);
	uv = vUV;

    vec3 T = normalize(modelVector * vTangent);
    vec3 B = normalize(modelVector * vBitangent);
    vec3 N = normalize(modelVector * vNormal.xyz);

	TBN = mat3(T, B, N);
}