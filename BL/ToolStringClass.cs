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
                    ("name: {0,-15} value: {1,-15}"
                    , item.Name, item.GetValue(t, null));
            }
            return result;
        }
    }
}
