using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public static class Converter
    {
        public static string LongitudeToSexadecimal(double longitude)
        {
            int hours = Convert.ToInt32(Math.Truncate(longitude));
            double minutes = (longitude - hours) * 60;
            int mins = Convert.ToInt32(Math.Truncate(minutes));
            double seconds = (minutes - mins) * 60;
            string str = Math.Abs(hours).ToString() + "° " + mins.ToString() + "' " + seconds.ToString("F3") + '"';
            if (hours > 0)
                str += " E";
            else
                str += " W";
            return str;
        }
        public static string LatitudeToSexadecimal(double longitude)
        {
            int hours = Convert.ToInt32(Math.Truncate(longitude));
            double minutes = (longitude - hours) * 60;
            int mins = Convert.ToInt32(Math.Truncate(minutes));
            double seconds = (minutes - mins) * 60;
            string str = Math.Abs(hours).ToString() + "° " + mins.ToString() + "' " + seconds.ToString("F3") + '"';
            if (hours > 0)
                str += " N";
            else
                str += " S";
            return str;
        }
    }
}
