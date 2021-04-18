using ImageTools.Transformations;
using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools
{
    /// <summary>
    /// Custom image class.
    /// Basically, it transforms a bitmap into a 2d array of pixels, easier to work with.
    /// </summary>
    public class CustomImage
    {
        /// <summary>
        /// Get or set the pixels of the Image.
        /// </summary>
        public Matrix<Pixel> ImageData { get; set; }
        /// <summary>
        /// Get the width of the Image.
        /// </summary>
        public int Width => ImageData.DimensionX;
        /// <summary>
        /// Get the height of the Image.
        /// </summary>
        public int Height => ImageData.DimensionY;

        /// <summary>
        /// Constructor creating a copy of the image.
        /// </summary>
        /// <param name="img">Custom Image to copy</param>
        public CustomImage(CustomImage img)
        {
            ImageData = new Matrix<Pixel>(img.ImageData);
        }

        /// <summary>
        /// Constructor creating an image from a matrix of pixels
        /// </summary>
        /// <param name="img">Pixels in image</param>
        public CustomImage(Pixel[,] values)
        {
            ImageData = new Matrix<Pixel>(values);
        }

        /// <summary>
        /// Constructor creating the custom image from an existing Bitmap.
        /// </summary>
        /// <param name="bmp">Bitmap to build the pixel matrix from.</param>
        public CustomImage(Bitmap bmp)
        {
            int x, y;

            //Lock the bits for reading and writing
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Size.Width, bmp.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            //Get address of first line
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytesCount = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytesCount];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytesCount);

            //Populate Pixels matrix
            ImageData = new Matrix<Pixel>(bmp.Width, bmp.Height);
            for (x = 0; x < bmp.Width; x++)
            {
                for (y = 0; y < bmp.Height; y++)
                {
                    ImageData[x, y] = new Pixel(rgbValues[x * 4 + bmp.Width * y * 4 + 2], rgbValues[x * 4 + bmp.Width * y * 4 + 1], rgbValues[x * 4 + bmp.Width * y * 4]);
                }
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// Transform the ImageData matrix with the specified transformation.
        /// </summary>
        /// <param name="transform">Transformation to run on the image.</param>
        public void Transform(IPictureTransformation transform, bool moveOnExistingImage = false)
        {
            ImageData = transform.Transform(ImageData, moveOnExistingImage);
        }

        /// <summary>
        /// Get the bitmap represented by the 2D pixel matrix.
        /// </summary>
        /// <returns>Bitmap of the image.</returns>
        public Bitmap GetBitmap()
        {
            try {
                Bitmap bmp = new Bitmap(ImageData.DimensionX, ImageData.DimensionY, PixelFormat.Format32bppRgb);

                //Convert pixels to bytes, then write them
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Size.Width, bmp.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                //Get address of first line
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytesCount = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytesCount];
            
                //Obtient les nouvelles valeurs RGB à mettre dans le bitmap. Si un pixel est null, le met blanc
                for (int i = 0; i < bytesCount; i++)
                {
                    if (i % 4 == 2)
                        rgbValues[i] = ImageData[(i / 4) % ImageData.DimensionX, i / 4 / ImageData.DimensionX]?.Red ?? 255;
                    else if (i % 4 == 1)
                        rgbValues[i] = ImageData[(i / 4) % ImageData.DimensionX, i / 4 / ImageData.DimensionX]?.Green ?? 255;
                    else if (i % 4 == 0)
                        rgbValues[i] = ImageData[(i / 4) % ImageData.DimensionX, i / 4 / ImageData.DimensionX]?.Blue ?? 255;
                }

                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytesCount);

                bmp.UnlockBits(bmpData);

                return bmp;
            }
            catch (ArgumentException arg)
            {
                 return null;
            }
        }
    }
}
