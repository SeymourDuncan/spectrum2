using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Spm.Shared
{
    public class DialogMessage : MessageBase
    {
        public string Message { get; set; }
        public string Caption { get; set; }
        public bool IsError { get; set; }

        public DialogMessage(string message, string caption = "", bool isError = false)
        {
            Message = message;
            if (!isError)
            {
                Caption = caption;
            }
            else
            {
                Caption = "Error";
            }
        }
    }
}
