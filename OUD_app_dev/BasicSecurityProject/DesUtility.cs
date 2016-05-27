using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BasicSecurityProject
{
    class DesUtility
    {
        public static string GenerateKey()
        {
            try
            {
                return Encoding.Default.GetString(DESCryptoServiceProvider.Create().Key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EncryptFile(string inputFile, string key, string encryptedFile)
        {
            try
            {
                var DES = new DESCryptoServiceProvider();
                DES.Key = Encoding.Default.GetBytes(key);
                DES.IV = Encoding.Default.GetBytes(key);

                var DESEncryptor = DES.CreateEncryptor();
                using (var fsread = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (var cryptostreamDecr = new CryptoStream(fsread, DESEncryptor, CryptoStreamMode.Read))
                using (var fswrite = new FileStream(encryptedFile, FileMode.Create, FileAccess.Write))
                {
                    cryptostreamDecr.CopyTo(fswrite);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DecryptFile(string encryptedFile, string key, string decryptedFile)
        {
            try
            {
                var DES = new DESCryptoServiceProvider();
                DES.Key = Encoding.Default.GetBytes(key);
                DES.IV = Encoding.Default.GetBytes(key);
                using (var desdecrypt = DES.CreateDecryptor())
                {
                    using (var fsread = new FileStream(encryptedFile, FileMode.Open, FileAccess.Read))
                    using (var cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read))
                    using (var fswrite = new FileStream(decryptedFile, FileMode.Create, FileAccess.Write))
                    {
                        cryptostreamDecr.CopyTo(fswrite);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}