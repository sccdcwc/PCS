using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCS.BLL;

namespace PCS.BLL
{
    class LoginBLL
    {
        SQLiteBLL sqlite = new SQLiteBLL();
        public void Login()
        {
 
        }

        public bool DLMChange(string DLM)
        {
            bool bSuccess = false;
            string sSql = string.Format("select * from jc_yh where yhdlm='{0}' and IsLast='true'", DLM);
            //if (sqlite.IsHaveInSqlite(sSql))
            //{
            //    //string sSql1 = "select yh.YHDLM,yh.DLMM,yh.ZWXM,dl.RemPass,dl.AutoLog from jc_yh yh join yh_pz pz on pz.YH_GUID=yh.YH_GUID join yh_dlpz dl on dl.DLConfigID=pz.DLConfigID where yh.IsLast='true'";
            //    //string sSql2 = "select yh.yhdlm,yh.dlmm,pz.pznrz,pz.pzmc from jc_yh yh join yh_pzb pz on pz.yh_guid=yh.yh_guid where yh.yhdlm='{0}'";
            //    //bSuccess = sqlite.IsHaveInSqlite(sSql1);
                
            //}
            bSuccess=sqlite.IsHaveInSqlite(sSql);
            return bSuccess;
        }


        public void ChangeIsLastLog(string DLM)
        {
            string sSql = string.Format("update jc_yh set IsLast='false' where IsLast='true'");
            string sSql2 = string.Format("update jc_yh set IsLast='true' where YHDLM='{0}'", DLM);
            sqlite.ExecuteSql(sSql);
            sqlite.ExecuteSql(sSql2);
 
        }
    }
}
