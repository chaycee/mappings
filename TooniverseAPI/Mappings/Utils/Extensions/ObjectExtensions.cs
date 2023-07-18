namespace TooniverseAPI.Mappings.Utils.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ObjectExtensions
{
    public static void RemoveStringArrayDuplicates<T>(this T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();

        foreach (var property in properties)
            if (property.PropertyType == typeof(string[]))
            {
                string[] array = (string[])property.GetValue(obj);
                if (array != null)
                {
                    string[] uniqueArray = array.Distinct().ToArray();
                    property.SetValue(obj, uniqueArray);
                }
            }
    }
}