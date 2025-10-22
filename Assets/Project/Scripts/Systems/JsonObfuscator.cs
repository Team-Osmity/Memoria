using System;
using System.Security.Cryptography;
using System.Text;

namespace Memoria.Systems
{
    public static class JsonObfuscator
    {
        private const byte DefaultXorKey = 0x5A;
        private const int AesKeyBytes = 32;
        private const int AesIvBytes = 16;
        private const int SaltBytes = 16;
        private const int PBKDF2Iterations = 10000;

        /// <summary>
        /// JSONを難読化
        /// </summary>
        public static string Encode(string json, byte key = DefaultXorKey)
        {
            if (string.IsNullOrEmpty(json)) return string.Empty;
            var bytes = Encoding.UTF8.GetBytes(json);
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] ^= key;
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// JSONを復号化
        /// </summary>
        public static string Decode(string encoded, byte key = DefaultXorKey)
        {
            if (string.IsNullOrEmpty(encoded)) return string.Empty;
            byte[] bytes;
            try { bytes = Convert.FromBase64String(encoded); }
            catch { return string.Empty; }
            for (int i = 0; i < bytes.Length; i++) bytes[i] ^= key;
            return Encoding.UTF8.GetString(bytes);
        }

        public static string AesEncrypt(string plainText, string passphrase)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            if (passphrase == null) passphrase = string.Empty;

            var salt = new byte[SaltBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var key = DeriveKey(passphrase, salt, AesKeyBytes);

            using (var aes = Aes.Create())
            {
                aes.KeySize = AesKeyBytes * 8;
                aes.BlockSize = AesIvBytes * 8;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;

                aes.GenerateIV();
                var iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.Write(salt, 0, salt.Length);
                    ms.Write(iv, 0, iv.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new System.IO.StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plainText);
                    }

                    var result = ms.ToArray();
                    return Convert.ToBase64String(result);
                }
            }
        }

        public static string AesDecrypt(string base64, string passphrase)
        {
            if (string.IsNullOrEmpty(base64)) return string.Empty;
            if (passphrase == null) passphrase = string.Empty;

            byte[] raw;
            try { raw = Convert.FromBase64String(base64); }
            catch { return string.Empty; }

            if (raw.Length < SaltBytes + AesIvBytes) return string.Empty;

            var salt = new byte[SaltBytes];
            Buffer.BlockCopy(raw, 0, salt, 0, SaltBytes);
            var iv = new byte[AesIvBytes];
            Buffer.BlockCopy(raw, SaltBytes, iv, 0, AesIvBytes);

            var cipherOffset = SaltBytes + AesIvBytes;
            var cipherLen = raw.Length - cipherOffset;

            var key = DeriveKey(passphrase, salt, AesKeyBytes);

            using (var aes = Aes.Create())
            {
                aes.KeySize = AesKeyBytes * 8;
                aes.BlockSize = AesIvBytes * 8;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                using (var ms = new System.IO.MemoryStream(raw, cipherOffset, cipherLen))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new System.IO.StreamReader(cs, Encoding.UTF8))
                {
                    try
                    {
                        return sr.ReadToEnd();
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
        }

        public static string ComputeHmacBase64(string message, byte[] keyBytes)
        {
            var msgBytes = Encoding.UTF8.GetBytes(message ?? string.Empty);
            using (var h = new HMACSHA256(keyBytes))
            {
                var mac = h.ComputeHash(msgBytes);
                return Convert.ToBase64String(mac);
            }
        }

        public static bool VerifyHmac(string message, string base64Hmac, byte[] keyBytes)
        {
            if (string.IsNullOrEmpty(base64Hmac)) return false;
            var expected = ComputeHmacBase64(message, keyBytes);
            return SlowEquals(expected, base64Hmac);
        }

        public static bool IsBase64(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            s = s.Trim();
            if (s.Length % 4 != 0) return false;
            foreach (char c in s)
            {
                if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=')) return false;
            }
            return true;
        }

        public static bool TryDecodeAuto(string encoded, out string decoded, string passphrase = null)
        {
            decoded = null;
            if (string.IsNullOrEmpty(encoded)) return false;

            if (!string.IsNullOrEmpty(passphrase))
            {
                try
                {
                    var d = AesDecrypt(encoded, passphrase);
                    if (!string.IsNullOrEmpty(d))
                    {
                        decoded = d;
                        return true;
                    }
                }
                catch { /* ignore */ }
            }

            try
            {
                if (IsBase64(encoded))
                {
                    var s = Decode(encoded, DefaultXorKey);
                    if (!string.IsNullOrEmpty(s))
                    {
                        decoded = s;
                        return true;
                    }
                }
            }
            catch { /* ignore */ }

            return false;
        }

        private static byte[] DeriveKey(string passphrase, byte[] salt, int bytes)
        {
            using (var kdf = new Rfc2898DeriveBytes(passphrase ?? string.Empty, salt, PBKDF2Iterations, HashAlgorithmName.SHA256))
            {
                return kdf.GetBytes(bytes);
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}