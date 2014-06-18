using PCS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace PCS.BLL
{
    class FileBLL
    {
        /// <summary>
        /// 向fileinfo中写数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<FileInfomation> WriteFileInfo(DataTable dt)
        {
            List<FileInfomation> files = new List<FileInfomation>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FileInfomation file = new FileInfomation();
                    file.YH_GUID = dt.Rows[i]["YH_GUID"].ToString();
                    file.FORMAT_LIST = dt.Rows[i]["FORMAT_LIST"].ToString();
                    file.GRADE_LIST = dt.Rows[i]["GRADE_LIST"].ToString();
                    file.GXSJ = dt.Rows[i]["GXSJ"].ToString();
                    file.JMFS = dt.Rows[i]["JMFS"].ToString();
                    file.MD5 = dt.Rows[i]["MD5"].ToString();
                    file.RTYPE = dt.Rows[i]["RTYPE"].ToString();
                    file.SUBJECT = dt.Rows[i]["SUBJECT"].ToString();
                    file.TITLE = dt.Rows[i]["TITLE"].ToString();
                    file.USAGE_TYPE = dt.Rows[i]["USAGE_TYPE"].ToString();
                    file.VERSION = dt.Rows[i]["VERSION"].ToString();
                    file.WJDXDW = dt.Rows[i]["WJDXDW"].ToString();
                    file.WJJWLDZ = dt.Rows[i]["WJJWLDZ"].ToString();
                    file.YHZY_ID = dt.Rows[i]["YHZY_ID"].ToString();
                    file.YZML = dt.Rows[i]["YZML"].ToString();
                    files.Add(file);
                }
            }

            return files;
        }
        /// <summary>
        /// 获取本地文件信息
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="yh_guid"></param>
        /// <returns>文件信息列表</returns>
        public List<FileInfomation> GetFileInfo(NodeModel Node, string yh_guid)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            List<FileInfomation> FileList = new List<FileInfomation>();
            List<FileInfomation> Files = new List<FileInfomation>();
            if (Node != null)
            {
                DataTable dt = new DataTable();
                string sSql = string.Format("select * from yh_bdml where yh_guid='{0}' and ml_id='{1}'", yh_guid, Node.ID);
                dt = sqlite.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    string sMlbtqc = dt.Rows[0]["MLBTMCQC"].ToString();
                    List<string> MLMCArray = FJMLBTQC(sMlbtqc);
                    string sSql1 = string.Format("select * from yh_bdwj where BQMLMC like '%-1%'");
                    for (int i = 0; i < MLMCArray.Count; i++)
                    {
                        if (MLMCArray[i] == "课程目录")
                            sSql1 = sSql1 + " and YZML>'100000' and yzml<'200000'";
                        else
                            sSql1 = sSql1 + string.Format(" and BQMLMC like '%{0}%'", MLMCArray[i]);
                    }
                    DataTable dt1 = sqlite.GetTable(sSql1);
                    Files = WriteFileInfo(dt1);
                    FileList = FileList.Union(Files).ToList<FileInfomation>();
                    //if (Node.Nodes != null)
                    //{
                    //    for (int i = 0; i < Node.Nodes.Count; i++)
                    //    {
                    //        Files = GetFileInfo(Node.Nodes[i], yh_guid);
                    //        FileList = FileList.Union(Files).ToList<FileInfomation>();
                    //    }
                    //}
                }
            }
            return FileList;
        }

        /// <summary>
        /// 根据本地目录信息创建本地文件夹
        /// </summary>
        /// <param name="Folders"></param>
        /// <param name="sPath"></param>
        public void CreateLocalFolder(ObservableCollection<NodeModel> Folders, string sPath)
        {
            string sTempPath = sPath;
            for (int i = 0; i < Folders.Count; i++)
            {
                sTempPath = sPath + "//" + Folders[i].NodeName;
                if (!Directory.Exists(sTempPath))
                {
                    Directory.CreateDirectory(sTempPath);
                }
                CreateLocalFolder(Folders[i].Nodes, sTempPath);
            }
        }
        /// <summary>
        /// 获取该节点的本地地址
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="sPath"></param>
        /// <param name="user"></param>
        /// <returns>节点完整本地地址为：根地址+"//"+返回值</returns>
        public string WritePath(NodeModel Node, string sPath, UserModel user)
        {
            SQLiteBLL sqlite = new SQLiteBLL();
            NodeBLL NB = new NodeBLL();
            NodeModel NewNode = NB.GetFatherNode(Node, user);
            if (Node != null)
            {
                if (Node.FJID == "0")
                {
                    sPath = Node.NodeName + "//" + sPath;
                }
                else
                {
                    sPath = Node.NodeName + "//" + sPath;
                    sPath = WritePath(NewNode, sPath, user);
                }
            }
            return sPath;
        }

        /// <summary>
        /// 获取目录下所有文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<FileInfo> ListFiles(string path)
        {
            List<FileInfo> fileslist = new List<FileInfo>();
            FileSystemInfo info = new DirectoryInfo(path);
            if (!info.Exists)
                return null;
            DirectoryInfo dir = info as DirectoryInfo;
            if (dir == null)
                return null;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                if (file != null)
                {
                    fileslist.Add(file);
                }
            }
            return fileslist;
        }
        /// <summary>
        /// 分解目录标题名称全称
        /// </summary>
        /// <param name="MLBTQC"></param>
        /// <returns></returns>
        public List<string> FJMLBTQC(string MLBTQC)
        {
            char[] Array = MLBTQC.Trim().ToArray();
            int i = 0;
            List<string> BTMC = new List<string>();
            string temp = string.Empty;
            foreach (char a in Array)
            {

                if (a != '/')
                {
                    temp = temp + a;
                }
                else
                {
                    BTMC.Add(temp);
                    temp = string.Empty;
                }
            }
            BTMC.Add(temp);
            return BTMC;
        }
    }


}
