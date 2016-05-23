using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;

namespace Basic_Security_AppDev
{
    /// <summary>
    /// Interaction logic for Encrypteren.xaml
    /// </summary>
    public partial class Decrypteren : Grid
    {
        public Decrypteren()
        {
            InitializeComponent();
        }

        private void steganografieCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ChangeEnabled();
        }

        private void steganografieCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeEnabled();
        }

        private void imageButton_Click(object sender, RoutedEventArgs e)
        {
            imageTextBox.Text = SelectFile("Bitmap|*.bmp");
        }

        private void encryptedFileButton_Click(object sender, RoutedEventArgs e)
        {
            encryptedFileTextBox.Text = SelectFile("Encrypted File|*.des");
        }

        private void encryptedDesKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            encryptedDesKeyFileTextBox.Text = SelectFile("Encrypted DES|*.rsa");
        }

        private void signedHashFileButton_Click(object sender, RoutedEventArgs e)
        {
            signedHashFileTextBox.Text = SelectFile("Signed Hash|*.signed");
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            folderTextBox.Text = SelectFolder();
        }

        private void myPrivateKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            myPrivateKeyFileTextBox.Text = SelectFile("Private Key|*.privkey");
        }

        private void sendersPulicKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            sendersPublicKeyFileTextBox.Text = SelectFile("Public Key|*.pubkey");
        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string image = imageTextBox.Text;
                string encryptedFile = encryptedFileTextBox.Text;
                string encryptedDesKeyFile = encryptedDesKeyFileTextBox.Text;
                string signedHashFile = signedHashFileTextBox.Text;
                string folder = folderTextBox.Text;
                string myPrivateKeyFile = myPrivateKeyFileTextBox.Text;
                string sendersPublicKeyFile = sendersPublicKeyFileTextBox.Text;

                if (((bool)steganografieCheckBox.IsChecked && !File.Exists(image)) || (!(bool)steganografieCheckBox.IsChecked &&  (!File.Exists(encryptedFile) || !File.Exists(encryptedDesKeyFile) || !File.Exists(signedHashFile))) || !Directory.Exists(folder) ||!File.Exists(myPrivateKeyFile) || !File.Exists(sendersPublicKeyFile))
                {
                    System.Windows.MessageBox.Show("Een van bovenstaande waarden is incorrect!");
                }
                else
                {
                    if ((bool)steganografieCheckBox.IsChecked)
                    {
                        SteganografieUtility.decodePicture(image,folder+"\\Encrypted.des",folder+"\\Des.rsa",folder+"\\Hah.signed");
                        encryptedFile = folder + "\\Encrypted.des";
                        encryptedDesKeyFile = folder + "\\Des.rsa";
                        signedHashFile = folder + "\\Hash.signed";
                    }

                    RSAUtility.DecryptFile(encryptedDesKeyFile, myPrivateKeyFile, folder + "\\Des.key");

                    StreamReader encryptedKeysr = new StreamReader(folder + "\\Des.key");
                    string fileName = encryptedKeysr.ReadLine();
                    string desKey = encryptedKeysr.ReadLine();
                    encryptedKeysr.Close();
                    
                    DesUtility.DecryptFile(encryptedFile, desKey, folder + "\\" + fileName);

                    RSAUtility.Hash(folder + "\\" + fileName, folder + "\\Hash.hash");
                    if (!RSAUtility.checkSignature(sendersPublicKeyFile, signedHashFile, folder + "\\Hash.hash"))
                    {
                        System.Windows.MessageBox.Show("Hashes komen niet overeen!", "Foutmelding");
                        File.Delete(folder + "\\Des.key");
                        File.Delete(folder + "\\" + fileName);
                    }
                    File.Delete(folder + "\\Hash.hash");
                    File.Delete(folder + "\\Des.key");
                    System.Windows.MessageBox.Show("Succesfully Decrypted!", "Succes");

                    File.Delete(folder + "\\Des.rsa");
                    File.Delete(folder + "\\Encrypted.des");
                    File.Delete(folder + "\\Hash.Signed");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!");
            }
        }

        private string SelectFile(string filter = null)
        {
            string path = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (filter != null)
                openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }

            return path;
        }

        private string SelectFolder()
        {
            string path = null;
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                path = dlg.SelectedPath;
            }
            return path;
        }

        private void ChangeEnabled()
        {
            imageTextBox.Text = string.Empty;
            imageTextBox.IsEnabled = !imageTextBox.IsEnabled;
            imageButton.IsEnabled = !imageButton.IsEnabled;
            encryptedFileTextBox.Text = string.Empty;
            encryptedFileTextBox.IsEnabled = !encryptedFileTextBox.IsEnabled;
            encryptedFileButton.IsEnabled = !encryptedFileButton.IsEnabled;
            encryptedDesKeyFileTextBox.Text = string.Empty;
            encryptedDesKeyFileTextBox.IsEnabled = !encryptedDesKeyFileTextBox.IsEnabled;
            encryptedDesKeyFileButton.IsEnabled = !encryptedDesKeyFileButton.IsEnabled;
            signedHashFileTextBox.Text = string.Empty;
            signedHashFileTextBox.IsEnabled = !signedHashFileTextBox.IsEnabled;
            signedHashFileButton.IsEnabled = !signedHashFileButton.IsEnabled;

        }
    }
}
