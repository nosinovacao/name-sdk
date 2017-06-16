using System;
using System.Reflection;
using System.Security.Cryptography;

namespace NAME.DigestHelpers
{

    /// <summary>
    /// Static digest helper methods
    /// </summary>
    public static class DigestHelper
    {
        private static readonly HashAlgorithm Algorithm;
        private static string assemblyQualifiedName = typeof(System.Security.Cryptography.MD5).GetTypeInfo().Assembly.FullName;


        static DigestHelper()
        {
            Algorithm = MD5.Create();
        }

        /// <summary>
        /// Gets the digest for a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The MD5 digest for this message</returns>
        public static string GetDigestForMessage(string message) => GetDigestForMessage(Algorithm, message);

        /// <summary>
        /// Gets the digest for a message using a specific algorithm.
        /// </summary>
        /// <param name="algorithm">The algorithm to use.</param>
        /// <param name="message">The message.</param>
        /// <returns>The digest of the message using the specified algorithm</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">algorithmType - Algorithm is not supported</exception>
        /// <exception cref="System.InvalidOperationException">type does not have the Create method</exception>
        public static string GetDigestForMessage(string algorithm, string message)
        {
            var algorithmType = Type.GetType($"System.Security.Cryptography.{algorithm.ToUpper()}, {assemblyQualifiedName}", false);
            if (algorithmType == null)
            {
                throw new ArgumentOutOfRangeException(nameof(algorithmType), "Algorithm is not supported");
            }

            var currentAlgorithm =
                algorithmType.GetTypeInfo()
                    .GetMethod("Create", Type.EmptyTypes)
                    .Invoke(null, null) as HashAlgorithm;

            if (currentAlgorithm == null)
            {
                throw new InvalidOperationException(
                    $"Invocation of Create method of type {algorithmType.FullName} cannot be casted to {typeof(HashAlgorithm).FullName}");
            }

            return GetDigestForMessage(currentAlgorithm, message);
        }

        /// <summary>
        /// Gets the digest for a message using a specific algorithm.
        /// </summary>
        /// <param name="algorithm">The algorithm to use.</param>
        /// <param name="message">The message.</param>
        /// <returns>The digest of the message using the specified algorithm</returns>
        public static string GetDigestForMessage(HashAlgorithm algorithm, string message)
        {
            var msgBytes = System.Text.Encoding.UTF8.GetBytes(message);
            var digest = algorithm.ComputeHash(msgBytes);
            var stringBuilder = new System.Text.StringBuilder();
            foreach (var b in digest)
                stringBuilder.AppendFormat("{0:x2}", b);
            return stringBuilder.ToString();
        }
    }

}