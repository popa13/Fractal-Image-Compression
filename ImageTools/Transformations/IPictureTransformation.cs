using MathTools.Matrixes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Transformations
{
    /// <summary>
    /// Define this interface to modify pixels of an image.
    /// </summary>
    public interface IPictureTransformation
    {
        Matrix<Pixel> Transform(Matrix<Pixel> pixels, bool moveOnExistingImage = false);
    }
}
