using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Utils
{
    /// <summary>
    /// Classe contenant plusieurs méthodes d'aide pour travailler avec les images.
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// Calcul le PSNR entre 2 images (plus c'est haut mieux c'est) pour la composante Y.
        /// See https://en.wikipedia.org/wiki/Peak_signal-to-noise_ratio for more info.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Valeur du PSNR pour Y</returns>
        public static double CalculatePSNRForY(CustomImage imgA, CustomImage imgB)
        {
            double mse = CalculateMSEForY(imgA, imgB);

            if (mse == 0d)
                return Double.NaN;
            else
                return 10 * Math.Log10((255 << 1) / mse);
        }

        /// <summary>
        /// Calcul le PSNR entre 2 images (plus c'est haut mieux c'est) pour la composante I.
        /// See https://en.wikipedia.org/wiki/Peak_signal-to-noise_ratio for more info.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Valeur du PSNR pour I</returns>
        public static double CalculatePSNRForI(CustomImage imgA, CustomImage imgB)
        {
            double mse = CalculateMSEForI(imgA, imgB);

            if (mse == 0d)
                return Double.NaN;
            else
                return 10 * Math.Log10((302 << 1) / mse);
        }

        /// <summary>
        /// Calcul le PSNR entre 2 images (plus c'est haut mieux c'est) pour la composante Q.
        /// See https://en.wikipedia.org/wiki/Peak_signal-to-noise_ratio for more info.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Valeur du PSNR pour Q</returns>
        public static double CalculatePSNRForQ(CustomImage imgA, CustomImage imgB)
        {
            double mse = CalculateMSEForQ(imgA, imgB);

            if (mse == 0d)
                return Double.NaN;
            else
                return 10 * Math.Log10((266 << 1) / mse);
        }

        /// <summary>
        /// Calculate the Mean Squared Error between 2 images on Y component.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Value of the PSNR for Y component</returns>
        public static double CalculateMSEForY(CustomImage imgA, CustomImage imgB)
        {
            if (imgA.Height != imgB.Height || imgA.Width != imgB.Width)
                throw new Exception("Size of 2 images is not the same!");

            int x, y;
            long squaredSum = 0L;

            for (x = 0; x < imgA.Width; x++)
            {
                for (y = 0; y < imgA.Height; y++)
                {
                    squaredSum += Convert.ToInt64(Math.Pow(imgA.ImageData[x, y].Y - imgB.ImageData[x, y].Y, 2));
                }
            }

            return squaredSum / (imgA.Width * imgA.Height);
        }

        /// <summary>
        /// Calculate the Mean Squared Error between 2 images on I component.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Value of the PSNR for I component</returns>
        public static double CalculateMSEForI(CustomImage imgA, CustomImage imgB)
        {
            if (imgA.Height != imgB.Height || imgA.Width != imgB.Width)
                throw new Exception("Size of 2 images is not the same!");

            int x, y;
            long squaredSum = 0L;

            for (x = 0; x < imgA.Width; x++)
            {
                for (y = 0; y < imgA.Height; y++)
                {
                    squaredSum += Convert.ToInt64(Math.Pow(imgA.ImageData[x, y].I - imgB.ImageData[x, y].I, 2));
                }
            }

            return squaredSum / (imgA.Width * imgA.Height);
        }

        /// <summary>
        /// Calculate the Mean Squared Error between 2 images on Q component.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Value of the PSNR for Q component</returns>
        public static double CalculateMSEForQ(CustomImage imgA, CustomImage imgB)
        {
            if (imgA.Height != imgB.Height || imgA.Width != imgB.Width)
                throw new Exception("Size of 2 images is not the same!");

            int x, y;
            long squaredSum = 0L;

            for (x = 0; x < imgA.Width; x++)
            {
                for (y = 0; y < imgA.Height; y++)
                {
                    squaredSum += Convert.ToInt64(Math.Pow(imgA.ImageData[x, y].Q - imgB.ImageData[x, y].Q, 2));
                }
            }

            return squaredSum / (imgA.Width * imgA.Height);
        }

        /// <summary>
        /// Calculate the differences image between 2 images.
        /// </summary>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        /// <returns>Difference images.</returns>
        public static CustomImage CalculateDifferencesImage(CustomImage imgA, CustomImage imgB)
        {
            if (imgA.Height != imgB.Height || imgA.Width != imgB.Width)
                throw new ArgumentException("Both images must have the same size.");

            int x, y;
            byte value;
            Matrix<Pixel> pixels = new Pixel[imgA.Width, imgA.Height];

            for (x = 0; x < pixels.DimensionX; x++)
            {
                for (y = 0; y < pixels.DimensionY; y++)
                {
                    value = Convert.ToByte(Math.Min(255, (Math.Abs(imgA.ImageData[x, y].Red - imgB.ImageData[x, y].Red) +
                        Math.Abs(imgA.ImageData[x, y].Green - imgB.ImageData[x, y].Green) +
                        Math.Abs(imgA.ImageData[x, y].Blue - imgB.ImageData[x, y].Blue))));
                    pixels[x, y] = new Pixel(value, value, value);
                }
            }

            return new CustomImage(pixels);
        }
    }
}
