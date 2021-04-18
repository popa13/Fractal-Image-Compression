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
    /// Base class for reading images from disk.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ImageReader<T>
    {
        public ImageReader()
        {

        }

        /// <summary>
        /// Read an image from disk.
        /// </summary>
        /// <param name="fileName">Filename of the image to read.</param>
        /// <returns>Image read.</returns>
        public T Read(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            return GetImageFromBytes(bytes);
        }

        /// <summary>
        /// Get Image from bytes of the file.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected abstract T GetImageFromBytes(byte[] bytes);
    }
}
