using ImageTools.Compression.Fractal;
using ImageTools.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.ImageWriters
{
    /// <summary>
    /// Class used to save a Fractal Image on disk.
    /// </summary>
    public class FractalImageWriter : ImageWriter<FractalCompressionResult>
    {
        public FractalImageWriter()
        {

        }

        /// <summary>
        /// Get the bytes of the Fractal image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        protected override byte[] GetBytes(FractalCompressionResult image)
        {
            int length = GetBitCount(image);
            int index = 0;
            int transformCountX = image.Transformations.GetLength(0);
            int transformCountY = image.Transformations.GetLength(1);
            byte[] bytes = new byte[(int)Math.Ceiling(length / 8d)];
            BitArray array = new BitArray(length);

            //Put TransformationTranslateSize
            array.SetBitsAtIndexFromNLastBits(0, 10, image.RCountX);
            index += 10;
            //Put size of transformations
            array.SetBitsAtIndexFromNLastBits(index, 5, image.TransformationTranslateSize);
            index += 5;

            //Put transformations
            for (int i = 0; i < transformCountX; i++)
            {
                for (int j = 0; j < transformCountY; j++)
                {
                    array.SetBitsAtIndexFromNLastBits(index, image.TransformationTranslateSize, image.Transformations[i, j].DiX);
                    index += image.TransformationTranslateSize;
                    array.SetBitsAtIndexFromNLastBits(index, image.TransformationTranslateSize, image.Transformations[i, j].DiY);
                    index += image.TransformationTranslateSize;
                    array.SetBitsAtIndexFromNLastBits(index, 7, image.Transformations[i, j].BrightnessTransformation);
                    index += 7;
                    array.SetBitsAtIndexFromNLastBits(index, 5, image.Transformations[i, j].ContrastTransformation);
                    index += 5;
                    array.SetBitsAtIndexFromNLastBits(index, 3, image.Transformations[i, j].TransformationType);
                    index += 3;
                }
            }

            array.CopyTo(bytes, 0);
            return bytes;
        }

        /// <summary>
        /// Get the number of bits contained in the file.
        /// </summary>
        /// <param name="image">Fractal Compression result to save</param>
        /// <returns>Number of bits taken in memory for image.</returns>
        private int GetBitCount(FractalCompressionResult image)
        {
            int length = 0;
            int transformCount = image.Transformations.GetLength(0) * image.Transformations.GetLength(1);

            //Size * 2 because of translation in X and in Y
            //15 because 7 bits for Oi, 5 bits for Si and 3 bits for transformation (7 + 5 + 3 = 15)
            length += transformCount * ((image.TransformationTranslateSize * 2) + 15);

            //Adds 10 bits for the size in X Ri of the image (Max 1024)
            //Adds 5 other bits for the size of the transformations (Max 65536, given by 2^16, where 16 can be given on 5 bits)
            length += 15;

            return length;
        }
    }
}
