# Fractal-Image-Compression

To store images on a computer, there are many compression scheme that do exist to accomplish this task. The main players in this industry are JPEG, JPEG 200, SVG and PNG. However, there is a fifth one which is interesting because it uses fractals to compress images. 

In this repository, you will find a pdf document and program which implements the Jacquin's method to compress images with fractals. It is not the most efficient, but it is the easiest to implement in a classroom project (there is a implementations of fractal compression scheme called FIF for Fractal Image Format). For more details on these methods, you can go to the [wikipedia page](https://en.wikipedia.org/wiki/Fractal_compression) or read the pdf file rapport-final.pdf in Documents et présentation of this repository.

To use the program, please refer to the guide_utilisateur_final.pdf in Document et présentation.

## Details of the program
The software is divided into 3 distinct projects: MathTools and ImageTools are two library-like projects that allow the reuse of code relevant to be reused, and ImageCompressionTool which provides the graphical interface to work with the fractal compression. 

### Description of MathTools
The MathTools project contains an ArrayExtensions class containing a method extension allowing to select and if necessary modify the elements of a multiple array dimensions which is used in the 2 classes of matrices. A base class of Matrix <T> is generic and therefore makes it possible to store objects of various types in a 2-dimensional matrix and provides easy access to these, since the use of multidimensional arrays is sometimes complex in C #. The DoubleMatrix class inherits from this class and implements the behaviors of a matrix for real types (double). It allows several basic matrix operations, such as addition, subtraction, multiplication by a vector, and matrix multiplication. 
  
### Description of ImageTools
The ImageTools project uses the MathTools library since several calculations are made with matrices. This is the project with the most code of the three. First of all, this library provides an ImageWriter <T> class which allows you to store
simple images and get their size on disk before saving them. A Image-Reader class <T> is also provided in order to read images from the disc. Two concrete implementations of each of these classes are made: one to work with JPEG and PNG  images using the implementation provided by the .NET Framework and one to work with the custom fractal compression format. 
  
Then, a Pixel class makes it possible to represent a pixel in an image according to its RGB notation (also provides the possibility of obtaining the YIQ components of the pixel). The CustomImage class allows to simplify the work with the images by allowing to transform a Bitmap object provided by the .NET Framework in an array of Pixel-type objects, or to generate a image from an array of pixels.

Also, the IPictureTransformation interface provides a simple interface for processing pixels of an image. A concrete implementation of this interface makes it possible to transform an image into grayscale (GreyscaleTransformation) while another allows to transform the image according to an affine transformation (AffineTransformation).

Several utility classes are also provided in this library. The BitArrayExtensions class allows you to simplify the writing and reading of bits in a BitArray object. This class is used for writing and reading the compressed image file with the methodology fractal. The CustomTimer class is used to calculate the code execution time, in order to calculate
compression and decompression time. Then, the ImageUtils class is used to find the image of the differences between an image A and an image B in addition to calculating the MSE metrics and PSNR for the Y, I and Q components between 2 images, in order to know if they are similar or not.

Finally, the FractalCompressionOptions class is used to define the options used during the fractal compression, i.e.: the size of the squares Ri, the size of the squares Dk and whether the image is softened or not. The FractalCompression class is the important class of the software, since it is there that we perform the fractal compression and decompression. The Compress method returns an object of type FractalCompressionResult, which contains the relevant compression information about to store the image on disk. The image will be saved and read using the FractalImageWriter and FractalImageReader classes respectively. 

### Description of ImageCompressionTool
The ImageCompressionTool project is the one that provides the graphical interface to the user to perform fractal compression. The ImageComparisonControl class defines a user control. It can be used to display and compare 2 images. It is also used in all windows of the application. The MainForm class defines the behavior of the main window. The FractalCompressionForm class defines the behavior of the window for fractal compression and decompression and
this inherits from BaseCompressionForm, which is an abstract class that would have been used to simplify the development of several different compression algorithms. However, we have only implemented the fractal compression, so this class is somewhat unnecessary. The mageDifferenceForm class is used to display information on the difference between 2 images, as well as the associated image of differences. Finally, the MainController class is a singleton allowing complex interactions with software classes.
