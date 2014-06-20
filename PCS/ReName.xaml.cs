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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;

namespace PCS
{
    /// <summary>
    /// ReName.xaml 的交互逻辑
    /// </summary>
    public partial class ReName : Window
    {
        public UserModel user = new UserModel();
        public NodeModel node = new NodeModel();
        public ObservableCollection<NodeModel> items = new ObservableCollection<NodeModel>();
        public string sPath=string.Empty;
        public FileInfomation fileinf = new FileInfomation();
        int i;//判断是重命名文件还是目录的标示
        public ReName()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (node.NodeName != null)
                i = 0;

            if (fileinf.TITLE != null)
                i = 1;

            if (i == 0)
            {
                Namebox.Text = node.NodeName;
            }
            else
            {
                Namebox.Text = fileinf.TITLE;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            NodeBLL NB = new NodeBLL();
            FileBLL FB = new FileBLL();
            string sSql = string.Empty;
            if (i == 0)
            {
                sSql = string.Format("update yh_bdml set MLBTMC='{0}',GXSJ=(select datetime('now','localtime')) where yh_guid='{1}' and ml_id='{2}'", Namebox.Text, user.YH_GUID, node.ID);
                sqlite.ExecuteSql(sSql);
                string sPath1 = string.Empty;               
                sPath1 = FB.WritePath(node, sPath1, user);
                node.FolderPath =sPath+"//"+sPath1;
                DirectoryInfo di = new DirectoryInfo(node.FolderPath);
                string sPath2 = di.Parent.FullName + "/" + Namebox.Text;
                Directory.Move(node.FolderPath, sPath2);
                NodeModel NewNode = NB.GetFatherNode2(items, node);
                NewNode.Nodes.Remove(node);
                node.NodeName = Namebox.Text;
                NewNode.Nodes.Add(node);
                this.Close();
            }
            else
            {
                sSql = string.Format("update yh_bdwj set TITLE='{0}',GXSJ=(select datetime('now','localtime')) where yh_guid='{1}' and yhzy_id='{2}'", Namebox.Text, user.YH_GUID, fileinf.YHZY_ID);
                sqlite.ExecuteSql(sSql);
                this.Close();
            }
        }


    }
}
