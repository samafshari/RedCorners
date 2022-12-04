using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedCorners.Services
{
    public class ContainerService
    {
        public ConcurrentDictionary<Type, object> Children { get; private set; }
            = new ConcurrentDictionary<Type, object>();

        public ConcurrentDictionary<string, object> ChildrenById { get; private set; }
            = new ConcurrentDictionary<string, object>();

        public T Get<T>()
        {
            return Children.TryGetValue(typeof(T), out var vm) ? (T)vm : default;
        }

        public IEnumerable<T> GetAll<T>()
        {
            var vm = Get<T>();
            return
                new object[] { vm }
                    .Union(ChildrenById.Values.Where(x => x.GetType() == typeof(T)))
                    .Where(x => x != null)
                    .Distinct()
                    .Cast<T>();
        }

        public T Get<T>(string id)
        {
            return ChildrenById.TryGetValue(id, out var vm) ? (T)vm : default;
        }

        public object Get(Type type)
        {
            return Children.TryGetValue(type, out var vm) ? vm : default;
        }

        public void Set<T>(T vm)
        {
            Set(typeof(T), vm);
        }

        public void Set(object vm)
        {
            Set(vm.GetType(), vm);
        }

        public object Set(Type type, object vm)
        {
            return Children[type] = vm;
        }

        public object Set(string id, object vm)
        {
            if (!Children.ContainsKey(vm.GetType()))
                Set(vm);
            return ChildrenById[id] = vm;
        }

        public T GetOrCreate<T>() where T : class, new()
        {
            return GetOrCreate(() => Activator.CreateInstance<T>());
        }

        public T GetOrCreate<T>(Func<T> factory) where T : class, new()
        {
            return (T)GetOrCreate(typeof(T), () => factory());
        }

        public object GetOrCreate(Type type)
        {
            return GetOrCreate(type, () => Activator.CreateInstance(type));
        }

        public object GetOrCreate(Type type, Func<object> factory)
        {
            var vm = Get(type);
            if (vm == default)
            {
                vm = factory();
                Set(vm);
            }
            return vm;
        }

        public void ClearAll()
        {
            Children.Clear();
            ChildrenById.Clear();
        }

        public void Remove<T>()
        {
            Remove(typeof(T));
        }

        public void ClearAll<T>()
        {
            ClearAll(typeof(T));
        }

        public void Remove(Type t)
        {
            Children.TryRemove(t, out _);
        }

        public void ClearAll(Type t)
        {
            Children.TryRemove(t, out _);
            var ids = ChildrenById
                .Where(x => x.Value != null && x.GetType() == t)
                .ToList();
            foreach (var id in ids)
                Remove(id.Key);
            Remove(t);
        }

        public void Remove(string id)
        {
            if (id != default)
                ChildrenById.TryRemove(id, out _);
        }

        public void Remove(object o)
        {
            if (o != null)
            {
                var ids = ChildrenById.Where(x => x.Value == o).ToList();
                foreach (var id in ids)
                    Remove(id);
                var t = o.GetType();
                Children.TryRemove(t, out _);
            }
        }
    }
}
