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
        private TextBox _textBoxOutPut;
        private TextBox _textBoxInPut;
        private string _strDomainName;
        private string _message;
        bool _disposed;                //非托管资源是否回收完毕

        public NetCheck(TextBox textBoxOutPut, TextBox textBoxInPut)
        {
            _textBoxInPut = textBoxInPut;
            _strDomainName = textBoxInPut.Text.ToString().Replace("\r\n",":");//获取输入文本框的域名
            _textBoxOutPut = textBoxOutPut;               //向输出文本框输出网络检测信息
            _dnsConfigCheck = new DNSconfigCheck(_textBoxOutPut);
            _pingServerNetCheck = new PingServerNetCheck(_textBoxOutPut);
            _localNetCheck = new LocalNetCheck(_textBoxOutPut);
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
            _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]开始检测本地NDS配置..." + System.Environment.NewLine;
            _textBoxOutPut.AppendText(_message);

            if (_dnsConfigCheck.checkNDSconfig())
            {
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]本地NDS配置检测完成" + System.Environment.NewLine;
                _textBoxOutPut.AppendText(_message);
                _textBoxOutPut.AppendText(System.Environment.NewLine);
                
                _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]开始检测本地网络配置..." + System.Environment.NewLine;
                _textBoxOutPut.AppendText(_message);
                if (_localNetCheck.checkLocalNetStat())
                {
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]本地网路配置检测完成" + System.Environment.NewLine;
                    _textBoxOutPut.AppendText(_message);

                    _textBoxOutPut.AppendText(System.Environment.NewLine);

                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]开始检测到远程服务器网络情况..." + System.Environment.NewLine;
                    _textBoxOutPut.AppendText(_message);
                    _pingServerNetCheck.checkNetPingToText(_strDomainName);
                    _message = "[" + System.DateTime.UtcNow.AddHours(8) + "]到远程服务器网络情况检测完成" + System.Environment.NewLine;
                    _textBoxOutPut.AppendText(_message);
                    _textBoxOutPut.AppendText(System.Environment.NewLine);
                }
            }
        }
        
    }
}
