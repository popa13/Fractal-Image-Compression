using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.ImageWriters
{
    /// <summary>
    /// Default Image reader. Reads standard image formats.
    /// </summary>
    public class DefaultImageWriter : ImageWriter<Bitmap>
    {
        /// <summary>
        /// Get the bytes of the bitmap.
        /// </summary>
        /// <param name="image">Bitmap to save</param>
        /// <returns>Array of bytes.</returns>
        protected override byte[] GetBytes(Bitmap image)
        {
            byte[] byteArray = new byte[0];

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return byteArray;
        }
    }
}
