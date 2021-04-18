using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathTools.Matrixes;

namespace ImageTools.Transformations
{
    /// <summary>
    /// Transformation transforming a colored image into a black and white one.
    /// </summary>
    public class GreyscaleTransformation : IPictureTransformation
    {
        public Matrix<Pixel> Transform(Matrix<Pixel> pixels, bool moveOnExistingImage = false)
        {
            //Lightness version
            return pixels.Select(x =>
            {
                if (x != null)
                {
                    byte mean = Convert.ToByte((x.Minimum + x.Maximum) / 2);
                    return new Pixel(mean, mean, mean);
                }

                return null;
            });
        }
    }
}
