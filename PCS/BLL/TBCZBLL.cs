using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.IO;
using PCS.BLL;
using PCS.Model;
using OracleDB;
using System.Windows;

namespace PCS.BLL
{
    public class TBCZBLL
    {
        /// <summary>
        /// 同步配置表
        /// </summary>
        /// <param name="user"></param>
        public void TBPZ(UserModel user)
        {
            try
            {
                ClassSerialzation cs = new ClassSerialzation();
                SQLiteBLL sqlite = new SQLiteBLL();
                LoginService.Service1Client client = new LoginService.Service1Client();
                TBService.Service1Client TBclient = new TBService.Service1Client();
                bool bSuccess = false;

                string sSql = string.Format("select * from YH_PZB where YH_GUID='{0}'", user.YH_GUID);
                bSuccess = sqlite.IsHaveInSqlite(sSql);    //本地同步表是否存在该用户的信息
                //获取服务器配置信息
                string sWLTimeSql = string.Format("select * from YH_PZB where YH_GUID='{0}'", user.YH_GUID);
                MemoryStream stream = client.GetTable(sWLTimeSql);
                DataTable dt1 = cs.DeSerializeBinary(stream) as DataTable;
                //if (dt1.Rows.Count == 0)
                //{
                //    string sInSql1 = string.Format("insert into YH_PZB ('YH_GUID','PZMC','PZLX','PZMRZ','PZNRZ','PZGXSJ') values ('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'))", user.YH_GUID, "是否记住密码", "登录配置", "false", "false", DateTime.Now);
                //    string sInSql2 = string.Format("insert into YH_PZB ('YH_GUID','PZMC','PZLX','PZMRZ','PZNRZ','PZGXSJ') values ('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'))", user.YH_GUID, "是否自动登录", "登录配置", "false", "false", DateTime.Now);
                //}
                if (bSuccess)
                {
                    //若存在
                    //获取本地配置信息
                    string sBDTimeSql = string.Format("select * from YH_PZB where YH_GUID='{0}'", user.YH_GUID);
                    DataTable dt = sqlite.GetTable(sBDTimeSql);

                    if (dt.Rows.Count > dt1.Rows.Count)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int m = -1;
                            string PzMc = dt.Rows[i]["PZMC"].ToString();
                            for (int l = 0; l < dt1.Rows.Count; l++)
                            {
                                if (dt1.Rows[l]["PZMC"].ToString() == PzMc)
                                {
                                    m = l;
                                    break;
                                }
                            }
                            //本地配置多余服务端配置
                            if (m == -1)
                            {
                                string sql = string.Format("insert into YH_PZB values ('{0}',SEC_YH_PZB.NEXTVAL,'{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'))", dt.Rows[i]["YH_GUID"], dt.Rows[i]["PZMC"], dt.Rows[i]["PZLX"], dt.Rows[i]["PZMRZ"], dt.Rows[i]["PZNRZ"], sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["PZGXSJ"])));
                                TBclient.ExcuteSql(sql);
                            }
                            else
                            {
                                bool bTime = CompareTime(dt.Rows[i]["PZGXSJ"].ToString(), dt1.Rows[m]["PZGXSJ"].ToString());
                                if (bTime)
                                {
                                    //服务器配置时间比本地新
                                    string stime = sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[m]["PZGXSJ"]));
                                    string sUpdateSql1 = string.Format("update YH_PZB set PZNRZ='{0}',PZGXSJ=(select datetime('{1}')) where YH_GUID='{2}' and PZMC='{3}'", dt1.Rows[m]["PZNRZ"], stime, user.YH_GUID, PzMc);
                                    sqlite.ExecuteSql(sUpdateSql1);
                                }
                                else
                                {
                                    //本地配置时间比服务器新
                                    string sUpdateSql2 = string.Format("update YH_PZB set PZNRZ='{0}',PZGXSJ=to_date('{1}','YYYY-MM-DD HH24:MI:SS') where YH_GUID='{2}' and PZMC='{3}'", dt.Rows[i]["PZNRZ"], sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["PZGXSJ"])), user.YH_GUID, PzMc);
                                    TBclient.ExcuteSql(sUpdateSql2);
                                }
                            }
                        }

                    }
                    else
                    {
                        if (dt.Rows.Count < dt1.Rows.Count)
                        {
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                int m = -1;
                                string PzMc = dt1.Rows[i]["PZMC"].ToString();
                                for (int l = 0; l < dt.Rows.Count; l++)
                                {
                                    if (dt.Rows[l]["PZMC"].ToString() == PzMc)
                                    {
                                        m = l;
                                        break;
                                    }
                                }
                                if (m == -1)
                                {

                                    string sql = string.Format("insert into YH_PZB ('YH_GUID','PZMC','PZLX','PZMRZ','PZNRZ','PZGXSJ') values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')))", dt.Rows[i]["YH_GUID"], dt.Rows[i]["PZMC"], dt.Rows[i]["PZLX"], dt.Rows[i]["PZMRZ"], dt.Rows[i]["PZNRZ"], dt.Rows[i]["PZGXSJ"]);
                                    sqlite.ExecuteSql(sql);
                                }
                                else
                                {
                                    bool bTime = CompareTime(dt.Rows[i]["PZGXSJ"].ToString(), dt1.Rows[m]["PZGXSJ"].ToString());
                                    if (bTime)
                                    {
                                        //服务器配置时间比本地新
                                        string stime = sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[m]["PZGXSJ"]));
                                        string sUpdateSql1 = string.Format("update YH_PZB set PZNRZ='{0}',PZGXSJ=(select datetime('{1}')) where YH_GUID='{2}' and PZMC='{3}'", dt1.Rows[m]["PZNRZ"], stime, user.YH_GUID, PzMc);
                                        sqlite.ExecuteSql(sUpdateSql1);
                                    }
                                    else
                                    {
                                        //本地配置时间比服务器新
                                        string sUpdateSql2 = string.Format("update YH_PZB set PZNRZ='{0}',PZGXSJ=to_date('{1}','YYYY-MM-DD HH24:MI:SS') where YH_GUID='{2}' and PZMC='{3}'", dt.Rows[i]["PZNRZ"], sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["PZGXSJ"])), user.YH_GUID, PzMc);
                                        TBclient.ExcuteSql(sUpdateSql2);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //若不存在
                    if (!TBclient.IsHavePZ(user.YH_GUID))
                    {
                        string sql1 = string.Format("insert into yh_pzb values ('{0}',SEC_YH_PZB.NEXTVAL,'是否记住密码','登录配置','false','false',to_date('{1}','YYYY-MM-DD HH24:MI:SS'))", user.YH_GUID, sqlite.ChangeTime(DateTime.Now));
                        client.ExcuteSql(sql1);
                        string sql2 = string.Format("insert into yh_pzb values ('{0}',SEC_YH_PZB.NEXTVAL,'是否自动登录','登录配置','false','false',to_date('{1}','YYYY-MM-DD HH24:MI:SS'))", user.YH_GUID, sqlite.ChangeTime(DateTime.Now));
                        client.ExcuteSql(sql2);

                    }
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        DateTime time = Convert.ToDateTime(dt1.Rows[i]["PZGXSJ"]);
                        string sSql1 = string.Format("insert into YH_PZB ('YH_GUID','PZMC','PZLX','PZMRZ','PZNRZ','PZGXSJ') values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')))", dt1.Rows[i]["YH_GUID"], dt1.Rows[i]["PZMC"], dt1.Rows[i]["PZLX"], dt1.Rows[i]["PZMRZ"], dt1.Rows[i]["PZNRZ"], sqlite.ChangeTime(time));
                        sqlite.ExecuteSql(sSql1);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void TBWJJ(UserModel user)
        {
            try
            {
                ClassSerialzation cs = new ClassSerialzation();
                SQLiteBLL sqlite = new SQLiteBLL();
                LoginService.Service1Client client = new LoginService.Service1Client();
                bool bSuccess = false;

                string sSql = string.Format("select * from YH_BDML where YH_GUID='{0}'", user.YH_GUID);
                bSuccess = sqlite.IsHaveInSqlite(sSql);    //本地文件目录表是否存在该用户的信息
                //获取服务器文件目录信息
                string sWLMLSql = string.Format("select * from YH_ML where YH_GUID='{0}'", user.YH_GUID);
                MemoryStream stream = client.GetTable(sWLMLSql);
                DataTable dt1 = cs.DeSerializeBinary(stream) as DataTable;

                if (bSuccess)
                {
                    //若存在
                    //获取本地文件目录信息
                    string sBDMLSql = string.Format("select * from YH_BDML where YH_GUID='{0}'", user.YH_GUID);
                    DataTable dt = sqlite.GetTable(sBDMLSql);
                    if (dt.Rows.Count > dt1.Rows.Count)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int m = -1;
                            string MLBM = dt.Rows[i]["MLBM"].ToString();
                            for (int l = 0; l < dt1.Rows.Count; l++)
                            {
                                if (dt1.Rows[l]["MLBM"].ToString() == MLBM)
                                {
                                    m = l;
                                    break;
                                }
                            }
                            if (m == -1)
                            {

                                string sql = string.Format("insert into YH_ML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,BZ) values ('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'),to_date('{6}','YYYY-MM-DD HH24:MI:SS'),'{7}','{8}','{9}')", dt.Rows[i]["YH_GUID"].ToString(), dt.Rows[i]["FJMLID"].ToString(), dt.Rows[i]["SFYZML"].ToString(), dt.Rows[i]["MLBM"].ToString(), dt.Rows[i]["MLBTMC"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["CJSJ"])), sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["GXSJ"])), dt.Rows[i]["QYZT"].ToString(), dt.Rows[i]["PXH"].ToString(), dt.Rows[i]["BZ"].ToString());
                                client.ExcuteSql(sql);
                            }
                            else
                            {
                                bool bTime = CompareTime(dt.Rows[i]["GXSJ"].ToString(), dt1.Rows[m]["GXSJ"].ToString());
                                if (bTime)
                                {
                                    //服务器文件目录更新时间比本地新
                                    string sUpdateSql1 = string.Format("update YH_BDML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=(select datetime('{4}')),BZ='{5}' where YH_GUID='{6}' and MLBM='{7}'", dt1.Rows[m]["FJMLBM"].ToString(), dt1.Rows[m]["MLBTMC"].ToString(), dt1.Rows[m]["SFYZML"].ToString(), dt1.Rows[m]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[m]["GXSJ"])), dt1.Rows[m]["BZ"].ToString(), user.YH_GUID, MLBM);
                                    sqlite.ExecuteSql(sUpdateSql1);
                                }
                                else
                                {
                                    //本地文件目录更新时间比服务器新
                                    string sUpdateSql2 = string.Format("update YH_ML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=to_date('{4}','YYYY-MM-DD HH24:MI:SS'),BZ='{5}' where YH_GUID='{6}' and MLBM='{7}'", dt.Rows[i]["FJMLBM"].ToString(), dt.Rows[i]["MLBTMC"].ToString(), dt.Rows[i]["SFYZML"].ToString(), dt.Rows[i]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["GXSJ"])), dt.Rows[i]["BZ"].ToString(), user.YH_GUID, MLBM); ;
                                    client.ExcuteSql(sUpdateSql2);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            int m = -1;
                            string MLBM = dt1.Rows[i]["MLBM"].ToString();
                            for (int l = 0; l < dt.Rows.Count; l++)
                            {
                                if (dt.Rows[l]["MLBM"].ToString() == MLBM)
                                {
                                    m = l;
                                    break;
                                }
                            }
                            if (m == -1)
                            {

                                string sql = string.Format("insert into YH_ML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,BZ) values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')),(select datetime('{6}')),'{7}','{8}','{9}')", dt1.Rows[i]["YH_GUID"].ToString(), dt1.Rows[i]["FJMLID"].ToString(), dt1.Rows[i]["SFYZML"].ToString(), dt1.Rows[i]["MLBM"].ToString(), dt1.Rows[i]["MLBTMC"].ToString(), dt1.Rows[i]["CJSJ"], dt1.Rows[i]["GXSJ"], dt1.Rows[i]["QYZT"].ToString(), dt1.Rows[i]["PXH"].ToString(), dt1.Rows[i]["BZ"].ToString());
                                sqlite.ExecuteSql(sql);
                            }
                            else
                            {
                                bool bTime = CompareTime(dt.Rows[i]["GXSJ"].ToString(), dt1.Rows[m]["GXSJ"].ToString());
                                if (bTime)
                                {
                                    //服务器文件目录更新时间比本地新
                                    string sUpdateSql1 = string.Format("update YH_BDML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=(select datetime('{4}')),BZ='{5}' where YH_GUID='{6}' and MLBM='{7}'", dt1.Rows[m]["FJMLBM"].ToString(), dt1.Rows[m]["MLBTMC"].ToString(), dt1.Rows[m]["SFYZML"].ToString(), dt1.Rows[m]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[m]["GXSJ"])), dt1.Rows[m]["BZ"].ToString(), user.YH_GUID, MLBM);
                                    sqlite.ExecuteSql(sUpdateSql1);
                                }
                                else
                                {
                                    //本地文件目录更新时间比服务器新
                                    string sUpdateSql2 = string.Format("update YH_ML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=to_date('{4}','YYYY-MM-DD HH24:MI:SS'),BZ='{5}' where YH_GUID='{6}' and MLBM='{7}'", dt.Rows[i]["FJMLBM"].ToString(), dt.Rows[i]["MLBTMC"].ToString(), dt.Rows[i]["SFYZML"].ToString(), dt.Rows[i]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt.Rows[i]["GXSJ"])), dt.Rows[i]["BZ"].ToString(), user.YH_GUID, MLBM); ;
                                    client.ExcuteSql(sUpdateSql2);
                                }
                            }
                        }

                    }

                }
                else
                {
                    //若不存在
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        try
                        {
                            DateTime time = Convert.ToDateTime(dt1.Rows[i]["CJSJ"]);
                            DateTime time1 = Convert.ToDateTime(dt1.Rows[i]["GXSJ"]);
                            string sSql1 = string.Format("insert into YH_BDML ('YH_GUID','FJMLBM','SFYZML','MLBM','MLBTMC','CJSJ','GXSJ','QYZT','PXH','MLBTMCQC','SFJCMLTQ','JCMLZSDBM') values ('{0}','{1}','{2}','{3}','{4}',(select datetime('{5}')),(select datetime('{6}')),'{7}','{8}','{9}','{10}','{11}')", dt1.Rows[i]["YH_GUID"].ToString(), dt1.Rows[i]["FJMLBM"].ToString(), dt1.Rows[i]["SFYZML"].ToString(), dt1.Rows[i]["MLBM"].ToString(), dt1.Rows[i]["MLBTMC"].ToString(), sqlite.ChangeTime(time), sqlite.ChangeTime(time1), dt1.Rows[i]["QYZT"].ToString(), dt1.Rows[i]["PXH"].ToString(), dt1.Rows[i]["MLBTMCQC"].ToString(), dt1.Rows[i]["SFJCMLTQ"].ToString(), dt1.Rows[i]["JCMLZSDBM"].ToString());
                            sqlite.ExecuteSql(sSql1);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    AddJCML(user);
                }
            }
            catch (Exception ex)
            { }
        }

        public void TBWJ(UserModel user)
        {
            try
            {
                ClassSerialzation cs = new ClassSerialzation();
                SQLiteBLL sqlite = new SQLiteBLL();
                LoginService.Service1Client client = new LoginService.Service1Client();
                bool bSuccess = false;

                string sSql = string.Format("select * from YH_BDWJ where YH_GUID='{0}'", user.YH_GUID);
                bSuccess = sqlite.IsHaveInSqlite(sSql);    //本地文件表是否存在该用户的信息
                //获取服务器文件信息
                string sWLWJSql = string.Format("select * from YH_ZY where YH_GUID='{0}'", user.YH_GUID);
                MemoryStream stream = client.GetTable(sWLWJSql);
                DataTable dt1 = cs.DeSerializeBinary(stream) as DataTable;

                if (bSuccess)
                {
                    //若存在
                    //获取本地文件信息
                    string sBDWJSql = string.Format("select * from YH_BDWJ where YH_GUID='{0}'", user.YH_GUID);
                    DataTable dt = sqlite.GetTable(sBDWJSql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int m = 0;
                        string YHZY_ID = dt.Rows[i]["YHZY_ID"].ToString();
                        for (int l = 0; l < dt1.Rows.Count; l++)
                        {
                            if (dt1.Rows[l]["YHZY_ID"].ToString() == YHZY_ID)
                            {
                                m = l;
                                break;
                            }
                        }

                        if (dt.Rows[i]["MD5"].ToString() != dt1.Rows[m]["MD5"].ToString())  //判断MD5值是否一致
                        {
                            //判断更新时间
                            bool btime = CompareTime(dt.Rows[i]["GXSJ"].ToString(), dt1.Rows[m]["GXSJ"].ToString());
                            if (btime)
                            {
                                //服务器文件更新时间比本地新
                                string sql1 = string.Format("update yh_bdzy set YZML='{0}',RTYPE='{1}',ZYLYFS='{2}',JMFS='{3}',MD5='{4}',WJJWLDZ='{5}',TITLE='{6}',FORMAT_LIST='{7}',WJDXDW='{8}',SUBJECT='{9}',GRADE_LIST='{10}',VERSION='{11}',USAGE_TYPE='{12}',GXSJ=(select datetime('{13}')) where YH_GUID='{14}' and YHZY_ID='{15}'", dt1.Rows[m]["YZML"].ToString(), dt1.Rows[m]["RTYPE"].ToString(), dt1.Rows[m]["ZYLYFS"].ToString(), dt1.Rows[m]["JMFS"].ToString(), dt1.Rows[m]["MD5"].ToString(), dt1.Rows[m]["WJJWLDZ"].ToString(), dt1.Rows[m]["TITLE"].ToString(), dt1.Rows[m]["FORMAT_LIST"].ToString(), dt1.Rows[m]["WJDXDW"].ToString(), dt1.Rows[m]["SUBJECT"].ToString(), dt1.Rows[m]["GRADE_LIST"].ToString(), dt1.Rows[m]["VERSION"].ToString(), dt1.Rows[m]["USAGE_TYPE"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[m]["GXSJ"])), dt1.Rows[m]["YH_GUID"].ToString(), dt1.Rows[m]["YHZY_ID"].ToString());
                                sqlite.ExecuteSql(sql1);
                            }
                            else
                            {
                                //本地文件更新时间比服务器新
                                string sql2 = string.Format("update yh_zy set YZML='{0}',RTYPE='{1}',ZYLYFS='{2}',JMFS='{3}',MD5='{4}',WJJWLDZ='{5}',TITLE='{6}',FORMAT_LIST='{7}',WJDXDW='{8}',SUBJECT='{9}',GRADE_LIST='{10}',VERSION='{11}',USAGE_TYPE='{12}',GXSJ=to_time('{13}','YYYY-MM-DD HH24:MI:SS') where YH_GUID='{14}' and YHZY_ID='{15}'", dt.Rows[i]["YZML"].ToString(), dt.Rows[i]["RTYPE"].ToString(), dt.Rows[i]["ZYLYFS"].ToString(), dt.Rows[i]["JMFS"].ToString(), dt.Rows[i]["MD5"].ToString(), dt.Rows[i]["WJJWLDZ"].ToString(), dt.Rows[i]["TITLE"].ToString(), dt.Rows[i]["FORMAT_LIST"].ToString(), dt.Rows[i]["WJDXDW"].ToString(), dt.Rows[i]["SUBJECT"].ToString(), dt.Rows[i]["GRADE_LIST"].ToString(), dt.Rows[i]["VERSION"].ToString(), dt.Rows[i]["USAGE_TYPE"].ToString(), dt.Rows[i]["GXSJ"], dt.Rows[i]["YH_GUID"].ToString(), dt.Rows[i]["YHZY_ID"].ToString());
                                client.ExcuteSql(sql2);
                            }

                        }
                        else
                        {
                            //MD5值一样 无操作

                        }
                    }

                }
                else
                {
                    //若不存在
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        try
                        {
                            DateTime time = Convert.ToDateTime(dt1.Rows[i]["GXSJ"]);
                            string sSql1 = string.Format("insert into YH_BDWJ ('YHZY_ID','YH_GUID','YZML','RTYPE','ZYLYFS','JMFS','MD5','WJJWLDZ','TITLE','FORMAT_LIST','WJDXDW','SUBJECT','GRADE_LIST','VERSION','USAGE_TYPE','GXSJ') values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',(select datetime('{15}')))", dt1.Rows[i]["YHZY_ID"].ToString(), dt1.Rows[i]["YH_GUID"].ToString(), dt1.Rows[i]["YZML"].ToString(), dt1.Rows[i]["RTYPE"].ToString(), dt1.Rows[i]["ZYLYFS"].ToString(), "false", dt1.Rows[i]["MD5"].ToString(), dt1.Rows[i]["WJJWLDZ"].ToString(), dt1.Rows[i]["TITLE"].ToString(), dt1.Rows[i]["FORMAT_LIST"].ToString(), dt1.Rows[i]["WJDXDW"].ToString(), dt1.Rows[i]["SUBJECT"].ToString(), dt1.Rows[i]["GRADE_LIST"].ToString(), dt1.Rows[i]["VERSION"].ToString(), dt1.Rows[i]["USAGE_TYPE"].ToString(), sqlite.ChangeTime(time));
                            sqlite.ExecuteSql(sSql1);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CompareTime(string time1, string time2)
        {
            bool bSuccess = false;
            DateTime dt1 = Convert.ToDateTime(time1);
            DateTime dt2 = Convert.ToDateTime(time2);
            if (DateTime.Compare(dt1, dt2) > 0)
            {
                //前者时间大于后者时间及前者时间较新
                bSuccess = false;
            }
            else
            {
                //后者时间大于前者时间及后者时间较新
                bSuccess = true;
            }
            return bSuccess;
        }
        /// <summary>
        /// 添加基础目录信息
        /// </summary>
        /// <param name="user"></param>
        public void AddJCML(UserModel user)
        {
            LoginService.Service1Client client = new LoginService.Service1Client();
            ClassSerialzation cs = new ClassSerialzation();
            SQLiteBLL sqlite = new SQLiteBLL();
            //获取是否基础目录提取信息
            string sSql1 = string.Format("select * from yh_bdml where SFJCMLTQ='1' and yh_guid='{0}'", user.YH_GUID);
            DataTable dt1 = sqlite.GetTable(sSql1);
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //从网络基础目录表中获取基础目录信息
                string sSql2 = string.Format("select * from jc_ml where fjmlbm='{0}'", dt1.Rows[i]["JCMLZSDBM"]);
                MemoryStream stream = client.GetTable(sSql2);
                DataTable dt2 = cs.DeSerializeBinary(stream) as DataTable;
                for (int l = 0; l < dt2.Rows.Count; l++)
                {
                    //查询本地是否存在基础目录信息
                    string sSql3 = string.Format("select * from YH_BDML where YH_ML_ID='{0}'", dt2.Rows[l]["YH_ML_ID"]);
                    if (!sqlite.IsHaveInSqlite(sSql3))
                    {
                        //若不存在则插入基础目录信息至本地数据库
                        string sSql4 = string.Format("insert into YH_BDML ('YH_ML_ID','FJMLID','FJMLBM','SFYZML','MLBM','MLBTMC','QYZT','PXH') values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",dt2.Rows[i]["YH_ML_ID"].ToString(),dt2.Rows[i]["FJMLID"].ToString(), dt2.Rows[l]["FJMLBM"].ToString(), dt2.Rows[l]["SFYZML"].ToString(), dt2.Rows[l]["MLBM"].ToString(), dt2.Rows[l]["MLBTMC"].ToString(), dt2.Rows[l]["QYZT"].ToString(), dt2.Rows[l]["PXH"].ToString(), dt2.Rows[i]["MLBTMCQC"]);
                        sqlite.ExecuteSql(sSql4);
                    }
                    //查询本地数据库是否存在基础目录的子目录信息
                    string sSql5 = string.Format("select * from YH_BDML where FJMLID='{0}'", dt2.Rows[l]["FJMLID"]);
                    if (!sqlite.IsHaveInSqlite(sSql5))
                    {
                        //若不存在则查询网络端基础目录子目录信息
                        string sSql6 = string.Format("select * from jc_ml where FJMLID='{0}'", dt2.Rows[l]["YH_ML_ID"]);
                        stream = client.GetTable(sSql6);
                        DataTable dt3 = cs.DeSerializeBinary(stream) as DataTable;
                        for (int m = 0; m < dt3.Rows.Count; m++)
                        {
                            //插入基础目录子目录信息至本地数据库
                            string sSql7 = string.Format("insert into YH_BDML (('YH_ML_ID','FJMLID','FJMLBM','SFYZML','MLBM','MLBTMC','QYZT','PXH') values ('{0}','{1}','{2}','{3}','{4}','{5}'','{6}','{7}')",dt3.Rows[m]["YH_ML_ID"],dt3.Rows[i]["FJMLID"], dt3.Rows[m]["FJMLBM"].ToString(), dt3.Rows[m]["SFYZML"].ToString(), dt3.Rows[m]["MLBM"].ToString(), dt3.Rows[m]["MLBTMC"].ToString(), dt3.Rows[m]["QYZT"].ToString(), dt3.Rows[m]["PXH"].ToString(), dt3.Rows[m]["MLBTMCQC"]);
                            sqlite.ExecuteSql(sSql7);
                        }
                    }
                }
            }
        }

        public void GXWJJ(UserModel user)
        {
            ClassSerialzation cs = new ClassSerialzation();
            LoginService.Service1Client client = new LoginService.Service1Client();
            SQLiteBLL sqlite = new SQLiteBLL();
            //获取本地同步时间
            string sSql1 = string.Format("select * from yh_pzb where yh_guid='{0}' and PZMC='同步时间'", user.YH_GUID);
            DataTable dt = sqlite.GetTable(sSql1);
            if (dt.Rows.Count>0)
            {
                string time = dt.Rows[0]["PZNRZ"].ToString();//本地同步时间

                //获取更新时间在本地同步时间之后的文件夹信息
                string sSql2 = string.Format("select * from yh_bdml where yh_guid='{0}' and GXSJ>datetime('{1}')", user.YH_GUID, sqlite.ChangeTime(Convert.ToDateTime(time)));
                DataTable dt2 = sqlite.GetTable(sSql2);
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    //获取该条消息在服务端的信息
                    string sSql3 = string.Format("select * from yh_ml where yh_guid={0} and yh_ml_id='{1}'", user.YH_GUID, dt2.Rows[i]["YH_ML_ID"]);
                    MemoryStream stream = new MemoryStream();
                    stream = client.GetTable(sSql3);
                    DataTable dt3 = cs.DeSerializeBinary(stream) as DataTable;
                    if (dt3.Rows.Count > 0)
                    {
                        DateTime WLtime = Convert.ToDateTime(dt3.Rows[0]["GXSJ"]);
                        DateTime BDtime = Convert.ToDateTime(dt2.Rows[i]["GXSJ"]);
                        if (WLtime > BDtime)
                        {
                            string sUpdateSql1 = string.Format("update YH_BDML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=(select datetime('{4}')),BZ='{5}' where YH_GUID='{6}' and YH_ML_ID='{7}'", dt3.Rows[0]["FJMLBM"].ToString(), dt3.Rows[0]["MLBTMC"].ToString(), dt3.Rows[0]["SFYZML"].ToString(), dt3.Rows[0]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt3.Rows[0]["GXSJ"])), dt3.Rows[0]["BZ"].ToString(), user.YH_GUID, dt3.Rows[0]["YH_ML_ID"]);
                            sqlite.ExecuteSql(sUpdateSql1);
                        }
                        else
                        {
                            //本地文件目录更新时间比服务器新
                            string sUpdateSql2 = string.Format("update YH_ML set MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=to_date('{4}','YYYY-MM-DD HH24:MI:SS'),BZ='{5}' where YH_GUID='{6}' and YH_ML_ID='{7}'", dt2.Rows[i]["FJMLBM"].ToString(), dt2.Rows[i]["MLBTMC"].ToString(), dt2.Rows[i]["SFYZML"].ToString(), dt2.Rows[i]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt2.Rows[i]["GXSJ"])), dt2.Rows[i]["BZ"].ToString(), user.YH_GUID, dt2.Rows[i]["YH_ML_ID"]); ;
                            client.ExcuteSql(sUpdateSql2);
                        }
                    }
                    else
                    {
                        string sInsertSql = string.Format("insert into YH_ML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,BZ,YH_ML_ID,FJMLID) values ('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'),to_date('{6}','YYYY-MM-DD HH24:MI:SS'),'{7}','{8}','{9}','{10}','{11}')", dt2.Rows[i]["YH_GUID"].ToString(), dt2.Rows[i]["FJMLBM"].ToString(), dt2.Rows[i]["SFYZML"].ToString(), dt2.Rows[i]["MLBM"].ToString(), dt2.Rows[i]["MLBTMC"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt2.Rows[i]["CJSJ"])), sqlite.ChangeTime(Convert.ToDateTime(dt2.Rows[i]["GXSJ"])), dt2.Rows[i]["QYZT"].ToString(), dt2.Rows[i]["PXH"].ToString(), dt2.Rows[i]["BZ"].ToString(), dt2.Rows[i]["YH_ML_ID"].ToString(),dt2.Rows[i]["FJMLID"].ToString());
                        client.ExcuteSql(sInsertSql);
                    }
                }
                sSql2 = string.Format("select * from yh_ml where yh_guid='{0}' and GXSJ>'{1}'", user.YH_GUID, time);
                MemoryStream stream2 = new MemoryStream();
                stream2 = client.GetTable(sSql2);
                dt2 = cs.DeSerializeBinary(stream2) as DataTable;
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    //获取该条消息在本地端的信息
                    string sSql3 = string.Format("select * from yh_bdml where yh_guid={0} and yh_ml_id='{1}'", user.YH_GUID, dt2.Rows[i]["yh_ml_id"]);
                    DataTable dt3 = sqlite.GetTable(sSql3);
                    if (dt3.Rows.Count > 0)
                    {
                        DateTime WLtime = Convert.ToDateTime(dt2.Rows[0]["GXSJ"]);
                        DateTime BDtime = Convert.ToDateTime(dt3.Rows[i]["GXSJ"]);
                        if (WLtime > BDtime)
                        {
                            string sUpdateSql1 = string.Format("update YH_BDML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=(select datetime('{4}')),BZ='{5}' where YH_GUID='{6}' and yh_ml_id='{7}'", dt3.Rows[0]["FJMLBM"].ToString(), dt3.Rows[0]["MLBTMC"].ToString(), dt3.Rows[0]["SFYZML"].ToString(), dt3.Rows[0]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt3.Rows[0]["GXSJ"])), dt3.Rows[0]["BZ"].ToString(), user.YH_GUID, dt3.Rows[0]["yh_ml_id"]);
                            sqlite.ExecuteSql(sUpdateSql1);
                        }
                        else
                        {
                            //本地文件目录更新时间比服务器新
                            string sUpdateSql2 = string.Format("update YH_ML set FJMLBM='{0}',MLBTMC='{1}',SFYZML='{2}',QYZT='{3}',GXSJ=to_date('{4}','YYYY-MM-DD HH24:MI:SS'),BZ='{5}' where YH_GUID='{6}' and yh_ml_id='{7}'", dt2.Rows[i]["FJMLBM"].ToString(), dt2.Rows[i]["MLBTMC"].ToString(), dt2.Rows[i]["SFYZML"].ToString(), dt2.Rows[i]["QYZT"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt2.Rows[i]["GXSJ"])), dt2.Rows[i]["BZ"].ToString(), user.YH_GUID, dt2.Rows[i]["yh_ml_id"]); ;
                            client.ExcuteSql(sUpdateSql2);
                        }
                    }
                    else
                    {
                        string sInsertSql = string.Format("insert into YH_ML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,BZ) values ('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'),to_date('{6}','YYYY-MM-DD HH24:MI:SS'),'{7}','{8}','{9}')", dt2.Rows[i]["YH_GUID"].ToString(), dt2.Rows[i]["FJMLID"].ToString(), dt2.Rows[i]["SFYZML"].ToString(), dt2.Rows[i]["MLBM"].ToString(), dt2.Rows[i]["MLBTMC"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt2.Rows[i]["CJSJ"])), sqlite.ChangeTime(Convert.ToDateTime(dt2.Rows[i]["GXSJ"])), dt2.Rows[i]["QYZT"].ToString(), dt2.Rows[i]["PXH"].ToString(), dt2.Rows[i]["BZ"].ToString());
                        sqlite.ExecuteSql(sInsertSql);
                    }

                }
            }
            else
            {
                //若本地没有同步时间，及本地无文件目录信息，因此插入文件目录信息
                string sSql2 = string.Format("select * from yh_ml where yh_guid='{0}'", user.YH_GUID);
                MemoryStream stream = client.GetTable(sSql2);
                DataTable dt1 = cs.DeSerializeBinary(stream) as DataTable;
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count;i++)
                    {
                        string sInsertSql = string.Format("insert into YH_ML (YH_GUID,FJMLBM,SFYZML,MLBM,MLBTMC,CJSJ,GXSJ,QYZT,PXH,BZ,YH_ML_ID,FJMLID) values ('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'),to_date('{6}','YYYY-MM-DD HH24:MI:SS'),'{7}','{8}','{9}','{10}','{11}')", dt1.Rows[i]["YH_GUID"].ToString(), dt1.Rows[i]["FJMLID"].ToString(), dt1.Rows[i]["SFYZML"].ToString(), dt1.Rows[i]["MLBM"].ToString(), dt1.Rows[i]["MLBTMC"].ToString(), sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[i]["CJSJ"])), sqlite.ChangeTime(Convert.ToDateTime(dt1.Rows[i]["GXSJ"])), dt1.Rows[i]["QYZT"].ToString(), dt1.Rows[i]["PXH"].ToString(), dt1.Rows[i]["BZ"].ToString(),dt1.Rows[i]["YH_ML_ID"].ToString(),dt1.Rows[i]["FJMLID"].ToString());
                        sqlite.ExecuteSql(sInsertSql);                  
                    }
                }
                string sSql3 = string.Format("insert into YH_PZB ('YH_GUID','PZMC','PZLX','PZMRZ','PZNRZ','PZGXSJ') values ('{0}','同步时间','同步配置','',(select datetiem('now','localtime')),(select datetiem('now','localtime')))",user.YH_GUID);
                sqlite.ExecuteSql(sSql3);
                AddJCML(user);
            }

        }
    }
}
