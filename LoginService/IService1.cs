using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace LoginService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        bool IsUserHave(string sUserName, string sPassword);

        [OperationContract]
        MemoryStream GetTable(string sSql);

        [OperationContract]
        void ExcuteSql(string sSql);

        [OperationContract]
        bool IsHavePZ(string YH_GUID);
        
    }
}
