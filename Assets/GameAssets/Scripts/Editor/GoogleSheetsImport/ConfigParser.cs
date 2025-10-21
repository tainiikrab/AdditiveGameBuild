using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace GoogleSpreadsheets
{
    public class ConfigParser<T> : ISheetParser where T : class, IConfig, new()
    {
        private const string idField = "ID";

        private readonly List<T> configs;
        private readonly Dictionary<string, MemberSetter> memberMap;
        private T currentConfig;

        public ConfigParser(GlobalConfig globalConfig)
        {
            // Находим List<T> как поле или свойство
            var listObj = FindOrCreateList(globalConfig, out var assignBack);
            if (listObj == null)
            {
                var newList = new List<T>();
                assignBack?.Invoke(newList);
                listObj = newList;
            }

            configs = (List<T>)listObj;

            // Кэшируем и поля, и свойства
            memberMap = BuildMemberMap();
        }

        public void Parse(string header, string cell)
        {
            var h = Normalize(header);
            var v = cell?.Trim() ?? string.Empty;

            // Special handling for the ID column — row start detection
            if (string.Equals(h, Normalize(idField), StringComparison.Ordinal))
            {
                if (string.IsNullOrWhiteSpace(v))
                {
                    // Empty ID cell means: stop parsing until next valid ID
                    currentConfig = null;
                    return;
                }

                // Try to find an existing config with this ID
                var existing = configs.FirstOrDefault(c => c.id == v);
                if (existing != null)
                {
                    currentConfig = existing;
                }
                else
                {
                    // Create a new config
                    currentConfig = new T();

                    // Set the ID via reflection so it works even if 'id' has a private setter
                    if (memberMap.TryGetValue(Normalize(idField), out var setter) &&
                        TryConvert(v, setter.Type, out var value))
                        setter.Set(currentConfig, value);

                    configs.Add(currentConfig);
                }

                // Done with ID column — don't process it again below
                return;
            }

            // If we don't have a current config yet, skip this cell
            if (currentConfig == null)
                return;

            // Assign any other column via reflection
            if (memberMap.TryGetValue(h, out var otherSetter) &&
                TryConvert(v, otherSetter.Type, out var otherValue))
                otherSetter.Set(currentConfig, otherValue);
        }


        private static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            Span<char> buf = stackalloc char[s.Length];
            var k = 0;
            foreach (var ch in s)
                if (char.IsLetterOrDigit(ch))
                    buf[k++] = char.ToLowerInvariant(ch);
            return new string(buf[..k]);
        }

        private Dictionary<string, MemberSetter> BuildMemberMap()
        {
            var map = new Dictionary<string, MemberSetter>(StringComparer.Ordinal);

            // Свойства
            foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanWrite) continue;
                var key1 = p.Name;
                var keyN = Normalize(key1);

                var setter = new MemberSetter(p.PropertyType, (obj, val) => p.SetValue(obj, val));
                map[key1] = setter;
                map[keyN] = setter;
            }

            // Поля
            foreach (var f in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var key1 = f.Name;
                var keyN = Normalize(key1);

                var setter = new MemberSetter(f.FieldType, (obj, val) => f.SetValue(obj, val));
                map[key1] = setter;
                map[keyN] = setter;
            }

            return map;
        }

        private static bool TryConvert(string s, Type targetType, out object result)
        {
            // Nullable<T>
            var isNullable = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
            var underlying = isNullable ? Nullable.GetUnderlyingType(targetType) : targetType;

            // Пустые значения
            if (string.IsNullOrWhiteSpace(s))
            {
                result = isNullable || underlying == typeof(string) ? null : GetDefault(underlying);
                // Возвращаем false, чтобы можно было оставить по умолчанию
                return underlying == typeof(string);
            }

            try
            {
                if (underlying == typeof(string))
                {
                    result = s;
                    return true;
                }

                if (underlying.IsEnum)
                {
                    result = Enum.Parse(underlying, s, true);
                    return true;
                }

                if (underlying == typeof(bool))
                {
                    if (bool.TryParse(s, out var b))
                    {
                        result = b;
                        return true;
                    }

                    if (s == "1" || s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                        s.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                        return true;
                    }

                    if (s == "0" || s.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                        s.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        result = false;
                        return true;
                    }
                }

                if (underlying == typeof(int))
                    if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    {
                        result = v;
                        return true;
                    }

                if (underlying == typeof(long))
                    if (long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    {
                        result = v;
                        return true;
                    }

                if (underlying == typeof(float))
                    if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    {
                        result = v;
                        return true;
                    }

                if (underlying == typeof(double))
                    if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    {
                        result = v;
                        return true;
                    }

                if (underlying == typeof(decimal))
                    if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    {
                        result = v;
                        return true;
                    }

                // Последний шанс
                result = Convert.ChangeType(s, underlying, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = GetDefault(underlying);
                return false;
            }
        }

        private static object GetDefault(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        private static object FindOrCreateList(GlobalConfig cfg, out Action<object> assignBack)
        {
            assignBack = null;

            // Пытаемся найти как поле
            var listField = typeof(GlobalConfig)
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f => f.FieldType == typeof(List<T>));

            if (listField != null)
            {
                assignBack = obj => listField.SetValue(cfg, obj);
                return listField.GetValue(cfg);
            }

            // Или как свойство
            var listProp = typeof(GlobalConfig)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(List<T>));

            if (listProp != null)
            {
                assignBack = obj => listProp.SetValue(cfg, obj);
                return listProp.GetValue(cfg);
            }

            throw new Exception($"В GlobalConfig нет списка для типа {typeof(T).Name}");
        }

        private sealed class MemberSetter
        {
            public MemberSetter(Type type, Action<object, object> set)
            {
                Type = type;
                Set = set;
            }

            public Type Type { get; }
            public Action<object, object> Set { get; }
        }
    }
}