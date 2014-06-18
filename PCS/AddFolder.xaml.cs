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
        public AddFolder()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            string sPath = string.Empty;
            string sSql = string.Format("select mlbm from yh_bdml where yh_guid='{0}' and mlbm like '9%%%%%' order by mlbm desc", user.YH_GUID);
            string sSql3=string.Format("select mlbm from yh_bdml where yh_guid='{0}' and fjmlbm='{1}' and mlbtmc='{2}'",user.YH_GUID,node.FJID,MLBTMC.Text);
            DataTable dt = new DataTable();
            dt = sqlite.GetTable(sSql3);
            if (dt.Rows.Count == 0)
            {
                dt = sqlite.GetTable(sSql);
                int mlbm = Convert.ToInt32(dt.Rows[0]["MLBM"]) + 1;
                if (node != null)
                {
                    string sSql1 = string.Format("insert into YH_BDML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH) values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')),(select datetime('{6}')),'{7}','{8}')", user.YH_GUID, node.ID, "1", mlbm, MLBTMC.Text, sqlite.ChangeTime(DateTime.Now), sqlite.ChangeTime(DateTime.Now), "1", "0");
                    sqlite.ExecuteSql(sSql1);
                    sPath = node.FolderPath + MLBTMC.Text;
                }
                else
                {
                    string sSql1 = string.Format("insert into YH_BDML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH) values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')),(select datetime('{6}')),'{7}','{8}')", user.YH_GUID, "0", "1", mlbm, MLBTMC.Text, sqlite.ChangeTime(DateTime.Now), sqlite.ChangeTime(DateTime.Now), "1", "0");
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
