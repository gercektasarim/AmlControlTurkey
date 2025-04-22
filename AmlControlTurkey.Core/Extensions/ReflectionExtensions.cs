using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static async Task<object?> ConvertValue(this object? value, Type targetType)
        {
            if (value == null || targetType == typeof(string))
                return value?.ToString();

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                return value == null ? null : Convert.ChangeType(value, underlyingType);
            }

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value.ToString()!);
            }

            if (value is IConvertible)
            {
                return Convert.ChangeType(value, targetType);
            }

            return value;
        }

        public static async Task<bool> CopyValues<U, T>(this U source, T target)
        {
            Type u = typeof(U);
            Type t = typeof(T);

            var isChange = false;
            var properties = u.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            var targets = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var tar = targets.FirstOrDefault(x => x.Name == prop.Name);
                if (prop.PropertyType.IsSealed == false || prop.Name == "Id" || !targets.Any(x => x.Name == prop.Name))
                    continue;
                var value = prop.GetValue(source);
                var tvalue = tar.GetValue(target);
                if (prop.PropertyType != typeof(byte[]) && value != null)
                {
                    tar.SetValue(target, value, null);
                    isChange = true;
                }
            }
            return isChange;
        }

        public static string ConvertDates(this string text)
        {
            var pattern = @"\b(\d{4})\b";
            var matches = Regex.Matches(text, pattern);
            var years = new List<string>();
            foreach (Match match in matches)
            {
                years.Add(match.Value);
            }
            return string.Join(' ', years.ToList());
        }
    }
}
