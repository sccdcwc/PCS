using PCS.BLL;
using PCS.Model;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace PCS
{
    /// <summary>
    /// AddFolder.xaml 的交互逻辑
    /// </summary>
    public partial class AddFolder : Window
    {
        public UserModel user = new UserModel();
        public NodeModel node = new NodeModel();
        public TreeView treev = new TreeView();
        public AddFolder()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            FileBLL FB = new FileBLL();
            string sPath = string.Empty;
            string Path = string.Empty;
            string sSql = string.Format("select mlbm from yh_bdml where yh_guid='{0}' and mlbm like '9%%%%%' order by mlbm desc", user.YH_GUID);
            string sSql3=string.Format("select mlbm from yh_bdml where yh_guid='{0}' and fjmlid='{1}' and mlbtmc='{2}'",user.YH_GUID,node.FJID,MLBTMC.Text);
            DataTable dt = new DataTable();
            dt = sqlite.GetTable(sSql3);
            if (dt.Rows.Count == 0)//判断是否本地存在该目录
            {
                //不存在
                dt = sqlite.GetTable(sSql);
                int mlbm = Convert.ToInt32(dt.Rows[0]["MLBM"]) + 1;
                Path= FB.WritePath(node, Path, user);
                NodeModel nNode = new NodeModel();
                if (node != null)
                {
                    //在子目录中添加子目录
                    string sSql1 = string.Format("insert into YH_BDML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,FJMLID,MLBTMCQC) values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')),(select datetime('{6}')),'{7}','{8}','{9}','{10}')", user.YH_GUID, node.MLBM, "1", mlbm, MLBTMC.Text, sqlite.ChangeTime(DateTime.Now), sqlite.ChangeTime(DateTime.Now), "1", "0",node.ID,Path);
                    sqlite.ExecuteSql(sSql1);
                    sPath = node.FolderPath + MLBTMC.Text;

                    string sSql2 = string.Format("select * from yh_bdml where yh_guid='{0}' and fjmlid='{1}' and MLBTMC='{2}'", user.YH_GUID, node.ID, MLBTMC.Text);
                    DataTable dt1 = sqlite.GetTable(sSql2);
                    nNode.NodeName = MLBTMC.Text;
                    nNode.ID = dt1.Rows[0]["ML_ID"].ToString();
                    nNode.FJID = node.ID;
                    nNode.changetime = dt1.Rows[0]["GXSJ"].ToString();
                    nNode.MLBM = mlbm.ToString();
                    nNode.Nodes = null;
                    node.Nodes.Add(nNode);
                }
                else
                {
                    //添加父级目录
                    string sSql1 = string.Format("insert into YH_BDML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,FJMLID,MLBTMCQC) values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')),(select datetime('{6}')),'{7}','{8}','{9}','{10}')", user.YH_GUID, "0", "1", mlbm, MLBTMC.Text, sqlite.ChangeTime(DateTime.Now), sqlite.ChangeTime(DateTime.Now), "1", "0", "-1",MLBTMC.Text);
                    sqlite.ExecuteSql(sSql1);
                    string sSql2 = string.Format("select * from yh_pzb where pzmc='文件夹本地地址' and yh_guid={0}", user.YH_GUID);
                    DataTable dt1 = sqlite.GetTable(sSql2);
                    sPath = dt1.Rows[0]["PZNRZ"].ToString() + "\\" + MLBTMC.Text;
                }

            }
            else
            {
                string sSql4 = string.Format("update yh_bdml set qyzt='1',GXSJ=(select datetime('now','localtime')) where yh_guid='{0}' and fjmlbm='{1}' and mlbtmc='{2}'", user.YH_GUID, node.FJID, MLBTMC.Text);
                sPath = node.FolderPath + MLBTMC.Text;
            }
            Directory.CreateDirectory(sPath);

            this.Close();
        }
    }
}
