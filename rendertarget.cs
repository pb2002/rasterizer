using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// based on http://www.opentk.com/doc/graphics/frame-buffer-objects

namespace Template
{
	class RenderTarget
	{
		uint _fbo;
		int _colorTexture;
		uint _depthBuffer;
		int _width, _height;
		public RenderTarget( int screenWidth, int screenHeight )
		{
			_width = screenWidth;
			_height = screenHeight;
			// create color texture
			GL.GenTextures( 1, out _colorTexture );
			GL.BindTexture( TextureTarget.Texture2D, _colorTexture );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, _width, _height, 0, PixelFormat.Rgba, PixelType.HalfFloat, IntPtr.Zero );
			GL.BindTexture( TextureTarget.Texture2D, 0 );
			// bind color and depth textures to fbo
			GL.Ext.GenFramebuffers( 1, out _fbo );
			GL.Ext.GenRenderbuffers( 1, out _depthBuffer );
			GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, _fbo );
			GL.Ext.FramebufferTexture( FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, _colorTexture, 0 );
			GL.Ext.BindRenderbuffer( RenderbufferTarget.RenderbufferExt, _depthBuffer );
			GL.Ext.RenderbufferStorage( RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent24, _width, _height );
			GL.Ext.FramebufferRenderbuffer( FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, _depthBuffer );
			// test FBO integrity
			bool untestedBoolean = CheckFboStatus();
			GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // return to regular framebuffer
		}
		public int GetTextureId()
		{
			return _colorTexture;
		}
		public void Bind()
		{
			GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, _fbo );
			GL.Clear( ClearBufferMask.DepthBufferBit );
			GL.Clear( ClearBufferMask.ColorBufferBit );
		}
		public void Unbind()
		{
			// return to regular framebuffer
			GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 );
		}
		private bool CheckFboStatus()
		{
			switch( GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) )
			{
			case FramebufferErrorCode.FramebufferCompleteExt:
				Console.WriteLine( "FBO: The framebuffer is complete and valid for rendering." );
				return true;
			case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
				Console.WriteLine( "FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume." );
				break;
			case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
				Console.WriteLine( "FBO: There are no attachments." );
				break;
			case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
				Console.WriteLine( "FBO: Attachments are of different size. All attachments must have the same width and height." );
				break;
			case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
				Console.WriteLine( "FBO: The color attachments have different format. All color attachments must have the same format." );
				break;
			case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
				Console.WriteLine( "FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment." );
				break;
			case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
				Console.WriteLine( "FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment." );
				break;
			case FramebufferErrorCode.FramebufferUnsupportedExt:
				Console.WriteLine( "FBO: This particular FBO configuration is not supported by the implementation." );
				break;
			default:
				Console.WriteLine( "FBO: Status unknown." );
				break;
			}
			return false;
		}
	}
}