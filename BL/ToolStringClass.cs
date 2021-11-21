using System;
using System.Reflection;

namespace IBL.BO
{
    public class ToolStringClass
    {
        public static string ToStringProperty<T>(T t)
        {
            string result = "";
            foreach (PropertyInfo item in t.GetType().GetProperties())
            {
                result +=
                    ($"{item.Name,-15} : {item.GetValue(t, null), -15}\n"
                     );
            }
            return result;
        }
    }
}
