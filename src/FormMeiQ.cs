using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MeiQNetCheck
{
    public partial class FormMeiQ : Form
    {
        public delegate void WriteMessageToTextBoxOutPut(string message);
        public delegate void InvalidButton();
        public delegate void ValidButton();
        public WriteMessageToTextBoxOutPut _wirteMessageToTextBoxOutPut;
        public InvalidButton _invalidButton;
        public ValidButton _validButton;
        public FormMeiQ()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            _wirteMessageToTextBoxOutPut = new WriteMessageToTextBoxOutPut(showMessageOnTextBoxOutPut);
            _invalidButton = new InvalidButton(invalidButton);
            _validButton = new ValidButton(validButton);
        }

        //buttonCheck
        private void button1_Click(object sender, EventArgs e)
        {
            NetCheck nc = new NetCheck(this);
            
            //create a new thread deal user request
            Thread dealNetCheck = new Thread(new ThreadStart(nc.startCheckNet));
            dealNetCheck.IsBackground = true;
            dealNetCheck.Start();
        }

        //buttonExit
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void showMessageOnTextBoxOutPut(string message)
        {
            textBoxOutPut.AppendText(message);
        }

        public void invalidButton()
        {
            buttonCheck.Enabled = false;
        }

        public void validButton()
        {
            buttonCheck.Enabled = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;

            }
        }

        private void FromMeiQ_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 取消关闭窗体
            e.Cancel = true;

            // 将窗体变为最小化
            this.WindowState = FormWindowState.Minimized;

            //不显示在系统任务栏
            this.ShowInTaskbar = false;
            
            //托盘图标课件
            notifyIcon1.Visible = true;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
