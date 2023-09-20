// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

#pragma warning disable IDE0005
using System;
using System.Security.Cryptography;
using System.Text;

#pragma warning restore IDE0005

namespace Gems.Utils
{
    public static class CryptoUtils
    {
        public static string ConvertToBase64EncryptedWithRsa512(string publicKey, string data)
        {
            return EncryptRsa512(publicKey, Encoding.UTF8.GetBytes(data));
        }

        public static string ConvertFromBase64EncryptedWithRsa512(string privateKey, string encryptedDataAsBase64)
        {
            return Encoding.UTF8.GetString(DecryptRsa512(privateKey, encryptedDataAsBase64));
        }

        public static string EncryptRsa512(string publicKey, byte[] dataBytes)
        {
            using var rsa = new RSACryptoServiceProvider(512);
            try
            {
                rsa.ImportFromPem(publicKey.ToCharArray());

                const int segmentSize = 53;
                const int encryptedSegmentSize = 64;
                var lastSegmentSize = dataBytes.Length % segmentSize;
                var segmentsCount = ((dataBytes.Length - lastSegmentSize) / segmentSize) + 1;
                var segment = new byte[segmentSize];
                var segmentOffset = 0;
                var encryptedSegmentOffset = 0;
                var segmentSizeForCopy = segmentSize;
                var encryptedSegment = new byte[encryptedSegmentSize];
                var encryptedData = new byte[encryptedSegmentSize * segmentsCount];
                for (var i = 0; i < segmentsCount; i++)
                {
                    segmentOffset = i * segmentSize;
                    if (i == segmentsCount - 1)
                    {
                        segment = new byte[segmentSize];
                        segmentSizeForCopy = lastSegmentSize;
                    }

                    Array.Copy(dataBytes, segmentOffset, segment, 0, segmentSizeForCopy);
                    encryptedSegment = rsa.Encrypt(segment, false);
                    encryptedSegmentOffset = i * encryptedSegmentSize;
                    Array.Copy(encryptedSegment, 0, encryptedData, encryptedSegmentOffset, encryptedSegmentSize);
                }

                var base64Encrypted = Convert.ToBase64String(encryptedData);
                return base64Encrypted;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        public static byte[] DecryptRsa512(string privateKey, string encryptedDataAsBase64)
        {
            using var rsa = new RSACryptoServiceProvider(512);
            try
            {
                rsa.ImportFromPem(privateKey.ToCharArray());
                var encryptedBytes = Convert.FromBase64String(encryptedDataAsBase64);
                const int encryptedSegmentSize = 64;
                const int decryptedSegmentSize = 53;
                var segmentsCount = encryptedBytes.Length / encryptedSegmentSize;
                var encryptedSegment = new byte[encryptedSegmentSize];
                var encryptedSegmentOffset = 0;
                var decryptedSegmentOffset = 0;
                var decryptedSegment = new byte[decryptedSegmentSize];
                var decryptedBytes = new byte[decryptedSegmentSize * segmentsCount];
                for (var i = 0; i < segmentsCount; i++)
                {
                    encryptedSegmentOffset = i * encryptedSegmentSize;
                    Array.Copy(encryptedBytes, encryptedSegmentOffset, encryptedSegment, 0, encryptedSegmentSize);
                    decryptedSegment = rsa.Decrypt(encryptedSegment, false);
                    decryptedSegmentOffset = i * decryptedSegmentSize;
                    Array.Copy(decryptedSegment, 0, decryptedBytes, decryptedSegmentOffset, decryptedSegmentSize);
                }

                return decryptedBytes;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }
}
