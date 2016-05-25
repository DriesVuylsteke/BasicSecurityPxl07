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
    public partial class RSA : Grid
    {
        public RSA()
        {
            InitializeComponent();
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            folderTextBox.Text = SelectFolder();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            string keyFolder = folderTextBox.Text;
            try
            {
                if (!Directory.Exists(keyFolder))
                    System.Windows.MessageBox.Show("Een van bovenstaande waarden is incorrect!");
                else
                {
                    RSAUtility.Keys(keyFolder);
                    folderTextBox.Clear();
                    System.Windows.MessageBox.Show("Your keypair has been created!");
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
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
    }
}
