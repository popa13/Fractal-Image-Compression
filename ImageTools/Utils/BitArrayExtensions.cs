using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTools.Utils
{
    /// <summary>
    /// Extension methods for BitArray class, making working with them easier (for saving and reading raw files).
    /// </summary>
    public static class BitArrayExtensions
    {
        /// <summary>
        /// Mask for extracting last bit of a serie of bits.
        /// </summary>
        private static readonly uint MASK = 0x1;

        /// <summary>
        /// Set specified bit count in BitArray at specified index, using last bits of specified int value.
        /// </summary>
        /// <param name="array">BitArray to edit</param>
        /// <param name="index">index of bit array to start inserting</param>
        /// <param name="bitsCount">number of bits to edit</param>
        /// <param name="value">Value used to edit bits</param>
        public static void SetBitsAtIndexFromNLastBits(this BitArray array, int index, int bitsCount, int value)
        {
            for (int i = index + bitsCount - 1; i >= index ; i--)
            {
                array.Set(i, (value & MASK) > 0);
                value >>= 1;
            }
        }

        /// <summary>
        /// Get specified bit count in BitArray at specified index.
        /// </summary>
        /// <param name="array">BitArray to edit</param>
        /// <param name="index">index of bit array to start inserting</param>
        /// <param name="bitsCount">number of bits to edit</param>
        /// <returns>Bits read in a uint</returns>
        public static uint GetBitsAtIndexFromNLastBits(this BitArray array, int index, int bitsCount)
        {
            uint value = 0;

            for (int i = index; i < index + bitsCount; i++)
            {
                value <<= 1;

                if (array.Get(i))
                    value |= MASK;
            }

            return value;
        }
    }
}
