using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MeiQNetCheck
{
    class CmdRunCommand : IDisposable
    {
        //执行的命令
        private string _strCommand;
        //要执行的程序
        private string _programName;
        //记录命令执行的返回结果
        private string _resultMessage;
        //Process类
        private Process _process;
        //非托管资源是否回收完毕
        bool _disposed;

        public CmdRunCommand()
        {
            _process = new Process();
        }

        ~CmdRunCommand()
        {
            Dispose(false);
        }
        
        public string ProcessName
        {
            get { return _programName; }
            set { _programName = value; }
        }

        public string CommandName
        {
            get { return _strCommand; }
            set { _strCommand = value; }
        }

        public string ResultMessage
        {
            get { return _resultMessage; }
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
                _process.Dispose();
            }
            _disposed = true;
        }

        /**
         * @brief 初始化Process相关信息
         * @param
         * @return
         */
        public void init()
        {
            //设置程序名
            _process.StartInfo.FileName = _programName;
            //设置是否使用操作系统shell启动
            _process.StartInfo.UseShellExecute = false;
            //接受来自程序的输入信息
            _process.StartInfo.RedirectStandardInput = true;
            //由调用程序获取输出信息
            _process.StartInfo.RedirectStandardOutput = true;
            //重定向到错误输出
            _process.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            _process.StartInfo.CreateNoWindow = true;
        }

        /**
         * @brief 执行命令并记录返回结果
         * @Param
         * @return
         */
        public void run()
        {
            init();
            try
            {
                _process.Start();
                //向窗口发送要执行的命令
                _process.StandardInput.WriteLine(_strCommand + "&exit");
                _process.StandardInput.AutoFlush = true;

                //获取输出信息
                _resultMessage = _process.StandardOutput.ReadToEnd();

                //等待程序执行完退出
                _process.WaitForExit();

                //释放与组件关联的所有组件
                _process.Close();
            }
            catch (Exception e)
            {
                _resultMessage = e.Message;
            }
        }

        /**
         * @brief 传递相关参数并执行命令
         * @param 
         * @return
         */
        public void run(string strProcessName, string strCommand)
        {
            ProcessName = strProcessName;
            CommandName = strCommand;
            run();
        }

    }
}
