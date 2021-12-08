﻿using System;
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
                result +=(String.Format("{0,-30}:{1, -15}\n", item.Name, item.GetValue(t, null)));
            }
            return result;
        }
    }
}
