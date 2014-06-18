using PCS.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PCS.BLL
{
    class NodeBLL
    {
        SQLiteBLL sqlite = new SQLiteBLL();
        public ObservableCollection<NodeModel> WriteNode(DataTable dt)
        {
            ObservableCollection<NodeModel> itemList = new ObservableCollection<NodeModel>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NodeModel Node = new NodeModel();
                    Node.NodeName = dt.Rows[i]["MLBTMC"].ToString().Replace("（","(").Replace("）",")");
                    Node.ID = dt.Rows[i]["YH_ML_ID"].ToString();
                    Node.FJID = dt.Rows[i]["FJMLID"].ToString();
                    Node.changetime = dt.Rows[i]["GXSJ"].ToString();
                    DataTable zdt = new DataTable();
                    string sSql = string.Format("select * from yh_bdml where FJMLID='{0}' and yh_guid='{1}' and qyzt='1' order by YH_ML_ID", dt.Rows[i]["MLBM"], dt.Rows[i]["YH_GUID"]);
                    zdt = sqlite.GetTable(sSql);
                    if (zdt.Rows.Count > 0)
                    {
                        Node.Nodes = WriteNode(zdt);

                    }
                    else
                    {
                        if (dt.Rows[i]["SFJCMLTQ"].ToString() == "1")
                        {
                            string sSql1 = string.Format("select * from yh_bdml where FJMLBM='{0}' order by MLBM", dt.Rows[i]["JCMLZSDBM"]);
                            DataTable dt1 = sqlite.GetTable(sSql1);
                            Node.Nodes = WriteNode(dt1);
                        }
                        else
                        {
                            string sSql1 = string.Format("select * from yh_bdml where FJMLBM='{0}' order by MLBM", dt.Rows[i]["MLBM"]);
                            DataTable dt1 = sqlite.GetTable(sSql1);
                            Node.Nodes = WriteNode(dt1);
                        }
                    }
                    itemList.Add(Node);
                }
            }
            return itemList;
        }

        public NodeModel FindDownward(ObservableCollection<NodeModel> nodes, string id)
        {
            if (nodes == null) return null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == id)
                    return nodes[i];
                NodeModel Node = FindDownward(nodes[i].Nodes, id);
                if (Node != null)
                    return Node;
            }
            return null;
        }

        public void NodeLoad(TreeView treev, ObservableCollection<NodeModel> itemList)
        {
            treev.Items.Clear();
            treev.ItemsSource = itemList;
        }

        public void DeleteNode(NodeModel Node, UserModel user)
        {
            string sSql = string.Format("update yh_bdml set qyzt='0',GXSJ=(select datetime('now','localtime')) where yh_guid='{0}' and mlbm='{1}'", user.YH_GUID, Node.ID);
            sqlite.ExecuteSql(sSql);
        }

        public NodeModel GetFatherNode(NodeModel Node, UserModel user)
        {
            string sql = string.Format("select * from yh_bdml where yh_guid='{0}' and MLBM='{1}'", user.YH_GUID, Node.FJID);
            DataTable dt = sqlite.GetTable(sql);
            NodeModel NewNode = new NodeModel();
            if (dt.Rows.Count > 0)
            {
                NewNode.NodeName = dt.Rows[0]["MLBTMC"].ToString();
                NewNode.ID = dt.Rows[0]["MLBM"].ToString();
                NewNode.FJID = dt.Rows[0]["FJMLBM"].ToString();
                NewNode.changetime = dt.Rows[0]["GXSJ"].ToString();
            }
            else
                NewNode = null;
            return NewNode;
        }
    }
}
