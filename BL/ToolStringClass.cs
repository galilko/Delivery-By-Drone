using System;
using System.Reflection;

namespace BO
{
    public class ToolStringClass
    {
        public static string ToStringProperty<T>(T t)
        {
            string result = "";
            foreach (PropertyInfo item in t.GetType().GetProperties())
            {
                result +=(String.Format("{0,-20}:{1}\n", item.Name, item.GetValue(t, null)));
            }
            return result;
        }
    }
}
