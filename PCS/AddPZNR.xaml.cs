using Microsoft.Win32;
using PCS.BLL;
using PCS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PCS
{
    /// <summary>
    /// AddPZNR.xaml 的交互逻辑
    /// </summary>
    public partial class AddPZNR : Window
    {
        public UserModel user = new UserModel();
        public AddPZNR()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.ShowDialog();
            if (FBD.SelectedPath != string.Empty)
            {
                string sPath = FBD.SelectedPath+"\\PersonCloudSave";
                DZ_Box.Text = sPath;
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            sqlite.DZInsert(user.YH_GUID, DZ_Box.Text);
            this.Close();
        }
    }
}
