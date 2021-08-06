using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Plugins
{
    public static class DateConverter
    {
        public static string Convert(DateTime value)
        {
            var formatedDateString = value.ToString("dd/MM/yyyy H:mm");
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("ka-GE");
            var formatedDate = DateTime.Parse(formatedDateString, cultureinfo);

            if (formatedDate.Date == DateTime.Today)
            {
                return string.Format("დღეს {0}", value.ToString("H:mm"));
            }
            else if (formatedDate.Date == DateTime.Today.AddDays(-1).Date)
            {
                return string.Format("გუშინ {0}", value.ToString("H:mm"));
            }
            return value.ToString("dd/MM/yyyy H:mm");
        }
    }
}
