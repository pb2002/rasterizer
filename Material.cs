using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Template
{
    public class Material
    {
        public Texture AlbedoMap;
        public Texture NormalMap;
        public Texture SpecularMap;
        // public Texture HeightMap;

        public Vector3 Color;
        public float Specular;
        public float NormalStrength;
        public float ParallaxStrength;

        public Material(
            Vector3 color,
            float roughness,
            Texture albedoMap = null, 
            Texture normalMap = null, 
            Texture specularMap = null, 
            // Texture heightMap = null,
            float normalStrength = 1.0f,
            float parallaxStrength = 0.03f
            )
        {
            Color = color;
            Specular = roughness;

            AlbedoMap = albedoMap ?? Texture.White;
            NormalMap = normalMap ?? Texture.DefaultNormal;
            SpecularMap = specularMap ?? Texture.White;
            // HeightMap = heightMap ?? Texture.White;
            
            NormalStrength = normalStrength;
            ParallaxStrength = parallaxStrength;
        }

        public void Upload(Shader shader)
        {
            shader.SetUniformVector3("cameraPos", Camera.Instance.Transform.Position);

            shader.SetUniformVector3("material.color", Color);
            shader.SetUniformFloat("material.specular", Specular);

            shader.BindTexture("material.albedoMap", AlbedoMap, 0);
            shader.BindTexture("material.normalMap", NormalMap, 1);
            shader.BindTexture("material.specularMap", SpecularMap, 2);
            // shader.BindTexture("material.heightMap", HeightMap, 3);

            shader.SetUniformFloat("material.normalStrength", NormalStrength);
            shader.SetUniformFloat("material.parallaxStrength", ParallaxStrength);
        }
    }
}
