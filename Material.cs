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

        public Vector3 Color;
        public float Roughness;

        public Material(Texture albedoMap, Texture normalMap, Texture specularMap, Vector3 color, float roughness)
        {
            AlbedoMap = albedoMap;
            NormalMap = normalMap;
            SpecularMap = specularMap;
            Color = color;
            Roughness = roughness;
        }

        public void Upload(Shader shader)
        {
            shader.SetUniformVector3("lightDir", Vector3.Normalize(new Vector3(1, -1, -0.5f)));
            shader.SetUniformVector3("cameraPos", Camera.Instance.Transform.Position);
            
            shader.BindTexture("material.albedoMap", AlbedoMap, 0);
            shader.BindTexture("material.normalMap", NormalMap, 1);
            shader.BindTexture("material.specularMap", SpecularMap, 2);

            shader.SetUniformVector3("material.color", Color);
            shader.SetUniformFloat("material.roughness", Roughness);
        }
    }
}
