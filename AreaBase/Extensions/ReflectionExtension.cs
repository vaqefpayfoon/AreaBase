using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AreaBase.Extensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            //GetValue item and optional index value whitch is null
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
