﻿using OracleDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TBService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class Service1 : IService1
    {

        public void ExcuteSql(string sSql)
        {
            OracleBLL ob = new OracleBLL();
            ob.ExuceSql(sSql);
        }

        public bool IsHavePZ(string YH_GUID)
        {
            bool bSuccess = false;
            OracleBLL ob = new OracleBLL();
            string sSql = string.Format("select * from yh_pzb where yh_guid={0}",YH_GUID);
            DataTable dt = new DataTable();
            dt = ob.GetTable(sSql);
            if (dt.Rows.Count > 0)
            {
                bSuccess = true;
            }
            return bSuccess;
        }
    }
}
