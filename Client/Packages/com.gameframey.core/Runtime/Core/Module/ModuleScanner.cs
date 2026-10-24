using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameFrameY.Core.Module
{
    /// <summary>
    /// 模块自动发现：扫描程序集中带 <see cref="GameModuleAttribute"/> 且实现 <see cref="IGameModule"/> 的具体类型。
    /// 默认扫描 AppDomain 中排除系统/Unity 内置前缀的程序集；也可显式传入程序集（热更程序集加载后二次装配）。
    /// </summary>
    public static class ModuleScanner
    {
        private static readonly string[] ExcludedAssemblyPrefixes =
        {
            "System", "mscorlib", "netstandard", "Mono", "Microsoft",
            "Unity", "UnityEngine", "UnityEditor", "JetBrains", "nunit",
        };

        public static IReadOnlyList<Type> ScanModules(params Assembly[] assemblies)
        {
            var targets = assemblies is { Length: > 0 }
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies().Where(IsCandidateAssembly).ToArray();

            var results = new List<Type>();
            foreach (var assembly in targets)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }
                catch
                {
                    continue;
                }

                foreach (var type in types)
                {
                    if (type.IsAbstract || type.IsInterface || !typeof(IGameModule).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    if (type.GetCustomAttribute<GameModuleAttribute>() == null)
                    {
                        continue;
                    }

                    results.Add(type);
                }
            }

            return results;
        }

        /// <summary>排除系统与 Unity 内置程序集（可按项目需要追加过滤）。</summary>
        public static bool IsCandidateAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            foreach (var prefix in ExcludedAssemblyPrefixes)
            {
                if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
