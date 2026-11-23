using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EventFlow.Application.Common
{
    public static class EnumHelper
    {
        /// <summary>
        /// Возвращает значение атрибута [Description] у члена enum.
        /// Если атрибут отсутствует — возвращает строковое представление члена.
        /// </summary>
        public static string GetDescription(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var attribute = member?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// Возвращает словарь, где ключ — имя члена enum, значение — описание из [Description].
        /// </summary>
        public static Dictionary<string, string> GetEnumDictionary<TEnum>() where TEnum : struct, Enum
            => Enum.GetValues<TEnum>().ToDictionary(e => e.ToString(), e => GetDescription(e));
    }
}
