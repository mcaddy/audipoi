//-----------------------------------------------------------------------
// <copyright file="SecureStrings.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------

namespace Mcaddy
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Secure Strings class
    /// </summary>
    public static class SecureStrings
    {
        /// <summary>
        /// Some Entropy
        /// </summary>
        private static byte[] entropy = Encoding.Unicode.GetBytes("SomeEntropy");

        /// <summary>
        /// Encrypt a secure string
        /// </summary>
        /// <param name="input">The secure string</param>
        /// <returns>An encrypted string</returns>
        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypt a secure string
        /// </summary>
        /// <param name="encryptedData">the encrypted string</param>
        /// <returns>A secure string</returns>
        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    DataProtectionScope.CurrentUser);
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a string to a secure string
        /// </summary>
        /// <param name="input">a string</param>
        /// <returns>A secure string</returns>
        public static SecureString ToSecureString(string input)
        {
            using (SecureString secure = new SecureString())
            {
                foreach (char c in input)
                {
                    secure.AppendChar(c);
                }

                secure.MakeReadOnly();

                return secure;
            }
        }

        /// <summary>
        /// Converts a SecureString to a string
        /// </summary>
        /// <param name="input">The SecureString</param>
        /// <returns>A string</returns>
        public static string ToInsecureString(SecureString input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            string returnValue = string.Empty;
            IntPtr ptr = Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }

            return returnValue;
        }
    }
}
