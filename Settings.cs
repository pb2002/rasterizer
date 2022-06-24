using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public static class Settings
    {
        public static readonly Vector3 ambientColor = new Vector3(0.46f, 0.7f, 1.0f);
        public static readonly float ambientIntensity = 0.2f;
        public static readonly Light light = new Light()
        {
            position = new Vector3(6.0f, 8.0f, 12.0f),
            color = new Vector3(1.0f, 0.7f, 0.5f),
            intensity = 5000.0f,
        };        
        
        public static void Upload(Shader shader)
        {
            shader.SetUniformVector3("ambientColor", ambientColor);
            shader.SetUniformFloat("ambientIntensity", ambientIntensity);

            shader.SetUniformVector3("light.position", light.position);
            shader.SetUniformVector3("light.color", light.color);
            shader.SetUniformFloat("light.intensity", light.intensity);            
        }
    }
    
    public struct Light 
    {
        public Vector3 position;
        public Vector3 color;
        public float intensity;
    }
}
