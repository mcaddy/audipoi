//-----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds a Range of items to a Collection
        /// </summary>
        /// <typeparam name="T">This objects type</typeparam>
        /// <param name="destination">this object</param>
        /// <param name="source">the other collection</param>
        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            List<T> list = destination as List<T>;

            if (list != null)
            {
                list.AddRange(source);
            }
            else
            {
                foreach (T item in source)
                {
                    destination.Add(item);
                }
            }
        }

        /// <summary>
        /// Split a Byte array into chunks
        /// </summary>
        /// <param name="value">input byte array</param>
        /// <param name="bufferLength">chunk size</param>
        /// <returns>a collection of byte arrays</returns>
        public static IEnumerable<byte[]> Split(this byte[] value, int bufferLength)
        {
            int countOfArray = value.Length / bufferLength;
            if (value.Length % bufferLength > 0)
            {
                countOfArray++;
            }

            for (int i = 0; i < countOfArray; i++)
            {
                yield return value.Skip(i * bufferLength).Take(bufferLength).ToArray();
            }
        }
    }
}
