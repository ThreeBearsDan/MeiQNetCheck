using System;
using System.Collections.Generic;
using System.Text;

namespace MeiQNetCheck
{
    class SystemInfo
    {
        //保存操作系统相关信息
        private string _message;
        //操作系统版本
        private string _osVersion;
        //用户名
        private string _userName;
        //.Net版本
        private string _dotNetVersion;

        public SystemInfo()
        {
            getOSInfo();
            getMessageOfOS();
        }

        public string OSInfo
        {
            get { return _message; }
        }

        /**
         * @brief 获取操作系统相关信息
         * @param
         * @return
         */
        public void getOSInfo()
        {
            //操作系统版本
            _osVersion = System.Environment.OSVersion.ToString();
            //用户名
            _userName = System.Environment.MachineName.ToString();
            //.net版本
            _dotNetVersion = System.Environment.Version.ToString();
        }

        /**
         * @brief 获取操作系统相关信息
         * @param
         * @return  string _message
         */
        public string getMessageOfOS()
        {
            _message = "-------------------------------------------------------------------" + System.Environment.NewLine
                + "操作系统版本: " + _osVersion + System.Environment.NewLine
                + "计算机名: " + _userName + System.Environment.NewLine
                + ".NET Framework Versions: " + _dotNetVersion + System.Environment.NewLine;
            return _message;
        }
    }
}
