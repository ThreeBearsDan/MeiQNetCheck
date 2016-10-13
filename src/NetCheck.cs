using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MeiQNetCheck
{
    class NetCheck : IDisposable
    {
        private DNSconfigCheck _dnsConfigCheck;
        private PingServerNetCheck _pingServerNetCheck;
        private LocalNetCheck _localNetCheck;
        //private TextBox _textBoxInPut;
        private string _strDomainName;
        private string _message;
        private string _mailMessage;
        private MailToMeiQ _mailSend;
        private CmdRunCommand _cmdRunCommand;
        //非托管资源是否回收完毕
        bool _disposed;                
        private FormMeiQ _formMeiQ;
        private SystemInfo _systemInfo;


        public NetCheck(FormMeiQ formMeiQ)
        {
            _formMeiQ = formMeiQ;
            //_textBoxInPut = textBoxInPut;
            //_strDomainName = textBoxInPut.Text.ToString().Replace("\r\n",":");//获取输入文本框的域名
            _strDomainName = "meiqia.com:"+
                            "app.meiqia.com:"+
                            "eco-push-api-agent.meiqia.com:" +
                            "static.meiqia.com:" +
                            "app-cdn-s0.b0.upaiyun.com:" +
                            "app-s3-cdn.b0.upaiyun.com";
            _dnsConfigCheck = new DNSconfigCheck(_formMeiQ);
            _pingServerNetCheck = new PingServerNetCheck(_formMeiQ);
            _localNetCheck = new LocalNetCheck(_formMeiQ);
            _mailSend = new MailToMeiQ();
            _cmdRunCommand = new CmdRunCommand();
            _systemInfo = new SystemInfo();
        }

        ~NetCheck()
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
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                _pingServerNetCheck.Dispose();
                _mailSend.Dispose();
                _cmdRunCommand.Dispose();
                _localNetCheck.Dispose();
            }
            _disposed = true;
        }

        /**
         * @brief  进行网络检测
         * @param
         * @return
         */
        public void startCheckNet()
        {
            invalidButton();
            _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]" + "启动检测" + System.Environment.NewLine+System.Environment.NewLine;

            _message += "[" + System.DateTime.UtcNow.AddHours(8) + "]开始检测本地NDS配置..." + System.Environment.NewLine;
            Log.logToTextBox(_message, _formMeiQ);

            if (_dnsConfigCheck.checkNDSconfig())
            {

                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]本地NDS配置检测完成" + System.Environment.NewLine;
                Log.logToTextBox(_message + System.Environment.NewLine, _formMeiQ);

                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]开始检测本地网络配置..." + System.Environment.NewLine;
                Log.logToTextBox(_message, _formMeiQ);
                if (_localNetCheck.checkLocalNetStat())
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]本地网路配置检测完成" + System.Environment.NewLine;
                    Log.logToTextBox(_message + System.Environment.NewLine, _formMeiQ);

                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]开始检测到远程服务器网络情况..." + System.Environment.NewLine;

                    Log.logToTextBox(_message, _formMeiQ);

                    //ping 美洽服务器
                    if (_pingServerNetCheck.checkNetPing(_strDomainName))
                    {
                        _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]【到美洽远程服务器网络正常】" + System.Environment.NewLine;
                        _message += "[" + System.DateTime.UtcNow.AddHours(8) + "]到美洽远程服务器网络情况检测完成" + System.Environment.NewLine;

                        Log.logToTextBox(_message + System.Environment.NewLine, _formMeiQ);
                        _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]" + "网络检测结束" + System.Environment.NewLine;
                        _message += "[" + System.DateTime.UtcNow.AddHours(8) + "]" + "（如果您使用浏览器访问仍然存在页面不正常的情况，请尝试清理浏览器缓存及cookie）"
                                        + System.Environment.NewLine;
                        _message += "______________________________________________________"
                                    + System.Environment.NewLine + System.Environment.NewLine;
                        Log.logToTextBox(_message, _formMeiQ);
                    }
                    else
                    {
                        //到美洽服务器的相关信息 包括系统，dns，ping, 路由相关信息
                        getRemoteServerCheckToMail();

                        //mail to us
                        _mailSend.MailMessageBody = _mailMessage;
                        _mailSend.AsyncSendMail();

                        _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]【到美洽远程服务器网络异常】" + System.Environment.NewLine;
                        _message += "[" + System.DateTime.UtcNow.AddHours(8) + "]到美洽远程服务器网络情况检测完成" + System.Environment.NewLine;

                        Log.logToTextBox(_message + System.Environment.NewLine, _formMeiQ);
                        _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]" + "网络检测结束" + System.Environment.NewLine;
                        _message += "______________________________________________________"
                                    + System.Environment.NewLine + System.Environment.NewLine;
                        Log.logToTextBox(_message, _formMeiQ);
                    }

                }
                else
                {

                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]" + "网络检测结束" + System.Environment.NewLine;
                    _message += "______________________________________________________"
                                + System.Environment.NewLine + System.Environment.NewLine;
                    Log.logToTextBox(_message, _formMeiQ);
                }
            }
            else
            {
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]" + "网络检测结束" + System.Environment.NewLine;
                _message += "______________________________________________________"
                            + System.Environment.NewLine + System.Environment.NewLine;
                Log.logToTextBox(_message, _formMeiQ);
            }

            _mailMessage += "-------------------------------------------------";
            Dispose();
            validButton();
        }

        /**
         * @brief 让界面上的[开始检测]按钮失效
         * @param 
         * @return
         */
        public void invalidButton()
        {
            _formMeiQ.Invoke(_formMeiQ._invalidButton);
        }

        /**
         * @brief 让界面上的[开始检测]按钮生效
         * @param
         * @return
         */
        public void validButton()
        {
            _formMeiQ.Invoke(_formMeiQ._validButton);
        }

        /**
         * @brief 获取邮件发送的DNS信息,包括系统信息
         * @param
         * @return
         */
        public void getDNSMessageCheckToMail()
        {
            //dns配置信息mail to me
            _mailMessage = "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[操作系统信息]" + System.Environment.NewLine;
            _mailMessage += _systemInfo.OSInfo;

            //设备名称
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[设备名]" + System.Environment.NewLine;

            //用户IP
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[用户IP]" + System.Environment.NewLine;
            _mailMessage += "IP：" + DNSconfigCheck.GetLocalIPAddress() + System.Environment.NewLine;

            //DNS配置
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[DNS信息]" + System.Environment.NewLine;
            _cmdRunCommand.run("cmd.exe", "nslookup host");
            _mailMessage += _cmdRunCommand.ResultMessage;

            //运营商
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[运营商]" + System.Environment.NewLine;
        }

        /**
         * @brief 获取本地网络一个情况,系统信息，DNS信息，Ping信息
         * @param
         * @return
         */
        public void getLocalNetMessageCheckToMail()
        {
            getDNSMessageCheckToMail();
            //ping   www.qq.com
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[Ping信息]" + System.Environment.NewLine;
            _cmdRunCommand.run("cmd.exe", "ping www.qq.com");
            _mailMessage += _cmdRunCommand.ResultMessage;

            //add 20160913
            /*
            _cmdRunCommand.run("cmd.exe", "tracert www.qq.com");
            _mailMessage += _cmdRunCommand.ResultMessage;
             * */
        }

        /**
         * @brief 获取到远程服务器的信息，包括系统信息，NDS信息，Ping信息
         * @param
         * @return
         */
        public void getRemoteServerCheckToMail()
        {
            getLocalNetMessageCheckToMail();

            //到远程服务器
            //_mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            //_mailMessage += "【到美洽远程服务器的信息：】" + System.Environment.NewLine;
            string[] domainName = _pingServerNetCheck.FailedDomianName.Split(new char[] { ':' });
            
            for (var i = 0; i < domainName.Length - 1; i++)
            {
                //modify 20160913
                _cmdRunCommand.run("cmd.exe", "ping " + domainName[i] + " -n 2");
                _mailMessage += _cmdRunCommand.ResultMessage;
            }

            //_mailMessage += "-----------------------------------------------";


            //网络类型
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[网络类型]" + System.Environment.NewLine;

            //traceroute信息
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;
            _mailMessage += "[traceroute信息]" + System.Environment.NewLine;
            for (var i = 0; i < domainName.Length - 1; i++)
            {
                _cmdRunCommand.run("cmd.exe", "tracert -d -w 1500 -h 32 " + domainName[i]);
                _mailMessage += _cmdRunCommand.ResultMessage;
            }
            _mailMessage += "-----------------------------------------------" + System.Environment.NewLine;

            /*
            _cmdRunCommand.run("cmd.exe", "tracert www.meiqia.com");
            _mailMessage += _cmdRunCommand.ResultMessage;
             * */
        }
    }
}
