using ImageTools;
using ImageTools.Compression.Fractal;
using ImageTools.ImageReaders;
using ImageTools.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCompressionTool.Controllers
{
    /// <summary>
    /// Singleton.
    /// Main Controller class used for interactions with classes.
    /// </summary>
    public class MainController
    {
        private static MainController _instance = null; //Singleton instance

        /// <summary>
        /// Get Singleton Instance.
        /// </summary>
        public static MainController Instance
        {
            get { return _instance == null ? _instance = new MainController() : _instance; }
        }

        /// <summary>
        /// Private Constructor.
        /// </summary>
        private MainController()
        {

        }

        /// <summary>
        /// Compress image using fractal compression and using specified options.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public FractalCompressionResult CompressUsingFractalCompression(CustomImage image, FractalCompressionOptions options)
        {
            using (var timer = new CustomTimer("Fractal Compression"))
            {
                FractalCompression compression = new FractalCompression(options);
                return compression.Compress(image);
            }
        }

        /// <summary>
        /// Open a Fractal Compressed file and return the result and the decoding image.
        /// </summary>
        /// <param name="fileName">FileName of the fractal compressed image</param>
        /// <returns>Tuple containing the compression result, the image used to uncompress the image and the decoding image name</returns>
        public Tuple<FractalCompressionResult, Bitmap, string> OpenFractalCompressedFileAndGetDecodingImage(string fileName)
        {
            //Open image to decode this image
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "jpg";
            ofd.Filter = "Images (*.png)|*.png|Images (*.jpg)|*.jpg";
            ofd.Title = "Choisissez une image de décodage";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FractalImageReader reader = new FractalImageReader();
                var result = reader.Read(fileName);
                return new Tuple<FractalCompressionResult, Bitmap, string>(result, new Bitmap(ofd.FileName), ofd.FileName);
            }
            else
            {
                MessageBox.Show("Une image de décodage doit être choisie afin de d'ouvrir l'image encodée par fractales!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }
    }
}
