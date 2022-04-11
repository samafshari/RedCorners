using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace RedCorners
{
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreInject : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class Export : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreExport : Attribute
    {
    }

    public enum ExportMode
    {
        AllButIgnored,
        Exportables
    }

    public static class InjectExtensions
    {
        public static void Inject(this object me, object destination)
        {
            var destProps = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var myProps = me.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in myProps)
            {
                if (item.GetCustomAttributes(typeof(IgnoreInject), true).Any())
                    continue;
                var matchingFields = from x in destProps where x.Name == item.Name select x;
                if (matchingFields.Count() == 0) continue;
                try
                {
                    matchingFields.First().SetValue(destination, item.GetValue(me));
                }
                catch (Exception)
                {

                }
            }
        }

        public static void InjectDictionary(this IDictionary<string, object> configuration, object destination)
        {
            var destProps = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in configuration.Keys)
            {
                var matchingFields = from x in destProps where x.Name == item select x;
                if (matchingFields.Count() == 0) continue;
                try
                {
                    matchingFields.First().SetValue(destination, configuration[item]);
                }
                catch (Exception)
                {
                }
            }
        }

        public static T ReturnAs<T>(this object me) where T : new()
        {
            if (me == null) return default;
            T instance = new T();
            me.Inject(instance);
            return instance;
        }

        public static List<T> ReturnAsList<T>(this IEnumerable<object> mes) where T : new()
        {
            if (mes == null) return null;
            if (mes.Count() == 0) return new List<T>();
            return mes.Select(x => x.ReturnAs<T>()).ToList();
        }

        public static Dictionary<string, object> Export(this object me, ExportMode mode)
        {
            var results = new Dictionary<string, object>();
            var myProps = me.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in myProps)
            {
                if (item.GetCustomAttributes(typeof(IgnoreExport), true).Any())
                    continue;
                if (mode == ExportMode.Exportables && !item.GetCustomAttributes(typeof(Export), true).Any())
                    continue;

                try
                {
                    results[item.Name] = item.GetValue(me);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }

        public static Dictionary<string, string> ToString(this DateTime date, string prefix, params string[] formats)
        {
            var results = new Dictionary<string, string>();
            prefix = prefix ?? string.Empty;
            foreach (var format in formats)
                results[$"{prefix}{format}"] = date.ToString(format);
            return results;
        }

        public static Dictionary<string, string> ToString(this DateTimeOffset date, string prefix, params string[] formats)
        {
            var results = new Dictionary<string, string>();
            prefix = prefix ?? string.Empty;
            foreach (var format in formats)
                results[$"{prefix}{format}"] = date.ToString(format);
            return results;
        }
    }

    public class DTO { }
}
