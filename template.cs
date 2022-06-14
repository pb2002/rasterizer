using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

// The template provides you with a window which displays a 'linear frame buffer', i.e.
// a 1D array of pixels that represents the graphical contents of the window.

// Under the hood, this array is encapsulated in a 'Surface' object, and copied once per
// frame to an OpenGL texture, which is then used to texture 2 triangles that exactly
// cover the window. This is all handled automatically by the template code.

// Before drawing the two triangles, the template calls the Tick method in MyApplication,
// in which you are expected to modify the contents of the linear frame buffer.

// After (or instead of) rendering the triangles you can add your own OpenGL code.

// We will use both the pure pixel rendering as well as straight OpenGL code in the
// tutorial. After the tutorial you can throw away this template code, or modify it at
// will, or maybe it simply suits your needs.

namespace Template
{
	public class OpenTkApp : GameWindow
	{
		static int _screenId;            // unique integer identifier of the OpenGL texture
		static MyApplication _app;       // instance of the application
		static bool _terminated = false; // application terminates gracefully when this is true
		protected override void OnLoad( EventArgs e )
		{
			// called during application initialization
			GL.ClearColor( Color.Black );
			GL.Enable( EnableCap.Texture2D );
			GL.Disable( EnableCap.DepthTest );
			GL.Hint( HintTarget.PerspectiveCorrectionHint, HintMode.Nicest );
			ClientSize = new Size( 512, 512 );
			_app = new MyApplication();
			_app.Screen = new Surface( Width, Height );
			Sprite.Target = _app.Screen;
			_screenId = _app.Screen.GenTexture();
			_app.Init();
		}
		protected override void OnUnload( EventArgs e )
		{
			// called upon app close
			GL.DeleteTextures( 1, ref _screenId );
			Environment.Exit( 0 );      // bypass wait for key on CTRL-F5
		}
		protected override void OnResize( EventArgs e )
		{
			// called upon window resize. Note: does not change the size of the pixel buffer.
			GL.Viewport( 0, 0, Width, Height );
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );
		}
		protected override void OnUpdateFrame( FrameEventArgs e )
		{
			// called once per frame; app logic
			var keyboard = OpenTK.Input.Keyboard.GetState();
			if( keyboard[OpenTK.Input.Key.Escape] ) _terminated = true;
		}
		protected override void OnRenderFrame( FrameEventArgs e )
		{
			// called once per frame; render
			_app.Tick();
			if( _terminated )
			{
				Exit();
				return;
			}
			// set the state for rendering the quad
			GL.ClearColor( Color.Black );
			GL.Enable( EnableCap.Texture2D );
			GL.Disable( EnableCap.DepthTest );
			GL.Color3( 1.0f, 1.0f, 1.0f );
			GL.BindTexture( TextureTarget.Texture2D, _screenId );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
						   _app.Screen.Width, _app.Screen.Height, 0,
						   OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
						   PixelType.UnsignedByte, _app.Screen.Pixels
						 );
			// draw screen filling quad
			GL.MatrixMode( MatrixMode.Modelview );
			GL.LoadIdentity();
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			GL.Begin( PrimitiveType.Quads );
			GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2( 1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2( 1.0f, 1.0f );
			GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -1.0f, 1.0f );
			GL.End();
			// prepare for generic OpenGL rendering
			GL.Enable( EnableCap.DepthTest );
			GL.Clear( ClearBufferMask.DepthBufferBit );
			GL.Disable( EnableCap.Texture2D );
			// do OpenGL rendering
			_app.RenderGL();
			// swap buffers
			SwapBuffers();
		}
		public static void Main( string[] args )
		{
			// entry point
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo( "en-US" );
			using( OpenTkApp app = new OpenTkApp() ) { app.Run( 30.0, 0.0 ); }
		}
	}
}