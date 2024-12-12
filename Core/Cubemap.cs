using System.IO;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Engine.Core.Texturing
{
    public static class Cubemap
    {
        public static uint LoadCubeMap(string[] faces)
        {
            uint texID;

            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.TextureCubeMap, texID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            for (int i = 0; i < 6; i++)
            {
                using (var stream = File.OpenRead(faces[i]))
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return texID;
        }
    }
}