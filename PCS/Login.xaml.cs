using System;
using System.Data;
using System.IO;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using PCS.BLL;
using PCS.Model;
using OracleDB;


namespace PCS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private static UserModel m_User = new UserModel();
        public UserModel user
        {
            get { return m_User; }
            set { m_User = value; }
        }

        LoginService.Service1Client client = new LoginService.Service1Client();

        ClassSerialzation cs = new ClassSerialzation();
        bool bLogin = true;


        public Login()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoginBLL lb = new LoginBLL();
            SQLiteBLL sqlite = new SQLiteBLL();
            string sPath = Directory.GetCurrentDirectory() + "/PersonCloudSaveDataBase.s3db";
            try
            {
                if (bLogin)
                {
                    if (!File.Exists(sPath))
                    {
                        sqlite.CreateTable();
                    }

                    string sSql = "select yh.yhdlm,yh.dlmm,pz.pzmc,pz.pznrz from jc_yh yh join yh_pzb pz on pz.yh_guid=yh.yh_guid where yh.IsLast='true'";
                    bool bIsLastDL = sqlite.IsHaveInSqlite(sSql);
                    if (bIsLastDL)
                    {
                        DataTable dt = sqlite.GetTable(sSql);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["PZMC"].ToString() == "是否记住密码")
                            {
                                if (dt.Rows[i]["PZNRZ"].ToString() == "true")
                                {
                                    UserName.Text = dt.Rows[0]["YHDLM"].ToString();
                                    PswdBox.Password = dt.Rows[0]["DLMM"].ToString();
                                    RemPass.IsChecked = true;
                                }
                            }
                            if (dt.Rows[i]["PZMC"].ToString() == "是否自动登录")
                            {
                                if (dt.Rows[i]["PZNRZ"].ToString() == "true")
                                    lb.Login();
                            }
                            UserName.Text = dt.Rows[0]["YHDLM"].ToString();
                        }
                    }
                    bLogin = false;             //通过bLogin变量来判断是第一次打开客户端时修改登录名还是用户修改登录名
                }
            }
            catch (Exception ex)
            {

                string smsg = ex.Message;
            }
        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        /// <summary>
        /// 按下登录键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            LoginBLL lb = new LoginBLL();
            //OracleBLL ob = new OracleBLL();
            bool bSuccess = false;
            string pswd = FormsAuthentication.HashPasswordForStoringInConfigFile(PswdBox.Password, "MD5");

            bSuccess = client.IsUserHave(UserName.Text, pswd);
            //DataTable table = new DataTable();
            //string sql = string.Format("select * from jc_yh where YHDLM={0} and DLMM={1}", UserName.Text, PswdBox.Password);
            //table = ob.GetTable(sql);
            //if (table.Rows.Count > 0)
            //{
            //    bSuccess = true;
            //}
            if (bSuccess)
            {
                DataTable dt = new DataTable();
                string sSql = string.Format("select yh_guid,yhdlm,dlmm,zwxm,yhlx from jc_yh where yhdlm='{0}' and dlmm='{1}'", UserName.Text, FormsAuthentication.HashPasswordForStoringInConfigFile(PswdBox.Password, "MD5"));
                MemoryStream stream = client.GetTable(sSql);
                dt = cs.DeSerializeBinary(stream) as DataTable;
                user.YH_GUID = dt.Rows[0]["YH_GUID"].ToString();
                user.YHDLM = dt.Rows[0]["YHDLM"].ToString().Trim();
                user.DLMM = dt.Rows[0]["DLMM"].ToString().Trim();
                user.ZWXM = dt.Rows[0]["ZWXM"].ToString().Trim();
                user.YHLX = dt.Rows[0]["YHLX"].ToString();
                if (!sqlite.IsHaveInSqlite(string.Format("select * from jc_yh where YHDLM='{0}'", UserName.Text)))
                {                 
                    string sSqliteSql = string.Format("insert into jc_yh values ('{0}','{1}','{2}','{3}','{4}','true')", user.YH_GUID,UserName.Text.ToString(), PswdBox.Password, user.ZWXM, user.YHLX);
                    sqlite.ExecuteSql(sSqliteSql);
                }
                lb.ChangeIsLastLog(user.YHDLM);
                CourseManager cm = new CourseManager();
                cm.user = user;
                cm.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("用户名或密码错误");
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image ig = sender as Image;
            if (ig.Tag.ToString() == "close")
            {
                Uri ur = new Uri("Resource/close0.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
            else
            {
                Uri ur = new Uri("Resource/mini0.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)  
        {
            Image ig = sender as Image;
            if (ig.Tag.ToString() == "close")
            {
                Uri ur = new Uri("Resource/close1.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
            else
            {
                Uri ur = new Uri("Resource/mini1.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
        }

        private void Border_Loaded_1(object sender, RoutedEventArgs e)
        {
            Storyboard sdb = Resources["leafMove"] as Storyboard;
            sdb.Begin();

            Storyboard baiyun = Resources["cloudMove"] as Storyboard;
            baiyun.Begin();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image ig = sender as Image;
            if (ig.Tag.ToString() == "close")
            {
                Application.Current.Shutdown();
                this.Close();
            }
            else this.WindowState = System.Windows.WindowState.Minimized; ;
        }

        private void RemPass_Click(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            if (RemPass.IsChecked == false)
            {
                string sSql1 = string.Format("update yh_pzb set PZNRZ='false',PZGXSJ=(select datetime('now','localtime')) where yh_guid=(select yh_guid from jc_yh where yhdlm='{0}') and PZMC='是否记住密码'", UserName.Text);
                sqlite.ExecuteSql(sSql1);
            }
            else
            {
                string sSql1 = string.Format("update yh_pzb set PZNRZ='true',PZGXSJ=(select datetime('now','localtime')) where yh_guid=(select yh_guid from jc_yh where yhdlm='{0}') and PZMC='是否记住密码'", UserName.Text);
                sqlite.ExecuteSql(sSql1);
            }
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginBLL lb = new LoginBLL();
            SQLiteBLL sqlite = new SQLiteBLL();
            if (!bLogin)
            {
                bool bSuccess = false;
                string sSql = string.Format("select yh.yhdlm,yh.dlmm,pz.pznrz,pz.pzmc from jc_yh yh join yh_pzb pz on pz.yh_guid=yh.yh_guid where yh.yhdlm='{0}'", UserName.Text);
                bSuccess = lb.DLMChange(UserName.Text);
                DataTable dt = sqlite.GetTable(sSql);
                if (bSuccess)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["PZMC"].ToString() == "是否记住密码")
                        {
                            if (dt.Rows[i]["PZNRZ"].ToString() == "true")
                            {
                                UserName.Text = dt.Rows[i]["YHDLM"].ToString();
                                PswdBox.Password = dt.Rows[i]["DLMM"].ToString();
                                RemPass.IsChecked = true;
                            }
                        }
                        if (dt.Rows[i]["PZMC"].ToString() == "是否自动登录")
                        {
                            if (dt.Rows[i]["PZNRZ"].ToString() == "true")
                                AutoLog.IsChecked = true;
                        }
                    }
                }
               
            }
        }


        private void AutoLog_Click(object sender, RoutedEventArgs e)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            if (RemPass.IsChecked == false)
            {
                string sSql1 = string.Format("update yh_pzb set PZNRZ='false' where yh_guid=(select yh_guid from jc_yh where yhdlm='{0}') and PZMC='是否自动登录'", UserName.Text);
                sqlite.ExecuteSql(sSql1);
            }
            else
            {
                string sSql1 = string.Format("update yh_pzb set PZNRZ='true' where yh_guid=(select yh_guid from jc_yh where yhdlm='{0}') and PZMC='是否自动登录'", UserName.Text);
                sqlite.ExecuteSql(sSql1);
            }

        }
    }
}
