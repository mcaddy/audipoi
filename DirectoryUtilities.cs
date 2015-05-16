//-----------------------------------------------------------------------
// <copyright file="DirectoryUtilities.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy
{
    using System.IO;

    /// <summary>
    /// Directory Utilities Class
    /// </summary>
    public sealed class DirectoryUtilities
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="DirectoryUtilities"/> class from being created
        /// </summary>
        private DirectoryUtilities()
        {
        }

        /// <summary>
        /// Get the size of a directory and all subfolders.
        /// </summary>
        /// <param name="folderPath">Path of folder</param>
        /// <returns>Size of the folder</returns>
        public static long GetDirectorySize(string folderPath)
        {
            string[] a = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            long b = 0;
            foreach (string name in a)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }

            return b;
        }
    }
}
