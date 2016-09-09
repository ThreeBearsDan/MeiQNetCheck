using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;

namespace MeiQNetCheck
{
    public class DNSconfigCheck
    {
        private string _message;       //holds the message of DNS Net Check
        private string _domainName;    //host name that need to parse,default is www.qq.com
        private FormMeiQ _formMeiQ;
        public DNSconfigCheck(FormMeiQ formMeiQ)
        {
            _domainName = "www.qq.com";
            _formMeiQ = formMeiQ;
        }

        /**
         * @brief check DNS Server is normal or not
         * @param 
         * @return ture normal    false unnromal
         */
        public bool checkNDSconfig()
        {
            IPAddress[] ips;
            try
            {
                ips = Dns.GetHostAddresses(_domainName);
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]【NDS 配置正常】" + System.Environment.NewLine;
                Log.logToTextBox(_message,_formMeiQ);
                return true;
            }
            catch
            {
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]【Error: NDS 配置异常，请检查本地DNS配置和网络连接情况】" + System.Environment.NewLine;
                Log.logToTextBox(_message,_formMeiQ);
                return false;
            }
        }
    }
}
