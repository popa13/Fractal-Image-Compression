using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathTools.Matrixes;
using MathTools.Extensions;

namespace ImageTools.Transformations
{
    /// <summary>
    /// Class for transforming an image using an AffineTransformation (matrix).
    /// </summary>
    public class AffineTransformation : IPictureTransformation
    {
        private DoubleMatrix[] _transformMatrixes; //Transformation matrix
        private DoubleMatrix[] _translateMatrixes; //Translation matrix
        private double[] _propabilitiesMatrix; //Probabilities matrix
        private Random _rnd = new Random((int)DateTime.Now.Ticks); //Random

        /// <summary>
        /// Constructor of an affine transformation.
        /// </summary>
        /// <param name="transformMatrix">Transform matrix n x n</param>
        /// <param name="translateMatrix">Translate matrix n x 1</param>
        /// <param name="probabilities">Probabilities array, used if one of transform matrixes must be used randomly.</param>
        public AffineTransformation(DoubleMatrix[] transformMatrixes, DoubleMatrix[] translateMatrixes, double[] probabilities = null)
        {
            _transformMatrixes = transformMatrixes;
            _translateMatrixes = translateMatrixes;
            _propabilitiesMatrix = probabilities;
        }

        /// <summary>
        /// Perform the transformation
        /// </summary>
        /// <param name="pixels">Pixels to transform</param>
        /// <returns>Transformed pixels</returns>
        public Matrix<Pixel> Transform(Matrix<Pixel> pixels, bool moveOnExistingImage = false)
        {
            int x, y;
            var newPixels = moveOnExistingImage ? pixels : new Matrix<Pixel>(pixels.DimensionX, pixels.DimensionY);

            for (x = 0; x < pixels.DimensionX; x++)
            {
                for (y = 0; y < pixels.DimensionY; y++)
                {
                    if (_propabilitiesMatrix == null) //Si pas de probabilité, on effectue toutes les transformations
                    {
                        for (int i = 0; i < _transformMatrixes.Length; i++)
                            CalculateTransformation(pixels, newPixels, i, x, y);
                    }
                    else //Si on a une probabilité, on choisit une transformation à effectuer
                        CalculateTransformation(pixels, newPixels, PickRandomMatrixIndex(), x, y);
                }
            }

            

            return newPixels;
        }

        /// <summary>
        /// Perform the transformation on a single pixel.
        /// </summary>
        /// <param name="pixels">Matrix of pixels of the image.</param>
        /// <param name="newPixels">Matrix of pixels of the newly generated image.</param>
        /// <param name="matrixIndex">Transform and translate matrix to use.</param>
        /// <param name="x">X position of the pixel to transform.</param>
        /// <param name="y">Y position of the pixel to transform.</param>
        private void CalculateTransformation(Matrix<Pixel> pixels, Matrix<Pixel> newPixels, int matrixIndex, int x, int y)
        {
            if (pixels[x, y] != null)
            {
                //Get new pixel position
                DoubleMatrix tempNewPixelPos = _transformMatrixes[matrixIndex] * new double[,] { { x }, { y } } + _translateMatrixes[matrixIndex];

                if (tempNewPixelPos[0, 0] >= 0 && tempNewPixelPos[0, 0] < pixels.DimensionX && tempNewPixelPos[1, 0] >= 0 && tempNewPixelPos[1, 0] < pixels.DimensionY)
                {
                    //Set the pixel at the right position in new image.
                    newPixels[(int)tempNewPixelPos[0, 0], (int)tempNewPixelPos[1, 0]] = pixels[x, y];
                }
            }
        }

        /// <summary>
        /// Pick a random matrix index when the affine transformation is random.
        /// </summary>
        /// <returns>Random matrix index.</returns>
        private int PickRandomMatrixIndex()
        {
            double rnd = _rnd.NextDouble();
            double sum = _propabilitiesMatrix[0];
            int index = 0;

            while (true)
            {
                if (rnd < sum)
                    return index;

                sum += _propabilitiesMatrix[++index];
            }
        }
    }
}
