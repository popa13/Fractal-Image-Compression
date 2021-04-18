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
    public class MainFormController
    {
        private static MainFormController _instance = null;

        public static MainFormController Instance
        {
            get { return _instance == null ? _instance = new MainFormController() : _instance; }
        }

        private MainFormController()
        {

        }

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
        /// <returns>Tuple containing the compression result and the image used to uncompress the image</returns>
        public Tuple<FractalCompressionResult, Bitmap> OpenFractalCompressedFileAndGetDecodingImage(string fileName)
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
                return new Tuple<FractalCompressionResult, Bitmap>(result, new Bitmap(ofd.FileName));
            }
            else
            {
                MessageBox.Show("Une image de décodage doit être choisie afin de d'ouvrir l'image encodée par fractals!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }
    }
}
