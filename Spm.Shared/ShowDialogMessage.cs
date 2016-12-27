using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spm.Shared
{
    // диалоговые окна
    public enum DialogWindowTypes
    {
        DwConnectionSettings = 0, // окно настроек подключения
        DwIssueList = 1
    }

    public class ShowDialogMessage
    {
        public ShowDialogMessage(DialogWindowTypes type)
        {
            Type = type;
        }
        public DialogWindowTypes Type { get; set; }
    }
}
