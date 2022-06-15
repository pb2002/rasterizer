using System;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Template
{
    public class Shader2
    {
        public int ProgramID { get; private set; }
        public int VertexShaderID { get; private set; }
        public int FragmentShaderID { get; private set; }

        // loading shaders
        private void Load( string filename, ShaderType type, int program, out int id )
        {
            // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
            id = GL.CreateShader(type);
            using(var sr = new StreamReader( filename )) 
                GL.ShaderSource(id, sr.ReadToEnd());
            
            GL.CompileShader(id);
            GL.AttachShader(program, id);
            Console.WriteLine(GL.GetShaderInfoLog(id));
        }

        public void LoadUniformData<T>(T data) where T : struct
        {
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                var attr = field.GetCustomAttribute<ShaderUniformAttribute>();
                if (attr == null) continue;
                
                var value = field.GetValue(data);

                switch (attr.Type)
                {
                    case UniformType.Float:
                        GL.ProgramUniform1(ProgramID, attr.Location, (float)value);
                        break;
                    case UniformType.Int:
                        GL.ProgramUniform1(ProgramID, attr.Location, (int)value);
                        break;
                    case UniformType.Vec2:
                        GL.ProgramUniform2(ProgramID, attr.Location, (Vector2)value);
                        break;
                    case UniformType.Vec3:
                        GL.ProgramUniform3(ProgramID, attr.Location, (Vector3)value);
                        break;
                    case UniformType.Vec4:
                        GL.ProgramUniform4(ProgramID, attr.Location, (Vector4)value);
                        break;
                    case UniformType.UniformM4x4:
                        var m = (Matrix4) value;
                        GL.ProgramUniformMatrix4(ProgramID, attr.Location, false, ref m);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                
            }
        }
    }

    public enum UniformType
    {
        Float,
        Int,
        Vec2,
        Vec3,
        Vec4,
        UniformM4x4
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class ShaderUniformAttribute : Attribute
    {
        public int Location;
        public UniformType Type;

        public ShaderUniformAttribute(int location, UniformType type)
        {
            Location = location;
            Type = type;
        }
    }
}