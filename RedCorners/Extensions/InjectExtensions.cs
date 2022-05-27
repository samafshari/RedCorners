using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;

namespace RedCorners
{

    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreInjectAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ProjectAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreProjectAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ForceCacheAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ForceAvoidCacheAttribute : Attribute
    {
    }

    public enum ProjectMode
    {
        AllButIgnored,
        Explicit
    }

    public static class InjectExtensions
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo[]> properties = new ConcurrentDictionary<Type, PropertyInfo[]>();
        static readonly ConcurrentDictionary<(Type t, Type a), bool> attributes = new ConcurrentDictionary<(Type t, Type a), bool>();
        static readonly ConcurrentDictionary<(PropertyInfo p, Type a), bool> propAttributes = new ConcurrentDictionary<(PropertyInfo p, Type a), bool>();

        public static bool CacheProperties = false;

        static bool HasCustomAttributes(Type type, Type attribute)
        {
            if (!attributes.ContainsKey((type, attribute)))
                return attributes[(type, attribute)] = type.GetCustomAttributes(attribute, true).Any();
            return attributes[(type, attribute)];
        }

        static bool HasCustomAttributes(PropertyInfo prop, Type attribute)
        {
            if (!propAttributes.ContainsKey((prop, attribute)))
                return propAttributes[(prop, attribute)] = prop.GetCustomAttributes(attribute, true).Any();
            return propAttributes[(prop, attribute)];
        }

        static PropertyInfo[] GetProperties(Type type, bool? useCache)
        {
            var c = useCache ?? CacheProperties;
            if (HasCustomAttributes(type, typeof(ForceCacheAttribute)))
                c = true;
            if (HasCustomAttributes(type, typeof(ForceAvoidCacheAttribute)))
                c = false;
            if (!c || !properties.ContainsKey(type))
                return properties[type] = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return properties[type];
        }

        public static void Inject(this object me, object destination, bool? cacheSrc = null, bool? cacheDst = null, Action<Exception> exceptionHandler = null)
        {
            var destProps = GetProperties(destination.GetType(), cacheDst);
            var myProps = GetProperties(me.GetType(), cacheSrc);
            foreach (var item in myProps)
            {
                if (HasCustomAttributes(item, typeof(IgnoreInjectAttribute)))
                    continue;
                var matchingFields = from x in destProps where x.Name == item.Name select x;
                if (!matchingFields.Any()) continue;
                try
                {
                    matchingFields.First().SetValue(destination, item.GetValue(me));
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
                }
            }
        }

        public static void InjectDictionary(this IDictionary<string, object> configuration, object destination, bool? cache = null, Action<Exception> exceptionHandler = null)
        {
            var destProps = GetProperties(destination.GetType(), cache);
            foreach (var item in configuration.Keys)
            {
                var matchingFields = from x in destProps where x.Name == item select x;
                if (!matchingFields.Any()) continue;
                try
                {
                    matchingFields.First().SetValue(destination, configuration[item]);
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
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
            if (!mes.Any()) return new List<T>();
            return mes.Select(x => x.ReturnAs<T>()).ToList();
        }

        public static Dictionary<string, object> ProjectAsDictionary(this object me, ProjectMode mode = ProjectMode.AllButIgnored, bool? cache = null, Action<Exception> exceptionHandler = null)
        {
            var results = new Dictionary<string, object>();
            var myProps = GetProperties(me.GetType(), cache);
            foreach (var item in myProps)
            {
                if (item.GetCustomAttributes(typeof(IgnoreProjectAttribute), true).Any())
                    continue;
                if (mode == ProjectMode.Explicit && !item.GetCustomAttributes(typeof(ProjectAttribute), true).Any())
                    continue;

                try
                {
                    results[item.Name] = item.GetValue(me);
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
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
