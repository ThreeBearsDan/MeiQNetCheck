using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MeiQNetCheck
{
    //用于系统日志的输出，包括文本控件显示以及其他相关信息获取
    class Log
    {
        public static void logToTextBox(string message,FormMeiQ mfromMeiQ)
        {
            FormMeiQ formMeiQ = mfromMeiQ;
            formMeiQ.Invoke(formMeiQ._wirteMessageToTextBoxOutPut,message);
        }
    }
}
