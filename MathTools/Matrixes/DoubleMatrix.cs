using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathTools.Matrixes
{
    public class DoubleMatrix : Matrix<double>
    {
        /// <summary>
        /// Constructeur d'une matrice.
        /// Crée une matrice de dimension désirée, avec ses valeurs initialisées à zéro.
        /// </summary>
        /// <param name="dimensionX">Nombre de colonnes de la matrice</param>
        /// <param name="dimensionY">Nombre de lignes de la matrice</param>
        public DoubleMatrix(int dimensionX, int dimensionY) : this(new double[dimensionX, dimensionY])
        {

        }

        /// <summary>
        /// Constructeur d'une matrice.
        /// Instancie une matrice avec les valeurs et les dimensions spécifiées par le tableau 2D passé en paramètre.
        /// </summary>
        /// <param name="values">Tableau à 2 dimensions de la matrice</param>
        public DoubleMatrix(double[,] values) : base(values)
        {

        }

        public DoubleMatrix(Matrix<double> matrix) : this(matrix.Elements)
        {

        }

        /// <summary>
        /// Opérateur d'addition, permet d'additionner 2 matrices de même dimension
        /// </summary>
        /// <param name="a">Matrice A</param>
        /// <param name="b">Matrice B</param>
        /// <returns>Matrice résultante de l'addition de A et B</returns>
        public static DoubleMatrix operator +(DoubleMatrix a, DoubleMatrix b)
        {
            if (a.DimensionX != b.DimensionX || a.DimensionY != b.DimensionY)
                throw new ArgumentException("Matrixes added together must have the same dimensions!");

            return new DoubleMatrix(a.Select((element, x, y) => element + b[x, y]));
        }

        /// <summary>
        /// Opérateur de soustraction, permet de soustraire 2 matrices de même dimension
        /// </summary>
        /// <param name="a">Matrice A</param>
        /// <param name="b">Matrice B</param>
        /// <returns>Matrice résultante de la soustraction de A et B</returns>
        public static DoubleMatrix operator -(DoubleMatrix a, DoubleMatrix b)
        {
            if (a.DimensionX != b.DimensionX || a.DimensionY != b.DimensionY)
                throw new ArgumentException("Matrixes substracted together must have the same dimensions!");

            return new DoubleMatrix(a.Select((element, x, y) => element - b[x, y]));
        }

        /// <summary>
        /// Change le signe des éléments d'une matrice
        /// </summary>
        /// <param name="matrix">Matrice dont on inverse le signe des valeurs</param>
        /// <returns>Matrice avec les signes opposés.</returns>
        public static DoubleMatrix operator -(DoubleMatrix matrix)
        {
            return new DoubleMatrix(matrix.Select(element => -element));
        }

        /// <summary>
        /// Multiplication matricielle de 2 matrices.
        /// </summary>
        /// <param name="a">Matrice A</param>
        /// <param name="b">Matrice B</param>
        /// <returns>Matrice résultante de la multiplication de A et B</returns>
        public static DoubleMatrix operator *(DoubleMatrix a, DoubleMatrix b)
        {
            if (a.DimensionY != b.DimensionX)
                throw new ArgumentException("Matrixes multiplied together must have compatible dimensions!");

            double[,] result = new double[a.DimensionX, b.DimensionY];

            for (int x = 0; x < a.DimensionX; x++)
            {
                for (int y = 0; y < b.DimensionY; y++)
                {
                    for (int k = 0; k < a.DimensionY; k++)
                        result[x, y] += a[x, k] * b[k, y];
                }
            }

            return new DoubleMatrix(result);
        }

        /// <summary>
        /// Multiplication d'une matrice ar un scalaire
        /// </summary>
        /// <param name="scalar">Scalaire multipliant la matrice</param>
        /// <param name="matrix">Matrice multipliée par un scalaire</param>
        /// <returns>Matrice résultante de la multiplication de scalar * matrix</returns>
        public static DoubleMatrix operator *(double scalar, DoubleMatrix matrix)
        {
            return new DoubleMatrix(matrix.Select(x => x * scalar));
        }
        public static DoubleMatrix operator *(DoubleMatrix matrix, double scalar)
        {
            return scalar * matrix;
        }

        /// <summary>
        /// Opérateur de casting implicite d'un tableau 2D à sa matrice correspondante.
        /// </summary>
        /// <param name="array">Tableau à convertir</param>
        public static implicit operator DoubleMatrix(double[,] array)
        {
            return new DoubleMatrix(array);
        }
    }
}
