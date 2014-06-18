using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OracleDB
{
    public class OracleBLL:IDisposable
    {
        private OracleConnection ocnn = new OracleConnection();
        public OracleBLL(string sDataSource = "HaiWen246", string sUser = "HW", string sPassword = "1")
        {
            string sConnStr = string.Format("Data Source={0};PERSIST SECURITY INFO=True;user={1};password={2};Pooling=true;Max Pool Size=5;Min Pool Size=5", sDataSource, sUser, sPassword);
            ocnn = new OracleConnection(sConnStr);
            ocnn.Open();
        }


        public DataTable GetTable(string sSql)
        {      
            DataTable dt = new DataTable();
            try
            {
               // ocnn.Open();
                OracleCommand cmd = ocnn.CreateCommand();
                cmd.CommandText = sSql;
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                oda.Fill(dt);
                //ocnn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
                  
            }
        }

        public void ExuceSql(string sSql)
        {
            try
            {
                //ocnn.Open();
                OracleCommand cmd = ocnn.CreateCommand();
                cmd.CommandText = sSql;
                cmd.ExecuteNonQuery();
               // ocnn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ///// <summary>
        ///// 执行存储过程
        ///// </summary>
        ///// <param name="ProcdureName">存储过程名称</param>
        ///// <param name="Params">传入参数</param>
        ///// <returns>返回成功与否</returns>
        //public bool ExecuteProcdure(string ProcdureName, params Parameter[] Params)
        //{
        //    bool bSuccess = false;
        //    try
        //    {
        //        OracleCommand Sqlcmd = new OracleCommand();
        //        Sqlcmd.Connection = ocnn;
        //        Sqlcmd.CommandText = ProcdureName;
        //        Sqlcmd.CommandType = CommandType.StoredProcedure;
        //        //注意Result必须放在第一位，才能正确返回值
        //        foreach (Parameter item in Params)
        //        {
        //            OracleParameter NewParameter = new OracleParameter();
        //            NewParameter.ParameterName = item.ParameterName;
        //            NewParameter.DbType = item.DataType;
        //            NewParameter.Direction = item.Direction;
        //            if (item.Direction != ParameterDirection.Output)
        //            {
        //                NewParameter.Value = item.ParameterValue;
        //            }

        //            Sqlcmd.Parameters.Add(NewParameter);

        //            if (item.DataSize > 0)
        //            {
        //                Sqlcmd.Parameters[item.ParameterName].Size = item.DataSize;
        //            }

        //        }

        //        Sqlcmd.Prepare();
        //        Sqlcmd.ExecuteNonQuery();
        //        bSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return bSuccess;
        //}

        /// <summary>
        /// 执行函数方法
        /// </summary>
        /// <param name="FunctionName">函数名称</param>
        /// <param name="ResultDataType">返回结果数据类型</param>
        /// <param name="Params">传入参数</param>
        /// <returns>返回函数结果</returns>
        public  object ExecuteFuntion(string FunctionName,DbType ResultDataType,params Parameter[] Params)
        {
            object oValue = null;
            try
            {
                OracleCommand Sqlcmd = new OracleCommand();
                Sqlcmd.Connection = ocnn;
                Sqlcmd.CommandText = FunctionName;
                Sqlcmd.CommandType = CommandType.StoredProcedure;

                //注意Result必须放在第一位，才能正确返回值
                OracleParameter oResult = new OracleParameter();
                oResult.ParameterName = "RESULT";
                oResult.Direction = ParameterDirection.ReturnValue;
                oResult.DbType = ResultDataType;
                Sqlcmd.Parameters.Add(oResult);
                foreach (Parameter item in Params)
                {
                    OracleParameter NewParameter = new OracleParameter();
                    NewParameter.ParameterName = item.ParameterName;
                    NewParameter.DbType = item.DataType;
                    NewParameter.Direction = item.Direction;
                    NewParameter.Value = item.ParameterValue;
                    Sqlcmd.Parameters.Add(NewParameter);
                }

                Sqlcmd.Prepare();
                Sqlcmd.ExecuteNonQuery();
                oValue = oResult.Value;

            }
            catch (Exception ex)
            {
              
            }
            return oValue;
        }

        /// <summary>
        /// 获取种子表
        /// </summary>
        /// <param name="sZZLX"></param>
        /// <returns></returns>
        public string GetSeed(string sZZLX)
        {

            string ReSeed = string.Empty;
            string SpName = string.Empty;
            SpName = "FN_GETSEED";
            Parameter[] DBParams = new Parameter[1];

            Parameter pZZLX = new Parameter();
            pZZLX.DataType = DbType.String;
            pZZLX.ParameterName = "sZZLX";
            pZZLX.ParameterValue = sZZLX;
            pZZLX.Direction = ParameterDirection.Input;
            DBParams[0] = pZZLX;
            object oResult = ExecuteFuntion(SpName, DbType.Decimal, DBParams);
            ReSeed = oResult.ToString();
            return ReSeed;

        }


        public void Dispose()
        {
            ocnn.Dispose();
        }
    }

    /// <summary>
    /// 参数类
    /// </summary>
    public class Parameter
    {
        public Parameter()
        {

        }
        public Parameter(string ParamName, object oValue, DbType inDataType, ParameterDirection inDirection)
        {
            this.ParameterName = ParamName;
            this.DataType = inDataType;
            this.ParameterValue = oValue;
            this.Direction = inDirection;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object ParameterValue { get; set; }
        /// <summary>
        /// 参数据类型
        /// </summary>
        public DbType DataType { get; set; }
        /// <summary>
        /// 参数据输入输出方向
        /// </summary>
        public ParameterDirection Direction { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
        public int DataSize { get; set; }
    }
}
