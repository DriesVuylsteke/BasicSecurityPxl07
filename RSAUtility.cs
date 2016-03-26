using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSAWPF
{
    class RSAUtility
    {
        public static object Encode { get; private set; }

        // Generate a new key pair
        public static void Keys(string publicKeyFileName, string privateKeyFileName)
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
                publicKeyFile = File.CreateText(publicKeyFileName);
                publicKeyFile.Write(publicKey);
                // Export private/public key pair
                privateKey = rsaProvider.ToXmlString(true);
                // Write private/public key pair to file
                privateKeyFile = File.CreateText(privateKeyFileName);
                privateKeyFile.Write(privateKey);
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                Console.WriteLine("Exception generating a new key pair!More info:");
                Console.WriteLine(ex.Message);
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

        public static bool compareHash(string file1, string file2)
        {
            StreamReader file1Reader = null;
            StreamReader file2Reader = null;
            byte[] hash1 = null;
            byte[] hash2 = null;
            try
            {
                //Open files for reading
                file1Reader = File.OpenText(file1);
                file2Reader = File.OpenText(file2);
                hash1 = Encoding.Unicode.GetBytes(file1Reader.ReadToEnd());
                hash2 = Encoding.Unicode.GetBytes(file2Reader.ReadToEnd());
                return hash1 == hash2;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong!");
                Console.WriteLine(ex.Message);
            }
            finally
            {

                file1Reader.Close();
                file2Reader.Close();
            }
            return false;
        }

        //hash a file
        public static void Hash(string fileToHash, string hashedFile)
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


        //Sign a hash with a private key
        public static void SignHash(string privateKey, string fileToSign, string signedFile)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
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
            encryptedBytes = rsaProvider.SignHash(plainBytes, CryptoConfig.MapNameToOID("SHA1"));
            // Write encrypted text to file
            File.WriteAllBytes(signedFile, encryptedBytes);
        }

        //Remove a signature with the public key
        public static bool checkSignature(string publicKey, string signedFile, string hash)
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

        // Encrypt a file
        public static void Encrypt(string publicKeyFileName, string plainFileName, string encryptedFileName)
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
                // Encrypt plain text
                plainBytes = Encoding.Unicode.GetBytes(plainText);
                encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
                // Write encrypted text to file
                encryptedFile = File.Create(encryptedFileName);
                encryptedFile.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                Console.WriteLine("Exception encrypting file!More info:");
                Console.WriteLine(ex.Message);
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
        public static void Decrypt(string privateKeyFileName, string encryptedFileName, string plainFileName)
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
                // Any errors? Show them
                Console.WriteLine("Exception decrypting file!More info:");
                Console.WriteLine(ex.Message);
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
