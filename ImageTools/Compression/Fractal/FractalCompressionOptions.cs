using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Compression.Fractal
{
    /// <summary>
    /// Class containing the options of the Fractal Image Compression.
    /// </summary>
    public class FractalCompressionOptions
    {
        /// <summary>
        /// Size X by X of the R squares
        /// </summary>
        public byte RSquareSize { get; set; } = 8;
        /// <summary>
        /// Size X by X of the D squares (usually 2 * RSquareSize)
        /// </summary>
        public byte DSquareRatio { get; set; } = 2;
        /// <summary>
        /// If we smooth the image or not.
        /// </summary>
        public bool SmoothImage { get; set; } = true;
    }
}
