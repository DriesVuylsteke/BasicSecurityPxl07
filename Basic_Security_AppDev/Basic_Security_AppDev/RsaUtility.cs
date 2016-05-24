using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Basic_Security_AppDev
{
    class RSAUtility
    {
        public static object Encode { get; private set; }

        // Generate a new key pair
        public static void Keys(string folder)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamWriter publicKeyFile = null;
            StreamWriter privateKeyFile = null;
            string publicKey = "";
            string privateKey = "";
            try
            {
                // Create a new key pair on target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL
                //cspParams.ProviderName; // CSP name
                cspParams.Flags = CspProviderFlags.UseArchivableKey;
                cspParams.KeyNumber = (int)KeyNumber.Exchange;
                rsaProvider = new RSACryptoServiceProvider(cspParams);
                // Export public ke
                publicKey = rsaProvider.ToXmlString(false);
                // Write public key to file
                publicKeyFile = File.CreateText(folder+"\\Public.pubkey");
                publicKeyFile.Write(publicKey);
                // Export private/public key pair
                privateKey = rsaProvider.ToXmlString(true);
                // Write private/public key pair to file
                privateKeyFile = File.CreateText(folder + "\\Private.privkey");
                privateKeyFile.Write(privateKey);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Do some clean up if needed
                if (publicKeyFile != null)
                {
                    publicKeyFile.Close();
                }
                if (privateKeyFile != null)
                {
                    privateKeyFile.Close();
                }
            }
        } // Keys

        //hash a file
        public static void Hash(string fileToHash, string hashedFile)
        {
            try
            {
                SHA1Managed hash = new SHA1Managed();
                byte[] plainBytes = null;
                byte[] hashedBytes = null;

                // Read the bytes from the file
                plainBytes = File.ReadAllBytes(fileToHash);
                // Hash the plain text
                hashedBytes = hash.ComputeHash(plainBytes);
                File.WriteAllBytes(hashedFile, hashedBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Sign a File with a private key
        public static void SignHash(string privateKey, string fileToSign, string signedFile)
        {
            try
            {
                // Variables
                CspParameters cspParams = null;
                RSACryptoServiceProvider rsaProvider = null;
                SHA1Managed hash = new SHA1Managed();
                string privateKeyText = "";
                byte[] plainBytes = null;
                byte[] encryptedBytes = null;

                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL
                                            //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(cspParams);
                // Read public key from file
                privateKeyText = File.ReadAllText(privateKey);
                // Import public key
                rsaProvider.FromXmlString(privateKeyText);
                // Read hash text from file and turn it into bytes
                plainBytes = File.ReadAllBytes(fileToSign);
                // Encrypt the bytes
                encryptedBytes = rsaProvider.SignHash(hash.ComputeHash(plainBytes), CryptoConfig.MapNameToOID("SHA1"));
                // Write encrypted text to file
                File.WriteAllBytes(signedFile, encryptedBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Remove a signature with the public key
        public static bool CompareHashes(string publicKey, string signedFile, string hash)
        {
            try
            {
                // Variables
                CspParameters cspParams = null;
                RSACryptoServiceProvider rsaProvider = null;
                byte[] signedHash = null;
                byte[] hashBytes = null;

                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL
                                            //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(cspParams);
                // Import public key
                rsaProvider.FromXmlString(File.ReadAllText(publicKey));
                // Read binary signed hash file
                signedHash = File.ReadAllBytes(signedFile);
                // Read the original hash
                hashBytes = File.ReadAllBytes(hash);
                // Check the data
                return rsaProvider.VerifyHash(hashBytes, CryptoConfig.MapNameToOID("SHA1"), signedHash);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Encrypt a file
        public static void EncryptFile(string plainFileName, string publicKeyFileName, string encryptedFileName)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamReader publicKeyFile = null;
            StreamReader plainFile = null;
            FileStream encryptedFile = null;
            string publicKeyText = "";
            string plainText = "";
            byte[] plainBytes = null;
            byte[] encryptedBytes = null;
            try
            {
                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL
                //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(cspParams);
                // Read public key from file
                publicKeyFile = File.OpenText(publicKeyFileName);
                publicKeyText = publicKeyFile.ReadToEnd();
                // Import public key
                rsaProvider.FromXmlString(publicKeyText);
                // Read plain text from file
                plainFile = File.OpenText(plainFileName);
                plainText = plainFile.ReadToEnd();
                plainFile.Close();
                // Encrypt plain text
                plainBytes = Encoding.Unicode.GetBytes(plainText);
                encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
                // Write encrypted text to file
                encryptedFile = File.Create(encryptedFileName);
                encryptedFile.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Do some clean up if needed
                if (publicKeyFile != null)
                {
                    publicKeyFile.Close();
                }
                if (plainFile != null)
                {
                    plainFile.Close();
                }
                if (encryptedFile != null)
                {
                    encryptedFile.Close();
                }
            }
        } // Encrypt

        // Decrypt a file
        public static void DecryptFile(string encryptedFileName, string privateKeyFileName, string plainFileName)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamReader privateKeyFile = null;
            FileStream encryptedFile = null;
            StreamWriter plainFile = null;
            string privateKeyText = "";
            string plainText = "";
            byte[] encryptedBytes = null;
            byte[] plainBytes = null;
            try
            {
                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL
                //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(cspParams);
                // Read private/public key pair from file
                privateKeyFile = File.OpenText(privateKeyFileName);
                privateKeyText = privateKeyFile.ReadToEnd();
                // Import private/public key pair
                rsaProvider.FromXmlString(privateKeyText);
                // Read encrypted text from file
                encryptedFile = File.OpenRead(encryptedFileName);
                encryptedBytes = new byte[encryptedFile.Length];
                encryptedFile.Read(encryptedBytes, 0, (int)encryptedFile.Length);
                // Decrypt text
                plainBytes = rsaProvider.Decrypt(encryptedBytes, false);
                // Write decrypted text to file
                plainFile = File.CreateText(plainFileName);
                plainText = Encoding.Unicode.GetString(plainBytes);
                plainFile.Write(plainText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Do some clean up if needed
                if (privateKeyFile != null)
                {
                    privateKeyFile.Close();
                }
                if (encryptedFile != null)
                {
                    encryptedFile.Close();
                }
                if (plainFile != null)
                {
                    plainFile.Close();
                }
            }
        } // Decrypt
    }
}