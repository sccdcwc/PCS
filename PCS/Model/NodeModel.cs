using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace PCS.Model
{
    public class NodeModel
    {
        public NodeModel()
        {
            this.Nodes =new ObservableCollection<NodeModel>();
            this.FJID = "0";
        }
        public string NodeName { get; set; }
        public string ID { get; set; }
        public string FJID { get; set; }
        public ObservableCollection<NodeModel> Nodes { get; set; }
        public string FolderPath { get; set; }
        public string changetime { get; set; }
    }
}
