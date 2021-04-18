using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools
{
    /// <summary>
    /// Class representing a Pixel through its different components.
    /// </summary>
    public class Pixel
    {
        public const int DOWN_SAMPLE_RATIO = 4; //Down Sample Ratio for I and Q

        /// <summary>
        /// Matrix for converting RGB to Y (Luminance), I (Hue), Q (Saturation)
        /// </summary>
        private static DoubleMatrix _yiqConversionMatrix = new double[,]
        {
            { 0.299, 0.587, 0.114 },
            { 0.596, -0.274, -0.322 },
            { 0.211, -0.523, 0.312 }
        };

        /// <summary>
        /// Matrix for converting YIQ back to RGB.
        /// </summary>
        private static DoubleMatrix _yiqInverseConversionMatrix = new double[,]
        {
            { 1d, 0.956, 0.621 },
            { 1d, -0.272, -0.647 },
            { 1d, -1.106, 1.703 }
        };

        /// <summary>
        /// Red component of Pixel.
        /// </summary>
        public byte Red { get; set; }
        /// <summary>
        /// Green component of Pixel.
        /// </summary>
        public byte Green { get; set; }
        /// <summary>
        /// Blue component of Pixel.
        /// </summary>
        public byte Blue { get; set; }

        //Cache of YIQ components
        private byte? _y;
        private short? _i;
        private short? _q;

        /// <summary>
        /// Get Y Component (Luminance)
        /// Range: [0, 255]
        /// </summary>
        public byte Y
        {
            get
            {
                if (_y == null)
                    _y = Convert.ToByte(_yiqConversionMatrix[0, 0] * Red + _yiqConversionMatrix[0, 1] * Green + _yiqConversionMatrix[0, 2] * Blue);
                return _y.Value;
            }
        }

        /// <summary>
        /// Get I Component (Hue)
        /// Range: [-151, 151]
        /// </summary>
        public short I
        {
            get
            {
                if (_i == null)
                    _i = Convert.ToInt16(_yiqConversionMatrix[1, 0] * Red + _yiqConversionMatrix[1, 1] * Green + _yiqConversionMatrix[1, 2] * Blue);
                return _i.Value;
            }
        }

        /// <summary>
        /// Get Q Component (Saturation)
        /// Range: [-133, 133]
        /// </summary>
        public short Q
        {
            get
            {
                if (_q == null)
                    _q = Convert.ToInt16(_yiqConversionMatrix[2, 0] * Red + _yiqConversionMatrix[2, 1] * Green + _yiqConversionMatrix[2, 2] * Blue);
                return _q.Value;
            }
        }

        /// <summary>
        /// Get I Component down-sampled.
        /// </summary>
        public double IDownSampled
        {
            get
            {
                if (I > 0)
                    return I / DOWN_SAMPLE_RATIO;
                return 0;
            }
        }

        /// <summary>
        /// Get Q Component down-sampled.
        /// </summary>
        public double QDownSampled
        {
            get
            {
                if (Q > 0)
                    return Q / DOWN_SAMPLE_RATIO;
                return 0;
            }
        }

        /// <summary>
        /// Get Red component normalized on the interval [0, 1].
        /// </summary>
        public double RedNormalized
        {
            get
            {
                if (Red > 0)
                    return Red / 255d;
                return 0d;
            }
        }
        /// <summary>
        /// Get Green component normalized on the interval [0, 1].
        /// </summary>
        public double GreenNormalized
        {
            get
            {
                if (Green > 0)
                    return Green / 255d;
                return 0d;
            }
        }
        /// <summary>
        /// Get Blue component normalized on the interval [0, 1].
        /// </summary>
        public double BlueNormalized
        {
            get
            {
                if (Blue > 0)
                    return Blue / 255d;
                return 0d;
            }
        }

        /// <summary>
        /// Obtain the minimum Component of the pixel (RGB).
        /// </summary>
        public byte Minimum
        {
            get { return Math.Min(Red, Math.Min(Green, Blue)); }
        }

        /// <summary>
        /// Obtain the maximum Component of the pixel (RGB).
        /// </summary>
        public byte Maximum
        {
            get { return Math.Max(Red, Math.Max(Green, Blue)); }
        }

        /// <summary>
        /// Constructor of a pixel
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        public Pixel(byte r, byte g, byte b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        /// <summary>
        /// Create a pixel from YIQ components.
        /// </summary>
        /// <param name="y">Y component</param>
        /// <param name="i">I component</param>
        /// <param name="q">Q component</param>
        /// <returns>Pixel</returns>
        public static Pixel FromYIQ(byte y, short i, short q)
        {
            return new Pixel(
                    Convert.ToByte(Math.Min(Math.Max(_yiqInverseConversionMatrix[0, 0] * y + _yiqInverseConversionMatrix[0, 1] * i + _yiqInverseConversionMatrix[0, 2] * q, 0), 255)),
                    Convert.ToByte(Math.Min(Math.Max(_yiqInverseConversionMatrix[1, 0] * y + _yiqInverseConversionMatrix[1, 1] * i + _yiqInverseConversionMatrix[1, 2] * q, 0), 255)),
                    Convert.ToByte(Math.Min(Math.Max(_yiqInverseConversionMatrix[2, 0] * y + _yiqInverseConversionMatrix[2, 1] * i + _yiqInverseConversionMatrix[2, 2] * q, 0), 255))
                );
        }

        /// <summary>
        /// Overrides the ToString() representation of a pixel to make debugging easier.
        /// </summary>
        /// <returns>String representation of a pixel.</returns>
        public override string ToString()
        {
            return $"R:{Red};G:{Green};B:{Blue}";
        }
    }
}
