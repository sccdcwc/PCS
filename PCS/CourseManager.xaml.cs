using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using PCS.BLL;
using PCS.Model;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;


namespace PCS
{
    /// <summary>
    /// CourseManager.xaml 的交互逻辑
    /// </summary>
    public partial class CourseManager : Window
    {
        public UserModel user = new UserModel();
        private TreeViewItem item = new TreeViewItem();
        public CourseManager()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            string sSql = string.Format("select * from yh_pzb where pzmc='文件夹本地地址' and yh_guid={0}", user.YH_GUID);
            string sPath = string.Empty;
            DataTable dt = sqlite.GetTable(sSql);
            if (dt.Rows.Count > 0)
            {
                sPath = dt.Rows[0]["PZNRZ"].ToString();
            }
            else
            {
                AddPZNR addpznr = new AddPZNR();
                addpznr.user = user;
                addpznr.ShowDialog();
            }

        }
        /// <summary>
        /// 选择树形节点查看目录下所有文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treev_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            NodeModel node = treev.SelectedItem as NodeModel;
            FileBLL fb = new FileBLL();
            List<FileInfomation> FileList = new List<FileInfomation>();
            FileList = fb.GetFileInfo(node, user.YH_GUID);
            Listv.ItemsSource = FileList;
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            NodeBLL NB = new NodeBLL();
            SQLiteBLL sqlite = new SQLiteBLL();
            FileBLL FB = new FileBLL();

            NodeModel Node = treev.SelectedItem as NodeModel;

            string sSql = string.Format("select * from yh_pzb where pzmc='文件夹本地地址' and yh_guid={0}", user.YH_GUID);
            DataTable dt = sqlite.GetTable(sSql);
            string sPath = dt.Rows[0]["PZNRZ"].ToString();
            sPath = sPath + "//" + FB.WritePath(Node, "", user);
            NB.DeleteNode(Node, user);
            Directory.Delete(sPath);
            ShowTree(treev);
        }

        /// <summary>
        /// 右键选中树形节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item1_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
        /// <summary>
        /// 添加系统目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 添加目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            string sSql = string.Format("select * from yh_pzb where pzmc='文件夹本地地址' and yh_guid={0}", user.YH_GUID);
            DataTable dt = sqlite.GetTable(sSql);
            string sPath = dt.Rows[0]["PZNRZ"].ToString();
            AddFolder af = new AddFolder();
            NodeModel Node = treev.SelectedItem as NodeModel;
            FileBLL FB = new FileBLL();
            sPath = sPath + "//" + FB.WritePath(Node, "", user);
            Node.FolderPath = sPath;
            af.user = user;
            af.node = Node;
            af.ShowDialog();
            ShowTree(treev);
        }
        /// <summary>
        /// 同步目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            ShowTree(treev);
        }
        /// <summary>
        /// 编辑目录（重命名）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            NodeModel Node = treev.SelectedItem as NodeModel;
            ReName rn = new ReName();
            rn.user = user;
            rn.node = Node;
            rn.ShowDialog();
            SQLiteBLL sqlite = new SQLiteBLL();
            NodeBLL NB = new NodeBLL();
            //添加树形目录数据
            ShowTree(treev);
        }

        private void treev_Loaded(object sender, RoutedEventArgs e)
        {
            NodeBLL NB = new NodeBLL();
            SQLiteBLL sqlite = new SQLiteBLL();
            FileBLL FB = new FileBLL();
            //添加树形目录数据
            ObservableCollection<NodeModel> NodeList = ShowTree(treev);
            //创建本地文件夹
            string sSql = string.Format("select * from yh_pzb where pzmc='文件夹本地地址' and yh_guid={0}", user.YH_GUID);
            DataTable dt = sqlite.GetTable(sSql);
            string sPath = dt.Rows[0]["PZNRZ"].ToString();
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
                FB.CreateLocalFolder(NodeList, sPath);
            }
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            ShowTree(treev);
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {

            TBCZBLL tbcz = new TBCZBLL();
            //同步信息
            tbcz.TBPZ(user);
            //tbcz.TBWJJ(user);
            tbcz.GXWJJ(user);
            tbcz.TBWJ(user);
            tbcz.AddJCML(user);
            NodeBLL NB = new NodeBLL();
            SQLiteBLL sqlite = new SQLiteBLL();
            FileBLL FB = new FileBLL();
            //添加树形目录数据
            ObservableCollection<NodeModel> NodeList = ShowTree(treev);
            //创建本地文件夹
            string sSql = string.Format("select * from yh_pzb where pzmc='文件夹本地地址' and yh_guid={0}", user.YH_GUID);
            DataTable dt = sqlite.GetTable(sSql);
            string sPath = dt.Rows[0]["PZNRZ"].ToString();
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
                FB.CreateLocalFolder(NodeList, sPath);
            }
        }

        private ObservableCollection<NodeModel> ShowTree(TreeView treev)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            NodeBLL NB = new NodeBLL();
            //添加树形目录数据
            string sSql = string.Format("select * from yh_bdml where FJMLID='-1' and yh_guid='{0}' order by pxh and mlbm", user.YH_GUID);
            DataTable dt = new DataTable();
            dt = sqlite.GetTable(sSql);
            ObservableCollection<NodeModel> NodeList = NB.WriteNode(dt);
            treev.ItemsSource = NodeList;
            return NodeList;
        }
        /// <summary>
        /// checkbox全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if (AllCheck.IsChecked == true)
            {
                Listv.SelectAll();
            }
            else
            {
                Listv.UnselectAll();
            }
        }
    }
}
