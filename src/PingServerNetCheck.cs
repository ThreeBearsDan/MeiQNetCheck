using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace MeiQNetCheck
{
    /**
       * @className PingServerNetCheck
       * @brief     通过ping服务器端判断服务器端网络是否通畅
       * @date      2016.9.2
       */
    public class PingServerNetCheck : IDisposable
    {
        private Ping _checkPing = new Ping();
        private PingOptions _options = new PingOptions();
        private PingReply _replyMessage;
        private string _message;               //holds the message of ping
        private int _timeOut;                  //holds timeout 
        private int _sendICMPTimes;                    //set the times of ping,default is 4
        private bool _connect;                 //连接标识
        private string[] _domainMutiName;          //存储多个域名
        private string _domainName;         //存储单个域名
        private string _failedDomainName;     //存储失败的域名
        private int _countSendFail;             //发送ICMP回送消息失败的次数
        private FormMeiQ _formMeiQ;
        bool _disposed;                //非托管资源是否回收完毕

        public PingServerNetCheck(FormMeiQ formMeiQ)
        {
            _timeOut = 3000;
            _message = "";
            _sendICMPTimes = 4;
            _connect = false;
            _formMeiQ = formMeiQ;
            _failedDomainName = "";
        }

        ~PingServerNetCheck()
        {
            Dispose(false);
        }

        //实现Dispose()接口
        public void Dispose()
        {
            Dispose(true);
            //.NET Framework 类库
            // GC..::.SuppressFinalize 方法
            //请求系统不要调用指定对象的终结器。
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //如果已经被回收，就中断执行
            if (_disposed) return; 
            if (disposing)
            {
                _checkPing.Dispose();
            }
            _disposed = true;
        }

        /**
         * @brief 通过ping对端服务器，判断是否与对端服务器网络连通
         * @param
         * @return
         */
        public bool checkNetPing(string url)
        {
            Log.logToTextBox("[" + System.DateTime.UtcNow.AddHours(8) + "]正在检测，请稍后", _formMeiQ);

            _domainMutiName = url.Split(new char[] { ':' });

            for (int i = 0; i < _domainMutiName.Length; i++)
            {
                Log.logToTextBox("。", _formMeiQ);
                _domainName = _domainMutiName[i].Trim();
                if (_domainName == "")
                {
                    continue;
                }
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]【" + _domainName + "】：" + System.Environment.NewLine;
                _countSendFail = 0;
                //每个域名ping 4次
                for (int j = 0; j < _sendICMPTimes; j++)
                {
                    checkNetPing();
                    Thread.Sleep(500);
                    //Application.DoEvents();
                }

                if (_countSendFail == _sendICMPTimes)
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]Error : 到【" + _domainName + "】服务器的网络不通" + System.Environment.NewLine;
                    _connect = false;
                }
                else if (_countSendFail > 0 && _countSendFail < _sendICMPTimes)
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]Note : 到【" + _domainName + "】服务器网络不稳定" + System.Environment.NewLine;
                    _connect = true;
                }
                else
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]到【" + _domainName + "】服务器网络正常" + System.Environment.NewLine;
                    _connect = true;
                }
            }
            Log.logToTextBox(System.Environment.NewLine, _formMeiQ);
            return _connect;
        }

        /**
         * @brief 通过ping对端服务器，判断是否与对端服务器网络连通
         * @param
         * @return
         */
        public bool checkNetPingToText(string url)
        {
            _domainMutiName = url.Split(new char[] { ':' });

            for (int i = 0; i < _domainMutiName.Length; i++)
            {
                _domainName = _domainMutiName[i].Trim();

                if (_domainName == "")
                {
                    continue;
                }
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]【" + _domainName + "】：" + System.Environment.NewLine;
                Log.logToTextBox(_message, _formMeiQ);
                _countSendFail = 0;
                //每个域名ping 4次
                for (int j = 0; j < _sendICMPTimes; j++)
                {
                    checkNetPingToText();
                    //Thread.Sleep(900);
                }

                if (_countSendFail == _sendICMPTimes)
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]Note : 到【" + _domainName + "】服务器的网络不通" + System.Environment.NewLine;
                    _connect = false;
                    Log.logToTextBox(_message,_formMeiQ);
                }
                else if (_countSendFail > 0 && _countSendFail < _sendICMPTimes)
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]Note : 到【" + _domainName + "】服务器网络不稳定" + System.Environment.NewLine;
                    _connect = true;
                    Log.logToTextBox(_message,_formMeiQ);
                }
                else
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]到【" + _domainName + "】服务器网络正常" + System.Environment.NewLine;
                    _connect = true;
                    Log.logToTextBox(_message, _formMeiQ);
                }
            }
            return _connect;
        }

        /**
  * @brief 通过ping常用服务器判断网络连通性
  * @param
  * @return
  */
        public void checkNetPing()
        {
            _options.DontFragment = true;    //不拆分数据包

            //设置可以发送32个字节的缓冲区
            string data = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            try
            {
                //向服务器发送ICMP回送请求消息
                _replyMessage = _checkPing.Send(_domainName, _timeOut, buffer, _options);

                if (_replyMessage.Status == IPStatus.Success)
                {
                    _message = "Server IP : [" + _replyMessage.Address.ToString() + "]  "
                             + "TTL : [" + _replyMessage.Options.Ttl + "]  "
                             + "Time : [" + _replyMessage.RoundtripTime + "]" + System.Environment.NewLine;

                }
                else
                {
                    _message = "Server IP : [" + _replyMessage.Address.ToString() + "]  "
                             + "TTL : [" + _replyMessage.Options.Ttl + "]  "
                             + "Time : [" + _replyMessage.RoundtripTime + "]" + System.Environment.NewLine;
                    _countSendFail++;
                }
            }
            catch
            {
                _countSendFail++;
                if (_countSendFail == _sendICMPTimes)
                {
                    _failedDomainName += _domainName;
                    _failedDomainName += ":";
                }
            }
        }

        /**
         * @brief 通过ping常用服务器判断网络连通性
         * @param
         * @return
         */
        public void checkNetPingToText()
        {
            _options.DontFragment = true;    //不拆分数据包

            //设置可以发送32个字节的缓冲区
            string data = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            try
            {
                    //向服务器发送ICMP回送请求消息
                    _replyMessage = _checkPing.Send(_domainName, _timeOut, buffer, _options);

                    if (_replyMessage.Status == IPStatus.Success)
                    {
                        _message = "Server IP : [" + _replyMessage.Address.ToString() + "]  "
                                 + "TTL : [" + _replyMessage.Options.Ttl + "]  "
                                 + "Time : [" + _replyMessage.RoundtripTime + "]" + System.Environment.NewLine;
                        Log.logToTextBox(_message, _formMeiQ);
                        
                    }
                    else
                    {
                        _message = "Server IP : [" + _replyMessage.Address.ToString() + "]  "
                                 + "TTL : [" + _replyMessage.Options.Ttl + "]  "
                                 + "Time : [" + _replyMessage.RoundtripTime + "]" + System.Environment.NewLine;
                        Log.logToTextBox(_message, _formMeiQ);
                        _countSendFail++;
                    }
                }
            catch
            {
                _countSendFail++;
                if (_countSendFail == _sendICMPTimes)
                {
                        _failedDomainName += _domainName;
                        _failedDomainName += ":";
                }
            }
        }

        /**
         * @brief 获取Ping产生的信息
         * @param
         * @return
         */
        public string netMessage
        {
            set { _message = value; }
            get { return _message; }
        }


         /**
         * @brief 获取Ping产生的信息
         * @param
         * @return
         */
        public string FailedDomianName
        {
            get { return _failedDomainName; }
        }
    }
}
