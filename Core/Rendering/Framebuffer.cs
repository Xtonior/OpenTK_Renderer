using OpenTK.Graphics.OpenGL4;

namespace Engine.Core.Rendering
{
    public class Framebuffer
    {
        private uint fbo, rbo;
        private uint framebufferTexture;

        public Framebuffer(int h, int w)
        {
            // Generating Framebuffer
            GL.GenFramebuffers(1, out fbo);
            GL.GenTextures(1, out framebufferTexture);
            GL.GenBuffers(1, out rbo);

            Bind(w, h);
        }

        public void Bind(int w, int h)
        {
            // Framebuffer texture
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.BindTexture(TextureTarget.Texture2D, framebufferTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Rgb, PixelType.UnsignedByte, System.IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, framebufferTexture, 0);

            // Render Buffer object
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            // Check framebuffer
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new RendererException("Failed initializing framebuffer!");
            }
        }

        public void Unbind()
        {
            // Unbind buffers and textures
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            // GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void Resize(int x, int y)
        {
            GL.BindTexture(TextureTarget.Texture2D, framebufferTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, x, y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, System.IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, framebufferTexture, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, x, y);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
        }

        public uint GetFramebufferTexture()
        {
            return framebufferTexture;
        }
    }
}