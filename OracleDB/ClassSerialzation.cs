using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace OracleDB
{
    public class ClassSerialzation
    {
        public MemoryStream SerizalizeBinary(object request)
        {
            BinaryFormatter serizalizer = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();           
            serizalizer.Serialize(memStream, request);
            return memStream;
        }


        public object DeSerializeBinary(MemoryStream memStream)
        {
            memStream.Position = 0;
            BinaryFormatter deserializer = new BinaryFormatter();
            object newobj = deserializer.Deserialize(memStream); //将二进制流反序列化为对象
            memStream.Close();                                   //关闭内存流，并释放
            return newobj;
        }
    }
}
