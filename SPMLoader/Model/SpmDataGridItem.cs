using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace SPMLoader.Model
{
    public class SpmDataGridItem: ObservableObject
    {
        //[DisplayName(@"L, нм.")]
        //public string LValue { get; set; }

        //[DisplayName(@"K")]
        //public string KValue { get; set; }

        [DisplayName(@"L, нм.")]
        public double LValue { get; set; }

        [DisplayName(@"K")]
        public double KValue { get; set; }
    }
}
