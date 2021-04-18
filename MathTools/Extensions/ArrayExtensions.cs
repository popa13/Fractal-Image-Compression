using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathTools.Extensions
{
    /// <summary>
    /// Classe simplifiant le travail avec les tableaux.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Fonction permettant de mapper un tableau à 2 dimensions à l'aide d'une fonction lambda.
        /// 
        /// Exemple d'utilisation pour doubler les éléments du tableau
        /// array.Select(element => element * 2);
        /// </summary>
        /// <typeparam name="T">Générique (type des éléments du tableau)</typeparam>
        /// <param name="array">Tableau sur lequel on itère</param>
        /// <param name="mapper">Fonction de mapping pour les éléments du tableau</param>
        /// <returns>Nouveau tableau de mêmes dimensions que "array", contenant les éléments transformées</returns>
        public static T[,] Select<T>(this T[,] array, Func<T, T> mapper)
        {
            return array.Select((el, x, y) => mapper(el));
        }

        /// <summary>
        /// Fonction permettant de mapper un tableau à 2 dimensions à l'aide d'une fonction lambda, 
        /// en ayant l'index en x et en y comme informations passées à notre fonction lambda.
        /// 
        /// Exemple d'utilisation donnant la valeur de la cellule divisée par la taille de l'array:
        /// array.Select((element, x, y) => element / (x * y));
        /// </summary>
        /// <typeparam name="T">Générique (type des éléments du tableau)</typeparam>
        /// <param name="array">Tableau sur lequel on itère</param>
        /// <param name="mapper">Fonction de mapping pour les éléments du tableau</param>
        /// <returns>Nouveau tableau de mêmes dimensions que "array", contenant les éléments transformées</returns>
        public static T[,] Select<T>(this T[,] array, Func<T, int, int, T> mapper)
        {
            int xSize = array.GetLength(0);
            int ySize = array.GetLength(1);
            T[,] result = new T[xSize, ySize];

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    result[x, y] = mapper(array[x, y], x, y);
                }
            }

            return result;
        }
    }
}
