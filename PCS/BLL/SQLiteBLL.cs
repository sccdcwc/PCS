using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace PCS.BLL
{
    class SQLiteBLL
    {
        string sDataSourcePath = Directory.GetCurrentDirectory() + "/PersonCloudSaveDataBase.s3db";
        SQLiteConnection conn = new SQLiteConnection();
        public SQLiteBLL()
        {
            if (File.Exists(sDataSourcePath))
            {
                SQLiteConnectionStringBuilder ConnStr = new SQLiteConnectionStringBuilder();
                ConnStr.DataSource = sDataSourcePath;
                conn.ConnectionString = ConnStr.ToString();
                conn.SetPassword("123456");
                conn.Open();
            }
            else
            {
                string datasource = sDataSourcePath;
                SQLiteConnection.CreateFile(datasource);
                SQLiteConnectionStringBuilder ConnStr = new SQLiteConnectionStringBuilder();
                ConnStr.DataSource = sDataSourcePath;
                ConnStr.Password = "123456";
                conn.ConnectionString = ConnStr.ToString();
                conn.Open();
                CreateTable();
            }
        }

        public SQLiteConnection ConnnectSQL(string datasource)
        {
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder ConnStr = new SQLiteConnectionStringBuilder();
            ConnStr.DataSource = datasource;
            ConnStr.Password = "123456";
            conn.ConnectionString = ConnStr.ToString();
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sSql"></param>
        /// <param name="conn"></param>
        public void ExecuteSql(string sSql)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = sSql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// 获取课程节点列表
        /// </summary>
        /// <param name="sSql"></param>
        /// <returns></returns>
        public ObservableCollection<object> ReadData(string sSql)
        {
            ObservableCollection<object> ObservableObj = new ObservableCollection<object>();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = sSql;
            cmd.Connection = conn;
            System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ObservableObj.Add(new { CourseID = reader.GetInt16(0), CourseName = reader.GetString(1), CourseTime = reader.GetDateTime(2), Size = reader.GetString(3), Subject = reader.GetString(4), SourceKind = reader.GetString(5), TeacherName = reader.GetString(6) });
            }
            return ObservableObj;
        }

        /// <summary>
        /// 判断是否存在于数据库
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public bool IsHaveInSqlite(string Sql)
        {
            bool bSuccess = false;
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = Sql;
            cmd.Connection = conn;
            System.Data.SQLite.SQLiteDataAdapter reader = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            reader.Fill(dt);
            if (dt.Rows.Count > 0)
                bSuccess = true;
            return bSuccess;
        }

        /// <summary>
        /// 添加课程
        /// </summary>
        /// <param name="file"></param>
        public void AddCourse(FileInfo file)
        {
            string a = file.CreationTime.Hour.ToString() + ":" + file.CreationTime.Minute.ToString() + ":" + file.CreationTime.Second.ToString();
            string sSql = string.Format("insert into Course (CourseName,CourseTime,Size,Subject,SourceKind,TeacherName,NetAddress,LocalFolderAddressID) values ('{0}', '{1}','{2}','yw','{3}','{4}','123','{5}')", file.Name, a, file.Length, file.Extension, "", 1);
            ExecuteSql(sSql);
        }

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="sSql"></param>
        /// <returns></returns>
        public DataTable GetTable(string sSql)
        {
            DataTable dt = new DataTable();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = sSql;
            cmd.Connection = conn;
            System.Data.SQLite.SQLiteDataAdapter reader = new SQLiteDataAdapter(cmd);
            reader.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 创建本地数据库表
        /// </summary>
        public void CreateTable()
        {
            //创建用户表
            string Sql1 = "create table JC_YH (YH_GUID VARCHAR(12) not null,YHDLM VARCHAR(32),DLMM VARCHAR(128),ZWXM VARCHAR(64),YHLX VARCHAR(8),IsLast VARCHAR(8) Default 'false',primary key (YH_GUID))";
            ExecuteSql(Sql1);
            //创建个人本地文件夹表
            string FolderSql = "create table YH_BDML(YH_BD_ID integer primary key,FJMLID integer,YH_GUID integer,FJMLBM integer(12),SFYZML varchar(1),MLBM varchar(8),MLBTMC varchar(64),CJSJ DateTime,GXSJ DateTime,QYZT varchar(1),PXH integer,BZ varchar(1) default '1',MLBTMCQC varchar(256),SFJCMLTQ varchar(1) default '0',JCMLZSDBM varchar(12) default '0')";
            ExecuteSql(FolderSql);
            //创建个人本地文件资源表
            string FileSql = "create table YH_BDWJ(BD_ID integer primary key autoincrement,YHZY_ID integer default 1,YH_GUID VARCHAR(12),YZML integer(8),RTYPE varchar(1),ZYLYFS varchar(1),JMFS varchar(1),MD5 varchar(32),WJJWLDZ varchar(256),TITLE varchar(64),FORMAT_LIST varchar(64),WJDXDW varchar(12),SUBJECT varchar(32),GRADE_LIST varchar(32),VERSION varchar(32),USAGE_TYPE varchar(32),GXSJ datetime,NRJC_GUID varchar(12) default '1',QYZT varchar(1) default '1')";
            ExecuteSql(FileSql);
            //创建用户配置表
            string Sql4 = "Create TABLE YH_PZB(PZ_ID integer primary key autoincrement,YH_GUID varchar(12) NOT NULL,PZMC varchar(32),PZLX varchar(32),PZMRZ varchar(32),PZNRZ varchar(64),PZGXSJ Datetime)";
            ExecuteSql(Sql4);
        }

        public string ChangeTime(DateTime time)
        {
            string mtime = string.Empty;
            string dtime = string.Empty;
            string htime = string.Empty;
            string mitime = string.Empty;
            string setime = string.Empty;
            //转化月份
            if (time.Month.ToString().Length == 1)
            {
                 mtime = "0" + time.Month.ToString();
            }
            else
            {
                mtime = time.Month.ToString();
            }
            //转化日期
            if (time.Day.ToString().Length == 1)
            {
                dtime = "0" + time.Day.ToString();
            }
            else
            {
                dtime = time.Day.ToString();
            }
            //转化小时
            if (time.Hour.ToString().Length == 1)
            {
                htime = "0" + time.Hour.ToString();
            }
            else
            {
                htime = time.Hour.ToString();
            } 
            //转化分钟
            if (time.Minute.ToString().Length == 1)
            {
                mitime = "0" + time.Minute.ToString();
            }
            else
            {
                mitime = time.Minute.ToString();
            }
            //转化秒钟
            if (time.Second.ToString().Length == 1)
            {
                setime = "0" + time.Second.ToString();
            }
            else
            {
                setime = time.Second.ToString();
            }

            string stime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", time.Year,mtime, dtime, htime, mitime,setime);
            return stime; 
        }
    }
}
