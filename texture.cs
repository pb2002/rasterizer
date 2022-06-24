using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class Texture
    {
        // data members
        public int Id;

        public Texture(Bitmap bmp,
                        TextureMinFilter minFilter = TextureMinFilter.LinearMipmapLinear,
                        TextureMagFilter magFilter = TextureMagFilter.Linear,
                        TextureWrapMode wrapMode = TextureWrapMode.Repeat,
                        bool mipmaps = true,
                        bool aniso = true)
        {
            Id = GL.GenTexture();
            Console.WriteLine($"    Texture ID is {Id}.");

            GL.BindTexture(TextureTarget.Texture2D, Id);
            
            if (aniso)
            {
                float a = GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropy);
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)All.TextureMaxAnisotropy, a);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            Console.Write($"    Loading texture data...");
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);           
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits(bmpData);
            Console.WriteLine($" done.");

            if (mipmaps)
            {
                Console.Write($"    Generating mipmaps...");
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                Console.WriteLine($" done.");
            }
        }

        public Texture(string filename,
                        TextureMinFilter minFilter = TextureMinFilter.LinearMipmapLinear,
                        TextureMagFilter magFilter = TextureMagFilter.Linear,
                        TextureWrapMode wrapMode = TextureWrapMode.Repeat,
                        bool mipmaps = true,
                        bool aniso = true)
            : this(LoadFromFile(filename),
                    minFilter, magFilter, wrapMode, mipmaps, aniso)
        {

        }
        private static Bitmap LoadFromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("A file name must be specified");
            if (!System.IO.File.Exists(filename)) throw new ArgumentException($"file '{filename}' does not exist.");            
            try
            {
                Console.Write($"Loading texture '{filename}'...");
                var bmp = new Bitmap(filename);
                Console.WriteLine($" done.");

                return bmp;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw e;
            }
        }

        public static Texture White
        {
            get
            {
                Bitmap bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, Color.White);
                return new Texture(bmp, 
                    minFilter: TextureMinFilter.Nearest, 
                    magFilter: TextureMagFilter.Nearest, 
                    mipmaps: false, 
                    aniso: false);
            }
        }
        public static Texture Black
        {
            get
            {
                Bitmap bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, Color.Black);
                return new Texture(bmp,
                    minFilter: TextureMinFilter.Nearest,
                    magFilter: TextureMagFilter.Nearest,
                    mipmaps: false,
                    aniso: false);
            }
        }
        public static Texture DefaultNormal
        {
            get
            {
                Bitmap bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, Color.FromArgb(127, 127, 255));
                return new Texture(bmp,
                    minFilter: TextureMinFilter.Nearest,
                    magFilter: TextureMagFilter.Nearest,
                    mipmaps: false,
                    aniso: false);
            }
        }
    }
}
