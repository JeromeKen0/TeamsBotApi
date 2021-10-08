using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TeamsApiBot
{
    public static class Helper
    {
        public static string QueryString<T>(T t)
        {
            StringBuilder sb = new StringBuilder();
            Type type = t.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (var prop in props)
            {
                sb.Append($"{prop.Name}={HttpUtility.UrlEncode(prop.GetValue(t)?.ToString())}&");
            }
            return sb.ToString().TrimEnd('&');
        }
    }
}
