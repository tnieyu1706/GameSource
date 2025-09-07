using System;

    public static class TypeIdentifySupport
    {
        public static Type ResolveType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            // Thử cách trực tiếp
            var type = Type.GetType(typeName);
            if (type != null)
                return type;

            // Nếu fail -> quét toàn bộ assemblies
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(typeName);
                if (t != null)
                    return t;
            }

            return null;
        }
    }