using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.ImageWriters
{
    /// <summary>
    /// Base class for saving images on disk using different formats.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ImageWriter<T>
    {
        public ImageWriter()
        {

        }

        /// <summary>
        /// Save the image on disk.
        /// </summary>
        /// <param name="fileName">Filename of the image.</param>
        /// <param name="image">Image to save.</param>
        public void Write(string fileName, T image)
        {
            byte[] bytes = GetBytes(image);
            File.WriteAllBytes(fileName, bytes);
        }

        /// <summary>
        /// Get the size in bytes of the file according to the image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public long GetFileSize(T image)
        {
            return GetBytes(image).Length;
        }

        /// <summary>
        /// Get the bytes of the image in order to save them.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <returns>Bytes to save on disk.</returns>
        protected abstract byte[] GetBytes(T image);
    }
}
