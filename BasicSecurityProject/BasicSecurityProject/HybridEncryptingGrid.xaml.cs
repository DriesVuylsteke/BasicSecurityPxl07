using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace BasicSecurityProject
{
    /// <summary>
    /// Interaction logic for HybridEncryptingGrid.xaml
    /// </summary>
    public partial class HybridEncryptingGrid : Grid
    {
        public HybridEncryptingGrid()
        {
            InitializeComponent();
        }

        //DesEncryption
        private void fileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                fileTextBox.Text = dlg.FileName;
            }
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderTextBox.Text = dlg.SelectedPath;
            }
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                desKeyTextBox.Text = DesUtility.GenerateKey();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+Environment.NewLine+ex.StackTrace, "Fout!");
            }
        }

        private void myPrivateKeySignButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                myPrivateKeySignTextBox.Text = dlg.FileName;
            }
        }

        private void receiversPublicKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                receiversPublicKeyTextBox.Text = dlg.FileName;
            }
        }

        private void Encrypt(string originalFile, string desKey, string destinationFolder, string receiversKeyRSAFile, string myKeyRSAFile)
        {
            try
            {
                DesUtility.EncryptFile(originalFile, desKey, destinationFolder + "\\Encrypted.file");

                StreamWriter generatedKey = new StreamWriter(destinationFolder + "\\Des.key");
                string file = System.IO.Path.GetFileName(originalFile);
                generatedKey.WriteLine(file);
                generatedKey.WriteLine(desKey);
                generatedKey.Flush();
                generatedKey.Close();

                RSAUtility.EncryptFile(destinationFolder + "\\Des.key", receiversKeyRSAFile, destinationFolder + "\\Des.rsa");
                File.Delete(destinationFolder + "\\Des.key");

                RSAUtility.SignHash(myKeyRSAFile, originalFile, destinationFolder + "\\Hash.signature");

                MessageBox.Show("Succesfully Encrypted!", "Succes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Fout!");
            }
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            string originalFile = fileTextBox.Text;
            string destinationFolder = folderTextBox.Text;
            string desKey = desKeyTextBox.Text;
            string myKeyRSAFile = myPrivateKeySignTextBox.Text;
            string receiversKeyRSAFile = receiversPublicKeyTextBox.Text;


            if (!(string.IsNullOrWhiteSpace(originalFile) || string.IsNullOrWhiteSpace(destinationFolder) || string.IsNullOrWhiteSpace(desKey) || string.IsNullOrWhiteSpace(myKeyRSAFile) || string.IsNullOrWhiteSpace(receiversKeyRSAFile)))
            {
                Thread t = new Thread(() => Encrypt(originalFile, desKey, destinationFolder, receiversKeyRSAFile, myKeyRSAFile));
                t.Start();
            }
            else
            {
                MessageBox.Show("Een van de waarden is incorrect!", "Foutmelding");
            }
        }

        //DesDecryption
        private void encryptedFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                encryptedFileTextBox.Text = dlg.FileName;
            }
        }

        private void encryptedKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                encryptedKeyTextBox.Text = dlg.FileName;
            }
        }

        private void signedHashButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                signedHashTextBox.Text = dlg.FileName;
            }
        }

        private void decryptedFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                decryptedFolderTextBox.Text = dlg.SelectedPath;
            }
        }

        private void myPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                myPrivateKeyTextBox.Text = dlg.FileName;
            }
        }

        private void sendersPublicKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                sendersPublicKeyTextBox.Text = dlg.FileName;
            }
        }

        private void Decrypt(string encryptedDesKey,string myPrivateKeyFile,string decryptedDestinationFolder, string encryptedFile, 
            string sendersPublicKeyFile, string signedHashFile)
        {
            try
            {
                RSAUtility.DecryptFile(encryptedDesKey, myPrivateKeyFile, decryptedDestinationFolder + "\\Des.key");

                StreamReader encryptedKeysr = new StreamReader(decryptedDestinationFolder + "\\Des.key");
                string fileName = encryptedKeysr.ReadLine();
                string desKey = encryptedKeysr.ReadLine();
                encryptedKeysr.Close();

                DesUtility.DecryptFile(encryptedFile, desKey, decryptedDestinationFolder + "\\" + fileName);

                RSAUtility.Hash(decryptedDestinationFolder + "\\" + fileName, decryptedDestinationFolder + "\\Hash.hash");
                if (!RSAUtility.checkSignature(sendersPublicKeyFile, signedHashFile, decryptedDestinationFolder + "\\Hash.hash"))
                {
                    MessageBox.Show("Hashes komen niet overeen!", "Foutmelding");
                    File.Delete(decryptedDestinationFolder + "\\Des.key");
                    File.Delete(decryptedDestinationFolder + "\\" + fileName);
                }
                File.Delete(decryptedDestinationFolder + "\\Hash.hash");
                File.Delete(decryptedDestinationFolder + "\\Des.key");
                MessageBox.Show("Succesfully Decrypted!", "Succes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Fout!");
            }
        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            string encryptedFile = encryptedFileTextBox.Text;
            string decryptedDestinationFolder = decryptedFolderTextBox.Text;
            string encryptedDesKey = encryptedKeyTextBox.Text;
            string myPrivateKeyFile = myPrivateKeyTextBox.Text;
            string sendersPublicKeyFile = sendersPublicKeyTextBox.Text;
            string signedHashFile = signedHashTextBox.Text;

            if (!(string.IsNullOrWhiteSpace(encryptedFile) || string.IsNullOrWhiteSpace(decryptedDestinationFolder) || string.IsNullOrWhiteSpace(encryptedDesKey) || string.IsNullOrWhiteSpace(myPrivateKeyFile) || string.IsNullOrWhiteSpace(sendersPublicKeyFile) || string.IsNullOrWhiteSpace(signedHashFile)))
            {
                Thread t = new Thread(() => Decrypt(encryptedDesKey, myPrivateKeyFile, decryptedDestinationFolder, encryptedFile, sendersPublicKeyFile, signedHashFile));
                t.Start();
            }
            else
            {
                MessageBox.Show("Een van de waarden is incorrect!", "Foutmelding");
            }
        }

        //RSAGeneration
        private void saveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveFolderTextBox.Text = dlg.SelectedPath;
            }
        }

        private void generateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            string folder = saveFolderTextBox.Text;

            if (!(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(folder)))
            {
                try
                {
                    RSAUtility.Keys(folder + "\\Public_" + name + ".key", folder + "\\Private_" + name + ".key");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+Environment.NewLine+ex.StackTrace+Environment.NewLine+ex.StackTrace, "Fout!");
                }
            }
            else
            {
                MessageBox.Show("Een van de waarden is incorrect!", "Foutmelding");
            }

        }
    }
}
