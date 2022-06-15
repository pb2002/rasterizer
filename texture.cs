using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

namespace Template
{
	public class Texture
	{
		// data members
		public int Id;

		// constructor
		public Texture( string filename )
		{
			if( String.IsNullOrEmpty( filename ) ) throw new ArgumentException( filename );
			Id = GL.GenTexture();
			GL.BindTexture( TextureTarget.Texture2D, Id );

			float aniso = GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropy);
			GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)All.TextureMaxAnisotropy, aniso);
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear );
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);			
			Bitmap bmp = new Bitmap( filename );
			BitmapData bmpData = bmp.LockBits( new Rectangle( 0, 0, bmp.Width, bmp.Height ), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0 );
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			
			bmp.UnlockBits( bmpData );
		}
	}
}
