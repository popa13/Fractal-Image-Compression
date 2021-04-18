using ImageTools.Compression.Fractal;
using ImageTools.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.ImageReaders
{
    /// <summary>
    /// Class to read fractal images from disk.
    /// </summary>
    public class FractalImageReader : ImageReader<FractalCompressionResult>
    {
        /// <summary>
        /// Get Image From bytes of the image.
        /// </summary>
        /// <param name="bytes">Bytes of the image.</param>
        /// <returns>Image represented in a result class.</returns>
        protected override FractalCompressionResult GetImageFromBytes(byte[] bytes)
        {
            int index = 0;
            BitArray array = new BitArray(bytes);
            int length = array.Length, rCountY, totalRCount;
            byte transformationTranslateSize, brightness, contrast, transformationType;
            int diX, diY;
            FractalCompressionResult.FractalTransformation[,] transformations;

            //Put TransformationTranslateSize
            short rCountX = Convert.ToInt16(array.GetBitsAtIndexFromNLastBits(0, 10));
            index += 10;
            //Put size of transformations
            transformationTranslateSize = Convert.ToByte(array.GetBitsAtIndexFromNLastBits(index, 5));
            index += 5;

            //Calculate rCountY
            rCountY = (length - 15) / 3 / (transformationTranslateSize * 2 + 15) / rCountX;
            totalRCount = rCountY * rCountX;
            transformations = new FractalCompressionResult.FractalTransformation[totalRCount, 3];

            //Put transformations
            for (int i = 0; i < totalRCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    diX = Convert.ToInt32(array.GetBitsAtIndexFromNLastBits(index, transformationTranslateSize));
                    index += transformationTranslateSize;
                    diY = Convert.ToInt32(array.GetBitsAtIndexFromNLastBits(index, transformationTranslateSize));
                    index += transformationTranslateSize;
                    brightness = Convert.ToByte(array.GetBitsAtIndexFromNLastBits(index, 7));
                    index += 7;
                    contrast = Convert.ToByte(array.GetBitsAtIndexFromNLastBits(index, 5));
                    index += 5;
                    transformationType = Convert.ToByte(array.GetBitsAtIndexFromNLastBits(index, 3));
                    index += 3;
                    transformations[i, j] = new FractalCompressionResult.FractalTransformation(diX, diY, brightness, contrast, transformationType);
                }
            }

            return new FractalCompressionResult(transformations, transformationTranslateSize, rCountX);
        }
    }
}
