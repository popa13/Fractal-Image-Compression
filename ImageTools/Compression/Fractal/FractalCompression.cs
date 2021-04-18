using ImageTools.Utils;
using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Compression.Fractal
{
    /// <summary>
    /// Class for compressing image using Fractal Compression.
    /// </summary>
    public class FractalCompression
    {
        private readonly FractalCompressionOptions _options;

        public const double S_MIN = 0;
        public const double S_MAX = 1d;
        public const double O_MIN = -255d;
        public const double O_MAX = 511d;
        public const double QR_BRIGHTNESS = (O_MAX - O_MIN) / 127d;
        public const double QR_CONTRAST = S_MAX / 15d;
        public const int S_BITS = 5;
        public const int O_BITS = 7;
        public const int S_BITS_VALUE = 1 << S_BITS;
        public const int O_BITS_VALUE = 1 << O_BITS;
        public const int Y_LEVELS = 255;
        public const int I_LEVELS = 302;
        public const int Q_LEVELS = 133;

        /// <summary>
        /// 8 Transformations matrix.
        /// </summary>
        private readonly sbyte[][,] _transformMatrixes = new sbyte[][,]
        {
            new sbyte[,] { { 1, 0 }, { 0, 1 } }, //Identity
            new sbyte[,] { { -1, 0 }, { 0, 1 } }, //Reflection on Y axis
            new sbyte[,] { { 1, 0 }, { 0, -1 } }, //Reflection on X axis
            new sbyte[,] { { 0, 1 }, { 1, 0} }, //Reflection on 45 degree line
            new sbyte[,] { { 0, -1 }, { -1, 0 } }, //Reflection on -45 degree line
            new sbyte[,] { { -1, 0 }, { 0, -1 } }, //180 degrees rotation
            new sbyte[,] { { 0, -1 }, { 1, 0 } }, //90 degrees rotation
            new sbyte[,] { { 0, 1 }, { -1, 0 } }, //270 degrees rotation
        };

        /// <summary>
        /// Constructor of a Fractal Image Compression.
        /// </summary>
        /// <param name="options">Options to use in the algorithm</param>
        public FractalCompression(FractalCompressionOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Compress image.
        /// </summary>
        /// <param name="image">Image to compress.</param>
        /// <returns>Result of the compression.</returns>
        public FractalCompressionResult Compress(CustomImage image)
        {
            //Validate the image and the params
            if (image.Width % _options.RSquareSize > 0 || image.Height % _options.RSquareSize > 0 || _options.DSquareRatio <= 1)
                throw new InvalidOperationException("The compression options specified can't compress this image as the R squares size is not a divisor of height and width. Ri Sizes must be divisor of Di sizes too.");

            object finalResultLock = new object();
            int rXCount = image.Width / _options.RSquareSize; //Number of R Squares in X
            int rYCount = image.Height / _options.RSquareSize; //Number of R Squares in Y
            int totalRCount = rXCount * rYCount;
            byte dSquareSize = Convert.ToByte(_options.RSquareSize * _options.DSquareRatio);
            int maxDxPos = image.Width - dSquareSize + 1; 
            int maxDyPos = image.Height - dSquareSize + 1; 
            byte dSize = dSquareSize;
            byte rSize = _options.RSquareSize;
            int n = _options.RSquareSize * _options.RSquareSize;
            double drRatio = (double)_options.RSquareSize / (double)dSquareSize;
            int drInverseRatio = dSquareSize / _options.RSquareSize;
            double pixelAverageSquareSize = (double)drInverseRatio * (double)drInverseRatio;
            var translateMatrixes = GetTranslateMatrixes(Convert.ToSByte(rSize));
            int transformationsCount = _transformMatrixes.Length;
            var transformations = new FractalCompressionResult.FractalTransformation[totalRCount, 3]; //Transformations (2 dimension to keep transformations for each ri in each color component)
            int rx;
            var rawImgData = image.ImageData;
            var imgDataY = new byte[image.Width, image.Height];
            var imgDataI = new short[image.Width, image.Height];
            var imgDataQ = new short[image.Width, image.Height];
            byte[,][][,] tempDiY = new byte[maxDxPos, maxDyPos][][,];
            short[,][][,] tempDiI = new short[maxDxPos, maxDyPos][][,];
            short[,][][,] tempDiQ = new short[maxDxPos, maxDyPos][][,];
            int[,] sumsOfDY = new int[maxDxPos, maxDyPos];
            int[,] sumsOfDYSquared = new int[maxDxPos, maxDyPos];
            int[,] sumsOfDI = new int[maxDxPos, maxDyPos];
            int[,] sumsOfDISquared = new int[maxDxPos, maxDyPos];
            int[,] sumsOfDQ = new int[maxDxPos, maxDyPos];
            int[,] sumsOfDQSquared = new int[maxDxPos, maxDyPos];

            //Fill imgData Y, I and Q
            Parallel.For(0, image.Width, dx =>
            {
                for (int dy = 0; dy < image.Height; dy++)
                {
                    imgDataY[dx, dy] = rawImgData[dx, dy].Y;
                    imgDataI[dx, dy] = rawImgData[dx, dy].I;
                    imgDataQ[dx, dy] = rawImgData[dx, dy].Q;
                }
            });

            //Average pixels of each Di to size of Ri
            Parallel.For(0, maxDxPos, dx =>
            {
                int i, x, y, j, k, newX, newY, indexX = 0, indexY = 0;
                sbyte[,] tempTransformMatrix;
                sbyte[] tempTranslateMatrix;
                byte[,] tempPixelY = null;
                short[,] tempPixelI = null;
                short[,] tempPixelQ = null;
                int tempSumY = 0, tempSumI = 0, tempSumQ = 0;
                byte tempAveragePixelY = 0;
                short tempAveragePixelI = 0, tempAveragePixelQ = 0;
                byte[,] tempDY = new byte[rSize, rSize];
                short[,] tempDI = new short[rSize, rSize];
                short[,] tempDQ = new short[rSize, rSize];

                for (int dy = 0; dy < maxDyPos; dy++)
                {
                    tempDiY[dx, dy] = new byte[transformationsCount][,];
                    tempDiI[dx, dy] = new short[transformationsCount][,];
                    tempDiQ[dx, dy] = new short[transformationsCount][,];

                    for (i = 0; i < transformationsCount; i++)
                    {
                        tempPixelY = new byte[rSize, rSize];
                        tempPixelI = new short[rSize, rSize];
                        tempPixelQ = new short[rSize, rSize];

                        for (x = 0; x < dSize; x += drInverseRatio) //Increment of inverse ratio to get only parts of Di
                        {
                            for (y = 0; y < dSize; y += drInverseRatio)
                            {
                                //Get transformation
                                tempTransformMatrix = _transformMatrixes[i];
                                tempTranslateMatrix = translateMatrixes[i];

                                //Average pixels on first transform
                                if (i == 0)
                                {
                                    //Reset temp variables
                                    tempSumY = 0;
                                    tempSumI = 0;
                                    tempSumQ = 0;

                                    //Average pixels
                                    for (j = 0; j < drInverseRatio; j++)
                                    {
                                        for (k = 0; k < drInverseRatio; k++)
                                        {
                                            tempSumY += imgDataY[dx + x + j, dy + y + k];
                                            tempSumI += imgDataI[dx + x + j, dy + y + k];
                                            tempSumQ += imgDataQ[dx + x + j, dy + y + k];
                                        }
                                    }

                                    //Calculating average Pixel Components
                                    tempAveragePixelY = Convert.ToByte(tempSumY / pixelAverageSquareSize);
                                    tempAveragePixelI = Convert.ToInt16(tempSumI / pixelAverageSquareSize);
                                    tempAveragePixelQ = Convert.ToInt16(tempSumQ / pixelAverageSquareSize);

                                    //Keep reference to subsampled Di.
                                    tempDY[x / drInverseRatio, y / drInverseRatio] = tempAveragePixelY;
                                    tempDI[x / drInverseRatio, y / drInverseRatio] = tempAveragePixelI;
                                    tempDQ[x / drInverseRatio, y / drInverseRatio] = tempAveragePixelQ;

                                    //Sum of "a" and "a^2" for future RMS calculation
                                    sumsOfDY[dx, dy] += tempAveragePixelY;
                                    sumsOfDI[dx, dy] += tempAveragePixelI;
                                    sumsOfDQ[dx, dy] += tempAveragePixelQ;
                                    sumsOfDYSquared[dx, dy] += tempAveragePixelY * tempAveragePixelY;
                                    sumsOfDISquared[dx, dy] += tempAveragePixelI * tempAveragePixelI;
                                    sumsOfDQSquared[dx, dy] += tempAveragePixelQ * tempAveragePixelQ;
                                }

                                newX = tempTransformMatrix[0, 0] * (x / drInverseRatio) + tempTransformMatrix[0, 1] * (y / drInverseRatio) + tempTranslateMatrix[0];
                                newY = tempTransformMatrix[1, 0] * (x / drInverseRatio) + tempTransformMatrix[1, 1] * (y / drInverseRatio) + tempTranslateMatrix[1];

                                indexX = x / drInverseRatio;
                                indexY = y / drInverseRatio;

                                //Store pixel components for transformed Di (Fi)
                                tempPixelY[newX, newY] = tempDY[indexX, indexY];
                                tempPixelI[newX, newY] = tempDI[indexX, indexY];
                                tempPixelQ[newX, newY] = tempDQ[indexX, indexY];
                            }
                        }

                        //Store pixels of Di
                        tempDiY[dx, dy][i] = tempPixelY;
                        tempDiI[dx, dy][i] = tempPixelI;
                        tempDiQ[dx, dy][i] = tempPixelQ;
                    }
                }
            });

            //Find the Transformations to do...
            //For each Ri
            for (rx = 0; rx < rXCount; rx++)
            {
                //Parallel execution of for loop, so it runs faster.
                Parallel.For(0, rYCount, ry =>
                {
                    //Calculate the sums of components in Ri for future RMS metric calculation
                    int sumOfRY = 0;
                    int sumOfRYSquared = 0;
                    int sumOfRI = 0;
                    int sumOfRISquared = 0;
                    int sumOfRQ = 0;
                    int sumOfRQSquared = 0;
                    int sumOfDAndRY;
                    int sumOfDAndRI;
                    int sumOfDAndRQ;
                    int x, y, indexX, indexY, dx, dy, i, sYInt, oYInt, sIInt, oIInt, sQInt, oQInt,
                        bestRmsYIndexX = -1, bestRmsYIndexY = -1, bestRmsYTransformationIndex = -1,
                        bestRmsIIndexX = -1, bestRmsIIndexY = -1, bestRmsITransformationIndex = -1,
                        bestRmsQIndexX = -1, bestRmsQIndexY = -1, bestRmsQTransformationIndex = -1,
                        bestOY = 0, bestSY = 0,
                        bestOI = 0, bestSI = 0,
                        bestOQ = 0, bestSQ = 0;
                    byte[,] tempPixelY = null;
                    short[,] tempPixelI = null;
                    short[,] tempPixelQ = null;
                    double sY, oY,
                        sI, oI,
                        sQ, oQ,
                        tempRmsY, tempRmsI, tempRmsQ,
                        tempSYDenominator, tempSIDenominator, tempSQDenominator,
                        finalRmsY = Double.MaxValue, finalRmsI = Double.MaxValue, finalRmsQ = Double.MaxValue;

                    for (x = 0; x < rSize; x++)
                    {
                        for (y = 0; y < rSize; y++)
                        {
                            //Sum of "b" and of "b^2"
                            indexX = rx * rSize + x;
                            indexY = ry * rSize + y;
                            sumOfRY += imgDataY[indexX, indexY];
                            sumOfRI += imgDataI[indexX, indexY];
                            sumOfRQ += imgDataQ[indexX, indexY];
                            sumOfRYSquared += imgDataY[indexX, indexY] * imgDataY[indexX, indexY];
                            sumOfRISquared += imgDataI[indexX, indexY] * imgDataI[indexX, indexY];
                            sumOfRQSquared += imgDataQ[indexX, indexY] * imgDataQ[indexX, indexY];
                        }
                    }

                    //For Each Di
                    for (dx = 0; dx < maxDxPos; dx++)
                    {
                        for (dy = 0; dy < maxDyPos; dy++)
                        {
                            tempSYDenominator = n * sumsOfDYSquared[dx, dy] - sumsOfDY[dx, dy] * sumsOfDY[dx, dy];
                            tempSIDenominator = n * sumsOfDISquared[dx, dy] - sumsOfDI[dx, dy] * sumsOfDI[dx, dy];
                            tempSQDenominator = n * sumsOfDQSquared[dx, dy] - sumsOfDQ[dx, dy] * sumsOfDQ[dx, dy];

                            //For Each possible transformations
                            for (i = 0; i < transformationsCount; i++)
                            {
                                sumOfDAndRY = 0;
                                sumOfDAndRI = 0;
                                sumOfDAndRQ = 0;
                                tempPixelY = tempDiY[dx, dy][i];
                                tempPixelI = tempDiI[dx, dy][i];
                                tempPixelQ = tempDiQ[dx, dy][i];

                                //Compute sums necessary for calculating RMS metric
                                for (x = 0; x < rSize; x++)
                                {
                                    indexX = rx * rSize + x;

                                    for (y = 0; y < rSize; y++)
                                    {
                                        //Calculate indexX and indexY
                                        indexY = ry * rSize + y;

                                        //Sum of "ri*di"
                                        sumOfDAndRY += tempPixelY[x, y] * imgDataY[indexX, indexY];
                                        sumOfDAndRI += tempPixelI[x, y] * imgDataI[indexX, indexY];
                                        sumOfDAndRQ += tempPixelQ[x, y] * imgDataQ[indexX, indexY];
                                    }
                                }
                                
                                //Compute contrast, brightness and RMS metric
                                sY = CalculateS(tempSYDenominator, n, sumOfDAndRY, sumsOfDY[dx, dy], sumOfRY, out sYInt);
                                sI = CalculateS(tempSIDenominator, n, sumOfDAndRI, sumsOfDI[dx, dy], sumOfRI, out sIInt);
                                sQ = CalculateS(tempSQDenominator, n, sumOfDAndRQ, sumsOfDQ[dx, dy], sumOfRQ, out sQInt);

                                oY = CalculateO(n, sY, sumOfRY, sumsOfDY[dx, dy], Y_LEVELS, out oYInt);
                                oI = CalculateO(n, sI, sumOfRI, sumsOfDI[dx, dy], I_LEVELS, out oIInt);
                                oQ = CalculateO(n, sQ, sumOfRQ, sumsOfDQ[dx, dy], Q_LEVELS, out oQInt);

                                tempRmsY = Math.Sqrt((sumOfRYSquared + sY * (sY * sumsOfDYSquared[dx, dy] - 2 * sumOfDAndRY + 2 * oY * sumsOfDY[dx, dy]) + oY * (n * oY - 2 * sumOfRY)) / n);
                                tempRmsI = Math.Sqrt((sumOfRISquared + sI * (sI * sumsOfDISquared[dx, dy] - 2 * sumOfDAndRI + 2 * oI * sumsOfDI[dx, dy]) + oI * (n * oI - 2 * sumOfRI)) / n);
                                tempRmsQ = Math.Sqrt((sumOfRQSquared + sQ * (sQ * sumsOfDQSquared[dx, dy] - 2 * sumOfDAndRQ + 2 * oQ * sumsOfDQ[dx, dy]) + oQ * (n * oQ - 2 * sumOfRQ)) / n);

                                if (Double.IsNaN(tempRmsY))
                                    throw new Exception("SQRT IS NAN!");
                                if (Double.IsNaN(tempRmsI))
                                    throw new Exception("SQRT IS NAN!");
                                if (Double.IsNaN(tempRmsQ))
                                    throw new Exception("SQRT IS NAN!");

                                //For each component, keep best transformation
                                if (!Double.IsNaN(tempRmsY) && finalRmsY > tempRmsY)
                                {
                                    finalRmsY = tempRmsY;
                                    bestRmsYIndexX = dx;
                                    bestRmsYIndexY = dy;
                                    bestRmsYTransformationIndex = i;
                                    bestSY = sYInt;
                                    bestOY = oYInt;
                                }

                                if (!Double.IsNaN(tempRmsI) && finalRmsI > tempRmsI)
                                {
                                    finalRmsI = tempRmsI;
                                    bestRmsIIndexX = dx;
                                    bestRmsIIndexY = dy;
                                    bestRmsITransformationIndex = i;
                                    bestSI = sIInt;
                                    bestOI = oIInt;
                                }

                                if (!Double.IsNaN(tempRmsQ) && finalRmsQ > tempRmsQ)
                                {
                                    finalRmsQ = tempRmsQ;
                                    bestRmsQIndexX = dx;
                                    bestRmsQIndexY = dy;
                                    bestRmsQTransformationIndex = i;
                                    bestSQ = sQInt;
                                    bestOQ = oQInt;
                                }
                            }
                        }
                    }

                    //Save Di retained for Ri
                    transformations[ry * rXCount + rx, 0] = new FractalCompressionResult.FractalTransformation(bestRmsYIndexX, bestRmsYIndexY, Convert.ToByte(bestOY), Convert.ToByte(bestSY), Convert.ToByte(bestRmsYTransformationIndex));
                    transformations[ry * rXCount + rx, 1] = new FractalCompressionResult.FractalTransformation(bestRmsIIndexX, bestRmsIIndexY, Convert.ToByte(bestOI), Convert.ToByte(bestSI), Convert.ToByte(bestRmsITransformationIndex));
                    transformations[ry * rXCount + rx, 2] = new FractalCompressionResult.FractalTransformation(bestRmsQIndexX, bestRmsQIndexY, Convert.ToByte(bestOQ), Convert.ToByte(bestSQ), Convert.ToByte(bestRmsQTransformationIndex));
                });
            }
            
            return new FractalCompressionResult(transformations, Convert.ToByte(Math.Ceiling(Math.Log(Math.Max(maxDxPos, maxDyPos), 2))), Convert.ToInt16(rXCount));
        }

        /// <summary>
        /// Uncompress image compressed using fractal compression (single iteration of decompression).
        /// </summary>
        /// <param name="compressionResult">Compressed image result.</param>
        /// <param name="decodingImage">Image used for decoding.</param>
        /// <returns>Uncompressed image.</returns>
        public CustomImage Uncompress(FractalCompressionResult compressionResult, CustomImage decodingImage)
        {
            int rXCount = compressionResult.RCountX;
            int rYCount = compressionResult.Transformations.GetLength(0) / compressionResult.RCountX;
            int imgSizeX = rXCount * _options.RSquareSize;
            int imgSizeY = rYCount * _options.RSquareSize;
            Pixel[,] pixels = new Pixel[imgSizeX, imgSizeY];
            int rSize = _options.RSquareSize;

            const int dimensions = 3;

            byte dSquareSize = Convert.ToByte(_options.RSquareSize * _options.DSquareRatio);
            double drRatio = (double)_options.RSquareSize / (double)dSquareSize;
            int drInverseRatio = _options.DSquareRatio;
            int maxDxPos = imgSizeX - dSquareSize + 1;
            int maxDyPos = imgSizeY - dSquareSize + 1;
            int dSize = dSquareSize;
            var imgData = decodingImage.ImageData;
            int pixelAverageSquareSize = drInverseRatio * drInverseRatio;
            var translateMatrixes = GetTranslateMatrixes(Convert.ToSByte(dSize));

            Parallel.For(0, rXCount, rx =>
            {
                int dimIndex, indexX, indexY, newX, newY, i, j, y;
                sbyte[,] tempTransform;
                sbyte[] tempTranslate;
                double[,,] tempComponents = new double[dSize, dSize, dimensions];
                double s, o;
                FractalCompressionResult.FractalTransformation transformation;
                double componentSumY;
                double componentSumI;
                double componentSumQ;

                for (int ry = 0; ry < rYCount; ry++)
                {
                    //Get transformed Di (Ri)
                    for (indexX = 0; indexX < dSize; indexX++)
                    {
                        for (indexY = 0; indexY < dSize; indexY++)
                        {
                            for (dimIndex = 0; dimIndex < dimensions; dimIndex++)
                            {
                                transformation = compressionResult.Transformations[ry * rXCount + rx, dimIndex];
                                tempTransform = _transformMatrixes[transformation.TransformationType];
                                tempTranslate = translateMatrixes[transformation.TransformationType];

                                newX = Convert.ToInt32(tempTransform[0, 0] * indexX + tempTransform[0, 1] * indexY + tempTranslate[0]);
                                newY = Convert.ToInt32(tempTransform[1, 0] * indexX + tempTransform[1, 1] * indexY + tempTranslate[1]);
                                s = transformation.ContrastTransformation / (double)(S_BITS_VALUE) * (2d * S_MAX) - S_MAX;

                                if (dimIndex == 0)
                                {
                                    o = (transformation.BrightnessTransformation / (double)(O_BITS_VALUE - 1) * ((1d + Math.Abs(s)) * Y_LEVELS));

                                    if (s > 0d)
                                        o -= s * Y_LEVELS;

                                    tempComponents[newX, newY, dimIndex] = imgData[transformation.DiX + indexX, transformation.DiY + indexY].Y * s + o;
                                }
                                else if (dimIndex == 1)
                                {
                                    o = (transformation.BrightnessTransformation / (double)(O_BITS_VALUE - 1) * ((1d + Math.Abs(s)) * I_LEVELS));

                                    if (s > 0d)
                                        o -= s * I_LEVELS;

                                    tempComponents[newX, newY, dimIndex] = imgData[transformation.DiX + indexX, transformation.DiY + indexY].I * s + o;
                                }
                                else
                                {
                                    o = (transformation.BrightnessTransformation / (double)(O_BITS_VALUE - 1) * ((1d + Math.Abs(s)) * Q_LEVELS));

                                    if (s > 0d)
                                        o -= s * Q_LEVELS;

                                    tempComponents[newX, newY, dimIndex] = imgData[transformation.DiX + indexX, transformation.DiY + indexY].Q * s + o;
                                }
                            }
                        }
                    }

                    //Populate pixels from Ri calculated
                    for (indexX = 0; indexX < dSize; indexX += drInverseRatio)
                    {
                        for (indexY = 0; indexY < dSize; indexY += drInverseRatio)
                        {
                            componentSumY = 0;
                            componentSumI = 0;
                            componentSumQ = 0;

                            for (i = 0; i < drInverseRatio; i++)
                            {
                                for (j = 0; j < drInverseRatio; j++)
                                {
                                    componentSumY += tempComponents[indexX + i, indexY + j, 0];
                                    componentSumI += tempComponents[indexX + i, indexY + j, 1];
                                    componentSumQ += tempComponents[indexX + i, indexY + j, 2];
                                }
                            }

                            pixels[rx * rSize + indexX / drInverseRatio, ry * rSize + indexY / drInverseRatio] = Pixel.FromYIQ(
                                Convert.ToByte(Math.Max(Math.Min(componentSumY / (drInverseRatio * drInverseRatio), 255), 0)),
                                Convert.ToInt16(Math.Max(Math.Min(componentSumI / (drInverseRatio * drInverseRatio), 151), -151)),
                                Convert.ToInt16(Math.Max(Math.Min(componentSumQ / (drInverseRatio * drInverseRatio), 133), -133)));
                        }
                    }
                }
            });

            if (_options.SmoothImage)
                pixels = SmoothImage(pixels, rSize);

            return new CustomImage(pixels);
        }

        /// <summary>
        /// Uncompress image using 10 iterations.
        /// </summary>
        /// <param name="compressionResult">Compressed image result.</param>
        /// <param name="decodingImage">Image used for decoding.</param>
        /// <returns>Custom Image.</returns>
        public CustomImage CompleteUncompress(FractalCompressionResult compressionResult, CustomImage decodingImage)
        {
            using (new CustomTimer("Complete decompression"))
            {
                for (int i = 0; i < 10; i++)
                    decodingImage = Uncompress(compressionResult, decodingImage);
            }

            //if (_options.SmoothImage)
            //    return new CustomImage(SmoothImage(decodingImage.ImageData, _options.RSquareSize));
            //else
                return decodingImage;
        }

        /// <summary>
        /// Smooth image Rx.
        /// </summary>
        /// <param name="pixels">Pixels array.</param>
        /// <param name="rSize">RSize</param>
        /// <returns>Smoothened pixels.</returns>
        public Pixel[,] SmoothImage(Pixel[,] pixels, int rSize)
        {
            int i, j, x,
                w1 = 2, w2 = 1,
                width = pixels.GetLength(0),
                height = pixels.GetLength(1);

            byte yComponent1, yComponent2;
            short iComponent1, iComponent2, qComponent1, qComponent2;

            for (i = 0; i < width - rSize; i += rSize)
            {
                for (j = 0; j < height - rSize; j += rSize)
                {
                    //Smooth horizontal line
                    for (x = 0; x < rSize; x++)
                    {
                        yComponent1 = pixels[i + x, j + rSize].Y;
                        iComponent1 = pixels[i + x, j + rSize].I;
                        qComponent1 = pixels[i + x, j + rSize].Q;
                        yComponent2 = pixels[i + x, j + rSize - 1].Y;
                        iComponent2 = pixels[i + x, j + rSize - 1].I;
                        qComponent2 = pixels[i + x, j + rSize - 1].Q;

                        pixels[i + x, j + rSize] = Pixel.FromYIQ(
                            Convert.ToByte((w1 * yComponent1 + w2 * yComponent2) / (w1 + w2)),
                            Convert.ToInt16((w1 * iComponent1 + w2 * iComponent2) / (w1 + w2)),
                            Convert.ToInt16((w1 * qComponent1 + w2 * qComponent2) / (w1 + w2))
                        );
                        pixels[i + x, j + rSize - 1] = Pixel.FromYIQ(
                            Convert.ToByte((w2 * yComponent1 + w1 * yComponent2) / (w1 + w2)),
                            Convert.ToInt16((w2 * iComponent1 + w1 * iComponent2) / (w1 + w2)),
                            Convert.ToInt16((w2 * qComponent1 + w1 * qComponent2) / (w1 + w2))
                        );
                    }

                    //Smooth vertical line
                    for (x = 0; x < rSize; x++)
                    {
                        yComponent1 = pixels[i + rSize, j + x].Y;
                        iComponent1 = pixels[i + rSize, j + x].I;
                        qComponent1 = pixels[i + rSize, j + x].Q;
                        yComponent2 = pixels[i + rSize - 1, j + x].Y;
                        iComponent2 = pixels[i + rSize - 1, j + x].I;
                        qComponent2 = pixels[i + rSize - 1, j + x].Q;

                        pixels[i + rSize, j + x] = Pixel.FromYIQ(
                            Convert.ToByte((w1 * yComponent1 + w2 * yComponent2) / (w1 + w2)),
                            Convert.ToInt16((w1 * iComponent1 + w2 * iComponent2) / (w1 + w2)),
                            Convert.ToInt16((w1 * qComponent1 + w2 * qComponent2) / (w1 + w2))
                        );
                        pixels[i + rSize - 1, j + x] = Pixel.FromYIQ(
                            Convert.ToByte((w2 * yComponent1 + w1 * yComponent2) / (w1 + w2)),
                            Convert.ToInt16((w2 * iComponent1 + w1 * iComponent2) / (w1 + w2)),
                            Convert.ToInt16((w2 * qComponent1 + w1 * qComponent2) / (w1 + w2))
                        );
                    }
                }
            }

            return pixels;
        }

        /// <summary>
        /// Get translate matrixes according to size of R.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private sbyte[][] GetTranslateMatrixes(sbyte size)
        {
            size = Convert.ToSByte(size - 1);

            return new sbyte[][]
            {
                new sbyte[] { 0, 0 },
                new sbyte[] { size, 0},
                new sbyte[] { 0, size},
                new sbyte[] { 0, 0 },
                new sbyte[] { size, size },
                new sbyte[] { size, size },
                new sbyte[] { size, 0 },
                new sbyte[] { 0, size },
            };
        }

        /// <summary>
        /// Calculate S parameter for decompression.
        /// </summary>
        /// <param name="denominator">Denominator in S calculation</param>
        /// <param name="n">Number of pixels in region.</param>
        /// <param name="sumOfDAndR">Sum of components of D and R.</param>
        /// <param name="sumOfD">Sum of components of D</param>
        /// <param name="sumOfR">Sum of components of R</param>
        /// <param name="sInt">Value of S discretized.</param>
        /// <returns>Value of S</returns>
        private double CalculateS(double denominator, int n, int sumOfDAndR, int sumOfD, int sumOfR, out int sInt)
        {
            double s;

            if (denominator != 0)
                s = (n * sumOfDAndR - sumOfD * sumOfR) / denominator;
            else
                s = 0d;

            if (s < 0d)
                s = 0d;

            sInt = (int)(0.5 + (s + S_MAX) / (2d * S_MAX) * S_BITS_VALUE);
            if (sInt < 0)
                sInt = 0;
            if (sInt >= S_BITS_VALUE)
                sInt = S_BITS_VALUE - 1;

            return (double)sInt / (double)(S_BITS_VALUE) * (2d * S_MAX) - S_MAX;
        }

        /// <summary>
        /// Calculate O parameter for decompression.
        /// </summary>
        /// <param name="n">Number of pixels in region.</param>
        /// <param name="s">Value of s.</param>
        /// <param name="sumOfR">Sum of components of R</param>
        /// <param name="sumOfD">Sum of components of D</param>
        /// <param name="levels">Levels used for component.</param>
        /// <returns>Value of S</returns>
        private double CalculateO(int n, double s, int sumOfR, int sumOfD, int levels, out int oInt)
        {
            double o = (sumOfR - s * sumOfD) / n;

            if (s > 0d)
                o += s * levels;

            oInt = (int)(0.5 + o / ((1d + Math.Abs(s)) * levels) * (O_BITS_VALUE - 1));

            if (oInt < 0)
                oInt = 0;
            if (oInt >= O_BITS_VALUE)
                oInt = O_BITS_VALUE - 1;

            o = (double)oInt / (double)(O_BITS_VALUE - 1) * ((1d + Math.Abs(s)) * levels);

            if (s > 0d)
                o -= s * levels;

            return o;
        }
    }
}
