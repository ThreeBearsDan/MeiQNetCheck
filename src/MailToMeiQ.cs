using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace MeiQNetCheck
{
    //MailAddress非托管资源需要手动释放，目前还尚未释放
    class MailToMeiQ : IDisposable
    {
        //邮件发送者
        private MailAddress _mailSender;

        //邮件接受者
        private MailAddress _mailReciver;

        //邮件包括邮件标题、邮件正文
        private MailMessage _message;

        //邮件正文
        private string _mailMessageBody;

        //SMTP服务器
        private string _smtpServer;

        //SMTP服务器端口
        //private int _port;

        //SMTP实例,发送邮件
        private SmtpClient _smtpClient;

        //验证发件人身份
        private NetworkCredential _netWorkCredential;

        //发件人用户名
        private string _userName;

        //发件人密码
        private string _passWord;

        //非托管资源是否回收完毕
        bool _disposed;               

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
                try
                {
                    _message.Dispose();
                }
                catch
                {
                    //do nothing
                }
            }
            _disposed = true;
        }

        public MailToMeiQ()
        {

        }

        ~MailToMeiQ()
        {
            Dispose(false);
        }

        public string MailMessageBody
        {
            set { _mailMessageBody = value; }
        }

        /**
         * @brief 初始化一些与邮件相关的信息
         * @param
         * @return
         */
        public void init()
        {
            _mailSender = new MailAddress("452260570qq@sina.com","NetCheck");
            _mailReciver = new MailAddress("wangdan@meiqia.com");
            _smtpServer = "smtp.sina.com";
            _message = new MailMessage(_mailSender, _mailReciver);
            _smtpClient = new SmtpClient(_smtpServer);

            _userName = "452260570qq@sina.com";
            _passWord = "*********";
            _netWorkCredential = new NetworkCredential(_userName, _passWord);

            _smtpClient.Host = _smtpServer;
            _smtpClient.EnableSsl = true;
            _smtpClient.Credentials = _netWorkCredential;

            //邮件标题
            _message.Subject = "MeiQNetCheck Log";
            _message.Body = _mailMessageBody;
        }

        /**
         * @brief 异步发送邮件,不管发送是否成功,不需要回调
         * @param
         * @return
         */
        public void AsyncSendMail()
        {
            init();
            try
            {
                _smtpClient.SendAsync(_message, "test");
            }
            catch
            {
                
            }
        }

    }
}
