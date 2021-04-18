using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Compression.Fractal
{
    /// <summary>
    /// Represents the resul of a Fractal Image Compression
    /// </summary>
    public class FractalCompressionResult
    {
        /// <summary>
        /// Array containing the contractive transformations for the array.
        /// 2D because second dimension stock transformations for Y, I and Q components.
        /// </summary>
        public FractalTransformation[,] Transformations { get; private set; }
        /// <summary>
        /// Size of the transformations (Size of X and Y directions of Di)
        /// </summary>
        public byte TransformationTranslateSize { get; private set; }

        /// <summary>
        /// Number of Ri in a row. (Used to retrieve dimensions)
        /// </summary>
        public short RCountX { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transformations">Transformations done to the image</param>
        /// <param name="transformationTranslateSize">Size of the DiX and DiY parameters when saving</param>
        /// <param name="rCountX">Number of Ri in a row</param>
        public FractalCompressionResult(FractalTransformation[,] transformations, byte transformationTranslateSize, short rCountX)
        {
            this.Transformations = transformations;
            this.TransformationTranslateSize = transformationTranslateSize;
            this.RCountX = rCountX;
        }

        /// <summary>
        /// Class representing one transformation to do to get resulting image.
        /// </summary>
        public class FractalTransformation
        {
            /// <summary>
            /// Get DiX position
            /// </summary>
            public int DiX { get; private set; }
            /// <summary>
            /// Get DiY position
            /// </summary>
            public int DiY { get; private set; }
            /// <summary>
            /// Get Brightness Transformation (o)
            /// </summary>
            public byte BrightnessTransformation { get; private set; }
            /// <summary>
            /// Get Contrast Transformation (s)
            /// </summary>
            public byte ContrastTransformation { get; private set; }
            /// <summary>
            /// Type of transformation applied
            /// </summary>
            public byte TransformationType { get; private set; }

            /// <summary>
            /// Constructor of a transformation
            /// </summary>
            /// <param name="diX">DiX position</param>
            /// <param name="diY">DiY position</param>
            /// <param name="brightnessTransformation">Transformation done to brightness (o)</param>
            /// <param name="contrastTransformation">Transformation done to contrast (s)</param>
            /// <param name="transformationType">Type of the transformation applied</param>
            public FractalTransformation(int diX, int diY, byte brightnessTransformationQuantized, byte contrastTransformationQuantized, byte transformationType)
            {
                this.DiX = diX;
                this.DiY = diY;
                this.BrightnessTransformation = brightnessTransformationQuantized;
                this.ContrastTransformation = contrastTransformationQuantized;
                this.TransformationType = transformationType;
            }

            /// <summary>
            /// Overrides ToString representation for easier debugging.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"DiX: {DiX}, DiY: {DiY}, brightness: {BrightnessTransformation}, contrast: {ContrastTransformation}, type: {TransformationType}";
            }
        }
    }
}
