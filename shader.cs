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

		// Vertex attributes ------
		public int AttributeVpos;
		public int AttributeVnrm;
		public int AttributeVuvs;
		public int AttributeVtng;
		public int AttributeVbtg;
		// ------------------------
		// constructor
		public Shader(string vertexShader, string fragmentShader)
		{

			// compile shaders
			ProgramID = GL.CreateProgram();
			Console.WriteLine($"Created shader program {ProgramID}.");
			Load(vertexShader, ShaderType.VertexShader, ProgramID, out VsId);
			Load(fragmentShader, ShaderType.FragmentShader, ProgramID, out FsId);

			Console.Write($"Linking program {ProgramID}...");
			GL.LinkProgram(ProgramID);

			var log = GL.GetProgramInfoLog(ProgramID);
			if (string.IsNullOrEmpty(log))
			{
				Console.WriteLine(" done.");
			}
			else
			{
				Console.WriteLine(" error!");
				Console.WriteLine($"An error occured while linking program {ProgramID}:");
				Console.WriteLine(log);
			}

			// get locations of shader parameters
			AttributeVpos = GL.GetAttribLocation(ProgramID, "vPosition");
			AttributeVnrm = GL.GetAttribLocation(ProgramID, "vNormal");
			AttributeVuvs = GL.GetAttribLocation(ProgramID, "vUV");
			AttributeVtng = GL.GetAttribLocation(ProgramID, "vTangent");
			AttributeVbtg = GL.GetAttribLocation(ProgramID, "vBitangent");
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

		public void BindTexture(string name, int texture, int t)
		{
			int texLoc = GL.GetUniformLocation(ProgramID, name);
			GL.Uniform1(texLoc, t);
			GL.ActiveTexture(TextureUnit.Texture0 + t);
			GL.BindTexture(TextureTarget.Texture2D, texture);
		}

		public void BindTexture(string name, Texture texture, int t)
		{
			int texLoc = GL.GetUniformLocation(ProgramID, name);
			GL.Uniform1(texLoc, t);
			GL.ActiveTexture(TextureUnit.Texture0 + t);
			GL.BindTexture(TextureTarget.Texture2D, texture.Id);
		}

		// loading shaders
		void Load(String filename, ShaderType type, int program, out int id)
		{
			Console.Write($"Loading shader '{filename}'...");

			// source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
			id = GL.CreateShader(type);


			using (StreamReader sr = new StreamReader(filename))
			{
				var source = sr.ReadToEnd();
				GL.ShaderSource(id, source);
			}
			Console.WriteLine(" done.");

			Console.Write("    Compiling...");
			GL.CompileShader(id);
			GL.AttachShader(program, id);
			var log = GL.GetShaderInfoLog(id);
			if (string.IsNullOrEmpty(log))
			{
				Console.WriteLine(" done.");
			}
			else
			{
				Console.WriteLine(" error!");
				Console.WriteLine("An error occured while compiling the shader:");
				Console.WriteLine(log);
			}
		}
	}
}
