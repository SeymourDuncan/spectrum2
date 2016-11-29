using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coredata;

namespace coredata
{
    public static class SpmExtenstions
    {
        public static string GetPropValue(this SpmObject obj, SpmProperty prop)
        {
            var system = obj.System;
            if (system == null)
                return "";
            var propVal =
                system.PropValues.FirstOrDefault(item => item.Object.Id == obj.Id && item.Property.Id == prop.Id);
            return propVal != null ? propVal.Value : "";
        }
    }
}
