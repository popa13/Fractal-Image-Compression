using MathTools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathTools.Matrixes
{
    public class Matrix<T>
    {
        public T[,] Elements; //Éléments de la matrice

        /// <summary>
        /// Dimension en X de la matrice
        /// </summary>
        public int DimensionX { get; private set; }
        /// <summary>
        /// Dimension en Y de la matrice
        /// </summary>
        public int DimensionY { get; private set; }

        /// <summary>
        /// Obtient la taille de la matrice (x * y)
        /// </summary>
        public int Size => DimensionX * DimensionY;

        /// <summary>
        /// Permet d'obtenir ou de mettre en place un élément dans la matrice.
        /// </summary>
        /// <param name="x">Indice x</param>
        /// <param name="y">Indice y</param>
        /// <returns>Élément à la position [x, y]</returns>
        public T this[int x, int y]
        {
            get { return Elements[x, y]; }
            set { Elements[x, y] = value; }
        }

        /// <summary>
        /// Constructeur d'une matrice.
        /// Crée une matrice de dimension désirée, avec ses valeurs initialisées à zéro.
        /// </summary>
        /// <param name="dimensionX">Nombre de colonnes de la matrice</param>
        /// <param name="dimensionY">Nombre de lignes de la matrice</param>
        public Matrix(int dimensionX, int dimensionY) :
            this(new T[dimensionX, dimensionY])
        {

        }

        /// <summary>
        /// Constructeur d'une matrice.
        /// Instancie une matrice avec les valeurs et les dimensions spécifiées par le tableau 2D passé en paramètre.
        /// </summary>
        /// <param name="values">Tableau à 2 dimensions de la matrice</param>
        public Matrix(T[,] values)
        {
            this.DimensionX = values.GetLength(0);
            this.DimensionY = values.GetLength(1);
            Elements = (T[,])values.Clone();
        }

        /// <summary>
        /// Fonction permettant de mapper les éléments d'une matrice dans une nouvelle matrice.
        /// 
        /// Exemple d'utilisation donnant la valeur de la cellule multipliée par 2:
        /// array.Select(element => element * 2);
        /// </summary>
        /// <param name="mapper">Fonction de mapping pour les éléments du tableau</param>
        /// <returns>Nouvelle matrice de mêmes dimensions que la matrice actuelle</returns>
        public Matrix<T> Select(Func<T, T> mapper)
        {
            return new Matrix<T>(Elements.Select(mapper));
        }

        /// <summary>
        /// Fonction permettant de mapper les éléments d'une matrice dans une nouvelle matrice.
        /// 
        /// Exemple d'utilisation donnant la valeur de la cellule divisée par la taille de la matrice:
        /// array.Select((element, x, y) => element / (x * y));
        /// </summary>
        /// <param name="mapper">Fonction de mapping pour les éléments du tableau</param>
        /// <returns>Nouvelle matrice de mêmes dimensions que la matrice actuelle</returns>
        public Matrix<T> Select(Func<T, int, int, T> mapper)
        {
            return new Matrix<T>(Elements.Select(mapper));
        }

        /// <summary>
        /// Opérateur de casting implicite d'une matrice à son tableau à 2 dimensions correspondant.
        /// </summary>
        /// <param name="matrix">Matrice à caster</param>
        public static implicit operator T[,](Matrix<T> matrix)
        {
            T[,] copy = new T[matrix.DimensionX, matrix.DimensionY];
            Array.Copy(matrix.Elements, copy, matrix.Size);
            return copy;
        }

        /// <summary>
        /// Opérateur de casting implicite d'un tableau 2D à sa matrice correspondante.
        /// </summary>
        /// <param name="array"></param>
        public static implicit operator Matrix<T>(T[,] array)
        {
            return new Matrix<T>(array);
        }
    }
}
