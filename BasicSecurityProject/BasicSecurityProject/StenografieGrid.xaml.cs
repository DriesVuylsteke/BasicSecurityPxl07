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
    public partial class StenografieGrid : Grid
    {
        public StenografieGrid()
        {
            InitializeComponent();
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

        private void imageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Bitmap Images (.bmp)|*.bmp";
            if (dlg.ShowDialog() == true)
            {
                imageTextBox.Text = dlg.FileName;
            }
        }

        private void embedButton_Click(object sender, RoutedEventArgs e)
        {
            string encryptedFile = encryptedFileTextBox.Text;
            string encryptedDesKey = encryptedKeyTextBox.Text;
            string signedHashFile = signedHashTextBox.Text;
            string image = imageTextBox.Text;

            if (!(string.IsNullOrWhiteSpace(encryptedFile) || string.IsNullOrWhiteSpace(encryptedDesKey) || string.IsNullOrWhiteSpace(signedHashFile)) || string.IsNullOrWhiteSpace(image))
            {
                Thread t = new Thread(() => StenografieUtility.embed(encryptedFile, encryptedDesKey, signedHashFile, new BitmapImage(new Uri(image))));
                t.Start();
            }
            else
            {
                MessageBox.Show("Een van de waarden is incorrect!", "Foutmelding");
            }
        }

        private void embeddedImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                embeddedImageTextBox.Text = dlg.FileName;
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

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            string embeddedImage = embeddedImageTextBox.Text;
            string decryptionFolder = decryptedFolderTextBox.Text;

            if (!(string.IsNullOrWhiteSpace(embeddedImage) || string.IsNullOrWhiteSpace(decryptionFolder)))
            {
                Thread t = new Thread(() => StenografieUtility.decodePicture(new BitmapImage(new Uri(embeddedImage)), decryptionFolder + "Encrypted.file", decryptionFolder + "Des.rsa", decryptionFolder + "Hash.signature"));
                t.Start();
            }
            else
            {
                MessageBox.Show("Een van de waarden is incorrect!", "Foutmelding");
            }
        }
    }
}
