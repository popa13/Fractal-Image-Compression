using ImageTools.Utils;
using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Compression.Fractal
{
    public class FractalCompression
    {
        private readonly FractalCompressionOptions _options;

        private readonly double[][,] _transformMatrixes = new double[][,]
        {
            new double[,] { { 1, 0 }, { 0, 1 } }, //Identity
            new double[,] { { -1, 0 }, { 0, 1 } }, //Reflection on Y axis
            new double[,] { { 1, 0 }, { 0, -1 } }, //Reflection on X axis
            new double[,] { { 0, 1 }, { 1, 0 } }, //Reflection on 45 degree line
            new double[,] { { 0, -1 }, { -1, 0 } }, //Reflection on -45 degree line
            new double[,] { { -1, 0 }, { 0, -1 } }, //180 degrees rotation
            new double[,] { { 0, -1 }, { 1, 0 } }, //90 degrees rotation
            new double[,] { { 0, 1 }, { -1, 0 } }, //270 degrees rotation
        };

        /// <summary>
        /// Constructor of a Fractal Image Compression.
        /// </summary>
        /// <param name="options">Options to use in the algorithm</param>
        public FractalCompression(FractalCompressionOptions options)
        {
            _options = options;
        }

        public FractalCompressionResult Compress(CustomImage image)
        {
            //Validate the image and the params
            if (image.Width % _options.RSquareSize > 0 || image.Height % _options.RSquareSize > 0 || _options.DSquareSize % _options.RSquareSize > 0)
                throw new InvalidOperationException("The compression options specified can't compress this image as the R squares size is not a divisor of height and width. Ri Sizes must be divisor of Di sizes too.");

            object finalResultLock = new object();
            int rXCount = image.Width / _options.RSquareSize; //Number of R Squares in X
            int rYCount = image.Height / _options.RSquareSize; //Number of R Squares in Y
            int totalRCount = rXCount * rYCount;
            int maxDxPos = image.Width - _options.DSquareSize + 1; // POP: Ici, le coin supérieur droit du carré ne se rend pas au coin supérieur droit de l'image
            int maxDyPos = image.Height - _options.DSquareSize + 1; // POP: même commentaire mais avec le coin inférieur gauche
            byte dSize = _options.DSquareSize;
            byte rSize = _options.RSquareSize;
            int n = _options.RSquareSize * _options.RSquareSize;
            double drRatio = (double)_options.RSquareSize / (double)_options.DSquareSize;
            int drInverseRatio = _options.DSquareSize / _options.RSquareSize;
            double pixelAverageSquareSize = (double)drInverseRatio * (double)drInverseRatio;
            var translateMatrixes = GetTranslateMatrixes(_options.RSquareSize);
            int transformationsCount = _transformMatrixes.Length;
            var transformations = new FractalCompressionResult.FractalTransformation[totalRCount, 3]; //Transformations (2 dimension to keep transformations for each ri in each color component)
            int rx;
            double finalRmsY = Double.MaxValue, finalRmsI = Double.MaxValue, finalRmsQ = Double.MaxValue,
                bestBrightnessY = 0d, bestContrastY = 0d,
                bestBrightnessI = 0d, bestContrastI = 0d,
                bestBrightnessQ = 0d, bestContrastQ = 0d;
            var rawImgData = image.ImageData;
            var imgDataY = new double[image.Width, image.Height];
            var imgDataI = new double[image.Width, image.Height];
            var imgDataQ = new double[image.Width, image.Height];
            double[,][][,] tempDiY = new double[maxDxPos, maxDyPos][][,];
            double[,][][,] tempDiI = new double[maxDxPos, maxDyPos][][,];
            double[,][][,] tempDiQ = new double[maxDxPos, maxDyPos][][,];
            double[,] sumsOfAY = new double[maxDxPos, maxDyPos];
            double[,] sumsOfAYSquared = new double[maxDxPos, maxDyPos];
            double[,] sumsOfAI = new double[maxDxPos, maxDyPos];
            double[,] sumsOfAISquared = new double[maxDxPos, maxDyPos];
            double[,] sumsOfAQ = new double[maxDxPos, maxDyPos];
            double[,] sumsOfAQSquared = new double[maxDxPos, maxDyPos];

            //Fill imgData Y, I and Q
            Parallel.For(0, image.Width, dx =>
            {
                for (int dy = 0; dy < image.Height; dy++)
                {
                    imgDataY[dx, dy] = rawImgData[dx, dy].YNormalized;
                    imgDataI[dx, dy] = rawImgData[dx, dy].IDownSampledAndNormalized;
                    imgDataQ[dx, dy] = rawImgData[dx, dy].QDownSampledAndNormalized;
                }
            });

            //Average pixels of each Di to size of Ri
            Parallel.For(0, maxDxPos, dx =>
            {
                int i, x, y, j, k, newX, newY, indexX = 0, indexY = 0;
                double[,] tempTransformMatrix;
                double[] tempTranslateMatrix;
                double[,] tempPixelY1 = null;
                double[,] tempPixelI1 = null;
                double[,] tempPixelQ1 = null;
                double tempSumY = 0d, tempSumI = 0d, tempSumQ = 0d,
                    tempAveragePixelY = 0d, tempAveragePixelI = 0d, tempAveragePixelQ = 0d;

                for (int dy = 0; dy < maxDyPos; dy++)
                {
                    tempDiY[dx, dy] = new double[transformationsCount][,];
                    tempDiI[dx, dy] = new double[transformationsCount][,];
                    tempDiQ[dx, dy] = new double[transformationsCount][,];

                    for (i = 0; i < transformationsCount; i++)
                    {
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
                                    tempPixelY1 = new double[rSize, rSize];
                                    tempPixelI1 = new double[rSize, rSize];
                                    tempPixelQ1 = new double[rSize, rSize];

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
                                    tempAveragePixelY = tempSumY > 0 ? tempSumY / pixelAverageSquareSize : 0d;
                                    tempAveragePixelI = tempSumI > 0 ? tempSumI / pixelAverageSquareSize : 0d;
                                    tempAveragePixelQ = tempSumQ > 0 ? tempSumQ / pixelAverageSquareSize : 0d;

                                    //Sum of "a" and "a^2" for future RMS calculation
                                    sumsOfAY[dx, dy] += tempAveragePixelY;
                                    sumsOfAI[dx, dy] += tempAveragePixelI;
                                    sumsOfAQ[dx, dy] += tempAveragePixelQ;
                                    sumsOfAYSquared[dx, dy] += tempAveragePixelY * tempAveragePixelY;
                                    sumsOfAISquared[dx, dy] += tempAveragePixelI * tempAveragePixelI;
                                    sumsOfAQSquared[dx, dy] += tempAveragePixelQ * tempAveragePixelQ;
                                }

                                newX = (byte)(tempTransformMatrix[0, 0] * (x / drInverseRatio) + tempTransformMatrix[0, 1] * (y / drInverseRatio) + tempTranslateMatrix[0]);
                                newY = (byte)(tempTransformMatrix[1, 0] * (x / drInverseRatio) + tempTransformMatrix[1, 1] * (y / drInverseRatio) + tempTranslateMatrix[1]);

                                indexX = (int)(newX * drRatio);
                                indexY = (int)(newY * drRatio);

                                //Store pixel components for transformed Di (Fi)
                                tempPixelY1[indexX, indexY] = tempAveragePixelY;
                                tempPixelI1[indexX, indexY] = tempAveragePixelI;
                                tempPixelQ1[indexX, indexY] = tempAveragePixelQ;

                                //Store pixels of Di
                                tempDiY[dx, dy][i] = tempPixelY1;
                                tempDiI[dx, dy][i] = tempPixelI1;
                                tempDiQ[dx, dy][i] = tempPixelQ1;
                            }
                        }
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
                    double sumOfBY = 0d;
                    double sumOfBYSquared = 0d;
                    double sumOfBI = 0d;
                    double sumOfBISquared = 0d;
                    double sumOfBQ = 0d;
                    double sumOfBQSquared = 0d;
                    double sumOfAAndBY;
                    double sumOfAAndBI;
                    double sumOfAAndBQ;
                    int x, y, indexX, indexY, dx, dy, i,
                        bestRmsYIndexX = -1, bestRmsYIndexY = -1, bestRmsYTransformationIndex = -1,
                        bestRmsIIndexX = -1, bestRmsIIndexY = -1, bestRmsITransformationIndex = -1,
                        bestRmsQIndexX = -1, bestRmsQIndexY = -1, bestRmsQTransformationIndex = -1;
                    double[,] tempPixelY = null;
                    double[,] tempPixelI = null;
                    double[,] tempPixelQ = null;
                    double tempContrastY, tempBrightnessY,
                        tempContrastI, tempBrightnessI,
                        tempContrastQ, tempBrightnessQ,
                        tempRmsY, tempRmsI, tempRmsQ;

                    for (x = 0; x < rSize; x++)
                    {
                        for (y = 0; y < rSize; y++)
                        {
                            //Sum of "b" and of "b^2"
                            indexX = rx * rSize + x;
                            indexY = ry * rSize + y;
                            sumOfBY += imgDataY[indexX, indexY];
                            sumOfBI += imgDataI[indexX, indexY];
                            sumOfBQ += imgDataQ[indexX, indexY];
                            sumOfBYSquared += imgDataY[indexX, indexY] * imgDataY[indexX, indexY];
                            sumOfBISquared += imgDataI[indexX, indexY] * imgDataI[indexX, indexY];
                            sumOfBQSquared += imgDataQ[indexX, indexY] * imgDataQ[indexX, indexY];
                        }
                    }

                    //For Each Di
                    using (var timer = new CustomTimer("Transformation"))
                    for (dx = 0; dx < maxDxPos; dx++)
                    {
                        for (dy = 0; dy < maxDyPos; dy++)
                        {
                            //For Each possible transformations
                            for (i = 0; i < transformationsCount; i++)
                            {
                                sumOfAAndBY = 0d;
                                sumOfAAndBI = 0d;
                                sumOfAAndBQ = 0d;
                                tempPixelY = tempDiY[dx, dy][i];
                                tempPixelI = tempDiI[dx, dy][i];
                                tempPixelQ = tempDiQ[dx, dy][i];

                                //Compute sums necessary for calculating RMS metric
                                for (x = 0; x < rSize; x++)
                                {
                                    for (y = 0; y < rSize; y++)
                                    {
                                        //Calculate indexX and indexY
                                        indexX = dx + x;
                                        indexY = dy + y;

                                        //Sum of "a*b"
                                        sumOfAAndBY += tempPixelY[x, y] * imgDataY[indexX, indexY];
                                        sumOfAAndBI += tempPixelI[x, y] * imgDataI[indexX, indexY];
                                        sumOfAAndBQ += tempPixelQ[x, y] * imgDataQ[indexX, indexY];
                                    }
                                }

                                //Compute contrast, brightness and RMS metric
                                tempContrastY = (n * sumOfAAndBY - sumsOfAY[dx, dy] * sumOfBY) / (n * sumsOfAYSquared[dx, dy] - Math.Pow(sumsOfAY[dx, dy], 2));
                                tempContrastI = (n * sumOfAAndBI - sumsOfAI[dx, dy] * sumOfBI) / (n * sumsOfAISquared[dx, dy] - Math.Pow(sumsOfAI[dx, dy], 2));
                                tempContrastQ = (n * sumOfAAndBQ - sumsOfAQ[dx, dy] * sumOfBQ) / (n * sumsOfAQSquared[dx, dy] - Math.Pow(sumsOfAQ[dx, dy], 2));

                                tempBrightnessY = (1d / n) * (sumOfBY - tempContrastY * sumsOfAY[dx, dy]);
                                tempBrightnessI = (1d / n) * (sumOfBI - tempContrastI * sumsOfAI[dx, dy]);
                                tempBrightnessQ = (1d / n) * (sumOfBQ - tempContrastQ * sumsOfAQ[dx, dy]);

                                tempRmsY = (1d / n) * (sumOfBYSquared + tempContrastY * (tempContrastY * sumsOfAYSquared[dx, dy] - 2 * sumOfAAndBY + 2 * tempBrightnessY * sumsOfAY[dx, dy]) + tempBrightnessY * (n * tempBrightnessY - 2 * sumOfBY));
                                tempRmsI = (1d / n) * (sumOfBISquared + tempContrastI * (tempContrastI * sumsOfAISquared[dx, dy] - 2 * sumOfAAndBI + 2 * tempBrightnessI * sumsOfAI[dx, dy]) + tempBrightnessI * (n * tempBrightnessI - 2 * sumOfBI));
                                tempRmsQ = (1d / n) * (sumOfBQSquared + tempContrastQ * (tempContrastQ * sumsOfAQSquared[dx, dy] - 2 * sumOfAAndBQ + 2 * tempBrightnessQ * sumsOfAQ[dx, dy]) + tempBrightnessQ * (n * tempBrightnessQ - 2 * sumOfBQ));

                                lock (finalResultLock)
                                {
                                    //For each component, keep best transformation
                                    if (finalRmsY > tempRmsY)
                                    {
                                        finalRmsY = tempRmsY;
                                        bestRmsYIndexX = dx;
                                        bestRmsYIndexY = dy;
                                        bestRmsYTransformationIndex = i;
                                        bestContrastY = tempContrastY;
                                        bestBrightnessY = tempBrightnessY;
                                    }

                                    if (finalRmsI > tempRmsI)
                                    {
                                        finalRmsI = tempRmsI;
                                        bestRmsIIndexX = dx;
                                        bestRmsIIndexY = dy;
                                        bestRmsITransformationIndex = i;
                                        bestContrastI = tempContrastI;
                                        bestBrightnessI = tempBrightnessI;
                                    }

                                    if (finalRmsQ > tempRmsQ)
                                    {
                                        finalRmsQ = tempRmsQ;
                                        bestRmsQIndexX = dx;
                                        bestRmsQIndexY = dy;
                                        bestRmsQTransformationIndex = i;
                                        bestContrastQ = tempContrastQ;
                                        bestBrightnessQ = tempBrightnessQ;
                                    }
                                }
                            }
                        }
                    }

                    //Save Di retained for Ri
                    lock (finalResultLock)
                    {
                        transformations[ry * rXCount + rx, 0] = new FractalCompressionResult.FractalTransformation(bestRmsYIndexX, bestRmsYIndexY, (byte)bestBrightnessY, (byte)bestContrastY, (byte)bestRmsYTransformationIndex);
                        transformations[ry * rXCount + rx, 1] = new FractalCompressionResult.FractalTransformation(bestRmsIIndexX, bestRmsIIndexY, (byte)bestBrightnessI, (byte)bestContrastI, (byte)bestRmsITransformationIndex);
                        transformations[ry * rXCount + rx, 2] = new FractalCompressionResult.FractalTransformation(bestRmsQIndexX, bestRmsQIndexY, (byte)bestBrightnessQ, (byte)bestContrastQ, (byte)bestRmsQTransformationIndex);
                    }
                });
            }
            
            return new FractalCompressionResult(transformations, (byte)Math.Ceiling(Math.Log(Math.Max(maxDxPos, maxDyPos), 2)), (short)rXCount);
        }

        public CustomImage Uncompress(FractalCompressionResult compressionResult, CustomImage decodingImage)
        {
            const int dimensions = 3;
            int rXCount = compressionResult.RCountX;
            int rYCount = compressionResult.Transformations.GetLength(0) / compressionResult.RCountX;
            int imgSizeX = rXCount * _options.RSquareSize;
            int imgSizeY = rYCount * _options.RSquareSize;

            //if (decodingImage.Width != imgSizeX || decodingImage.Height != imgSizeY)
            //    throw new ArgumentException("Decoding image must be the same size that the compressed image!");

            Pixel[,] pixels = new Pixel[imgSizeX, imgSizeY];
            double drRatio = (double)_options.RSquareSize / (double)_options.DSquareSize;
            int drInverseRatio = _options.DSquareSize / _options.RSquareSize;
            int dSizeXAdapted = decodingImage.Width / (rXCount / drInverseRatio);
            int dSizeYAdapted = decodingImage.Height / (rYCount / drInverseRatio);
            double drRatioAdapted = (double)_options.RSquareSize / dSizeXAdapted;
            int drInverseRatioAdapted = dSizeXAdapted / _options.RSquareSize;
            int maxDxPos = imgSizeX - _options.DSquareSize + 1;
            int maxDyPos = imgSizeY - _options.DSquareSize + 1;
            int dSize = _options.DSquareSize;
            int rSize = _options.RSquareSize;
            double[,][][,] tempDi = new double[maxDxPos, maxDyPos][][,];
            double[,,] imgData = new double[imgSizeX, imgSizeY, dimensions];
            var rawImgData = decodingImage.ImageData;
            double pixelAverageSquareSize = (double)drInverseRatioAdapted * (double)drInverseRatioAdapted;
            var translateMatrixes = GetTranslateMatrixes(_options.RSquareSize);

            //Fill imgData Y, I and Q
            Parallel.For(0, imgSizeX, dx =>
            {
                for (int dy = 0; dy < imgSizeY; dy++)
                {
                    imgData[dx, dy, 0] = rawImgData[dx, dy].YNormalized;
                    imgData[dx, dy, 1] = rawImgData[dx, dy].IDownSampledAndNormalized;
                    imgData[dx, dy, 2] = rawImgData[dx, dy].QDownSampledAndNormalized;
                }
            });

            //Subsample image to fit image to get Dis
            Parallel.For(0, maxDxPos, dx =>
            {
                int x, y, j, k, indexX = 0, indexY = 0;
                double[,] tempPixelY1 = null;
                double[,] tempPixelI1 = null;
                double[,] tempPixelQ1 = null;
                double tempSumY = 0d, tempSumI = 0d, tempSumQ = 0d,
                    tempAveragePixelY = 0d, tempAveragePixelI = 0d, tempAveragePixelQ = 0d;

                for (int dy = 0; dy < maxDyPos; dy++)
                {
                    tempDi[dx, dy] = new double[dimensions][,];
                    
                    for (x = 0; x < dSize; x += drInverseRatioAdapted) //Increment of inverse ratio to get only parts of Di
                    {
                        for (y = 0; y < dSize; y += drInverseRatioAdapted)
                        {
                            //Reset temp variables
                            tempSumY = 0;
                            tempSumI = 0;
                            tempSumQ = 0;
                            tempPixelY1 = new double[rSize, rSize];
                            tempPixelI1 = new double[rSize, rSize];
                            tempPixelQ1 = new double[rSize, rSize];

                            //Average pixels
                            for (j = 0; j < drInverseRatioAdapted; j++)
                            {
                                for (k = 0; k < drInverseRatioAdapted; k++)
                                {
                                    tempSumY += imgData[dx + x + j, dy + y + k, 0];
                                    tempSumI += imgData[dx + x + j, dy + y + k, 1];
                                    tempSumQ += imgData[dx + x + j, dy + y + k, 2];
                                }
                            }

                            //Calculating average Pixel Components
                            tempAveragePixelY = tempSumY > 0 ? tempSumY / pixelAverageSquareSize : 0d;
                            tempAveragePixelI = tempSumI > 0 ? tempSumI / pixelAverageSquareSize : 0d;
                            tempAveragePixelQ = tempSumQ > 0 ? tempSumQ / pixelAverageSquareSize : 0d;

                            indexX = (int)(y * drRatio);
                            indexY = (int)(x * drRatio);

                            ////Store pixel components for transformed Di (Fi)
                            tempPixelY1[indexX, indexY] = tempAveragePixelY;
                            tempPixelI1[indexX, indexY] = tempAveragePixelI;
                            tempPixelQ1[indexX, indexY] = tempAveragePixelQ;

                            ////Store pixels of Di
                            tempDi[dx, dy][0] = tempPixelY1;
                            tempDi[dx, dy][1] = tempPixelI1;
                            tempDi[dx, dy][2] = tempPixelQ1;
                        }
                    }
                }
            });

            //Uncompress image
            Parallel.For(0, rXCount, rx =>
            {
                int dimIndex, indexX, indexY, newX, newY;
                double[,] tempTransform;
                double[] tempTranslate;
                byte[,,] tempComponents = new byte[rSize, rSize, dimensions];
                FractalCompressionResult.FractalTransformation transformation;

                for (int ry = 0; ry < rYCount; ry++)
                {
                    //Get transformed Di (Ri)
                    for (indexX = 0; indexX < rSize; indexX++)
                    {
                        for (indexY = 0; indexY < rSize; indexY++)
                        {
                            for (dimIndex = 0; dimIndex < dimensions; dimIndex++)
                            {
                                transformation = compressionResult.Transformations[ry * rXCount + rx, dimIndex];
                                tempTransform = _transformMatrixes[transformation.TransformationType];
                                tempTranslate = translateMatrixes[transformation.TransformationType];

                                newX = (byte)(tempTransform[0, 0] * indexX + tempTransform[0, 1] * indexY + tempTranslate[0]);
                                newY = (byte)(tempTransform[1, 0] * indexX + tempTransform[1, 1] * indexY + tempTranslate[1]);

                                tempComponents[newX, newY, dimIndex] = (byte)(tempDi[transformation.DiX, transformation.DiY][dimIndex][indexX, indexY] * transformation.ContrastTransformation + transformation.BrightnessTransformation);
                            }
                        }
                    }

                    //Populate pixels from Ri calculated
                    for (indexX = 0; indexX < rSize; indexX++)
                    {
                        for (indexY = 0; indexY < rSize; indexY++)
                        {
                            pixels[rx * rSize + indexX, ry * rSize + indexY] = Pixel.FromYIQDownSampled(tempComponents[indexX, indexY, 0], tempComponents[indexX, indexY, 1], tempComponents[indexX, indexY, 2]);
                        }
                    }
                }
            });

            return new CustomImage(pixels);
        }

        private double[][] GetTranslateMatrixes(int rSize)
        {
            return new double[][]
            {
                new double[] { 0, 0 },
                new double[] { rSize, 0},
                new double[] { 0, rSize },
                new double[] { 0, 0 },
                new double[] { rSize, rSize },
                new double[] { rSize, rSize },
                new double[] { rSize, 0 },
                new double[] { 0, rSize },
            };
        }
    }
}
