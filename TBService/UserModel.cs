using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TBService
{
    class UserModel
    {
        private string m_YH_GUID;
        public string YH_GUID
        {
            set { m_YH_GUID = value; }
            get { return m_YH_GUID; }

        }

        /// <summary>
        /// 用户登录名
        /// </summary>
        private string m_YHDLM;
        public string YHDLM
        {
            set { m_YHDLM = value; }
            get { return m_YHDLM; }
        }

        /// <summary>
        /// 登录密码
        /// </summary>
        private string m_DLMM;
        public string DLMM
        {
            set { m_DLMM = value; }
            get { return m_DLMM; }
        }

        /// <summary>
        /// 中文姓名
        /// </summary>
        private string m_ZWXM;
        public string ZWXM
        {
            set { m_ZWXM = value; }
            get { return m_ZWXM; }
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        private string m_YHLX;
        public string YHLX
        {
            set { m_YHLX = value; }
            get { return m_YHLX; }
        }
    }
}
