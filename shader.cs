using System;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Template
{
	public class Shader
	{
		// data members
		public int ProgramID, VsId, FsId;
		public int AttributeVpos;
		public int AttributeVnrm;
		public int AttributeVuvs;
		public int AttributeVtng;
		public int AttributeVbtg;

		// constructor
		public Shader( string vertexShader, string fragmentShader )
		{
			// compile shaders
			ProgramID = GL.CreateProgram();
			Load( vertexShader, ShaderType.VertexShader, ProgramID, out VsId );
			Load( fragmentShader, ShaderType.FragmentShader, ProgramID, out FsId );
			GL.LinkProgram( ProgramID );
			Console.WriteLine( GL.GetProgramInfoLog( ProgramID ) );
			
			// get locations of shader parameters
			AttributeVpos = GL.GetAttribLocation( ProgramID, "vPosition" );
			AttributeVnrm = GL.GetAttribLocation( ProgramID, "vNormal" );
			AttributeVuvs = GL.GetAttribLocation( ProgramID, "vUV" );
			AttributeVtng = GL.GetAttribLocation( ProgramID, "vTangent");
			AttributeVbtg = GL.GetAttribLocation( ProgramID, "vBitangent");
		}

		public void Use()
		{
			GL.UseProgram(ProgramID);
		}
		public void SetUniformFloat(string name, float value)
        {
			var loc = GL.GetUniformLocation(ProgramID, name);
			GL.ProgramUniform1(ProgramID, loc, value);
		}
		public void SetUniformInt(string name, int value)
		{
			var loc = GL.GetUniformLocation(ProgramID, name);
			GL.ProgramUniform1(ProgramID, loc, value);
		}		
		public void SetUniformVector2(string name, Vector2 value)
		{
			var loc = GL.GetUniformLocation(ProgramID, name);
			GL.ProgramUniform2(ProgramID, loc, value);
		}
		public void SetUniformVector3(string name, Vector3 value)
		{
			var loc = GL.GetUniformLocation(ProgramID, name);
			GL.ProgramUniform3(ProgramID, loc, value);
		}
		public void SetUniformVector4(string name, Vector4 value)
        {
			var loc = GL.GetUniformLocation(ProgramID, name);
			GL.ProgramUniform4(ProgramID, loc, value);
		}
		public void SetUniformMatrix4(string name, Matrix4 value)
        {
            var loc = GL.GetUniformLocation(ProgramID, name);
			GL.ProgramUniformMatrix4(ProgramID, loc, false, ref value);
		}
		
		public void BindTexture(string name, Texture texture, int t)
		{
			int texLoc = GL.GetUniformLocation( ProgramID, name );
			GL.Uniform1( texLoc, t );
			GL.ActiveTexture(TextureUnit.Texture0 + t);
			GL.BindTexture(TextureTarget.Texture2D, texture.Id);
		}

		// loading shaders
        void Load( String filename, ShaderType type, int program, out int id )
		{
			// source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
			id = GL.CreateShader( type );
			using (StreamReader sr = new StreamReader(filename))
			{
				GL.ShaderSource( id, sr.ReadToEnd() );
			}
			GL.CompileShader( id );
			GL.AttachShader( program, id );
			Console.WriteLine( GL.GetShaderInfoLog( id ) );
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
