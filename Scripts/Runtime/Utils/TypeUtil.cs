using System;

namespace GameFramework.Runtime.Utils
{
    public static class TypeUtil
    {
        public static T CreateTypeInstance<T>(string typeName) where T : class
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));
            Type type = FindType(typeName);
            if (type == null || !typeof(T).IsAssignableFrom(type))
            {
              throw new InvalidOperationException($"Can not Create Type Instance {typeName} for type {typeof(T).Name}");
            }
            return (T)Activator.CreateInstance(type);
        }

        private static Type FindType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                type = assemblies[i].GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}