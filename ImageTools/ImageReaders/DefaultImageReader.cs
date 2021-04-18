using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.ImageReaders
{
    /// <summary>
    /// Default Image reader. Reads standard image formats.
    /// </summary>
    public class DefaultImageReader : ImageReader<Bitmap>
    {
        /// <summary>
        /// Get Bitmap from bytes.
        /// </summary>
        /// <param name="bytes">Bytes of the image.</param>
        /// <returns>Bitmap decoded from bytes.</returns>
        protected override Bitmap GetImageFromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return new Bitmap(stream);
        }
    }
}
