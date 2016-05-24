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
    public partial class Encrypteren : Grid
    {
        public Encrypteren()
        {
            InitializeComponent();
        }

        private void fileButton_Click(object sender, RoutedEventArgs e)
        {
            fileTextBox.Text = SelectFile();
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            folderTextBox.Text = SelectFolder();
        }

        private void desButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                desTextBox.Text = DesUtility.GenerateKey();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Fout!");
            }
        }

        private void myPrivateKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            myPrivateKeyFileTextBox.Text = SelectFile("Private Key|*.privkey");
        }

        private void receiversPublicKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            receiversPublicKeyFileTextBox.Text = SelectFile("Public Key|*.pubkey");
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

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                string file = fileTextBox.Text;
                string folder = folderTextBox.Text;
                string desKey = desTextBox.Text;
                string myPrivateKey = myPrivateKeyFileTextBox.Text;
                string receiversPublicKey = receiversPublicKeyFileTextBox.Text;
                string image = imageTextBox.Text;

                if (!File.Exists(file) || !Directory.Exists(folder) || string.IsNullOrEmpty(desKey) || !File.Exists(myPrivateKey) || !File.Exists(receiversPublicKey) || ((bool)steganografieCheckBox.IsChecked && !File.Exists(image)))
                {
                    System.Windows.MessageBox.Show("Een van bovenstaande waarden is incorrect!");
                }
                else
                {
                    //// 1.1. Hash maken van file
                    //RSAUtility.Hash(file, folder + "\\Hash.hash");
                    // 1.2. Hash signen met private key sender (RSA)
                    RSAUtility.SignHash(myPrivateKey, file, folder + "\\Hash.signed");
                    //File.Delete(folder + "\\Hash.hash");

                    // 2.1. File encrypteren met des sleutel
                    DesUtility.EncryptFile(file, desKey, folder + "\\Encrypted.des");

                    // 3.1. file maken met deskey en filename in
                    StreamWriter generatedKey = new StreamWriter(folder + "\\Des.key");
                    string fileName = System.IO.Path.GetFileName(file);
                    generatedKey.WriteLine(fileName);
                    generatedKey.WriteLine(desKey);
                    generatedKey.Flush();
                    generatedKey.Close();
                    // 3.2. bovenstaande file encrypteren met public key receiver (RSA)
                    RSAUtility.EncryptFile(folder + "\\Des.key", receiversPublicKey, folder + "\\Des.rsa");
                    //File.Delete(folder + "\\Des.key");

                    // 4.1. steganografie toepassen
                    if ((bool)steganografieCheckBox.IsChecked)
                    {
                        SteganografieUtility.embed(folder + "\\Encrypted.des", folder + "\\Des.rsa", folder + "\\Hash.signed", image, folder);
                        //File.Delete(folder + "\\Encrypted.des");
                        //File.Delete(folder + "\\Des.rsa");
                        //File.Delete(folder + "\\Hash.signed");
                    }

                    System.Windows.MessageBox.Show("Succesfully Encrypted!", "Succes");
                    ClearFields();
                }
        //}
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show(ex.Message, "Error!");
        //    }
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
        }

        private void ClearFields()
        {
            fileTextBox.Clear();
            folderTextBox.Clear();
            desTextBox.Clear();
            myPrivateKeyFileTextBox.Clear();
            receiversPublicKeyFileTextBox.Clear();
            steganografieCheckBox.IsChecked = false;
            imageTextBox.Clear();
        }
    }
}
