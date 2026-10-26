using System;
using System.Collections.Generic;
using System.Linq;

namespace GameFrameY.Core.Module
{
    /// <summary>
    /// 依赖解析与排序：DFS 拓扑排序（依赖先装配），检测缺失依赖与循环依赖；
    /// 同层无依赖时按 Priority、名字稳定排序，保证装配顺序可复现。
    /// </summary>
    public static class ModuleDependencyResolver
    {
        public static IReadOnlyList<GameModuleDescriptor> Resolve(IReadOnlyList<Type> moduleTypes)
        {
            return Resolve(moduleTypes, Array.Empty<string>());
        }

        /// <summary>
        /// 解析指定程序集中的模块；<paramref name="satisfiedDependencies"/> 表示已经安装、可作为依赖上下文的模块名。
        /// 这些模块不会出现在本次结果中，但可以满足新程序集模块的依赖声明。
        /// </summary>
        public static IReadOnlyList<GameModuleDescriptor> Resolve(
            IReadOnlyList<Type> moduleTypes,
            IReadOnlyCollection<string> satisfiedDependencies)
        {
            var satisfied = satisfiedDependencies == null
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(satisfiedDependencies, StringComparer.OrdinalIgnoreCase);
            var descriptors = new List<GameModuleDescriptor>();
            var byName = new Dictionary<string, GameModuleDescriptor>(StringComparer.OrdinalIgnoreCase);
            foreach (var type in moduleTypes)
            {
                var attribute = type.GetCustomAttributeInternal();
                var descriptor = new GameModuleDescriptor(type, attribute);
                if (byName.ContainsKey(descriptor.Name))
                {
                    throw new GameModuleException($"模块名重复: {descriptor.Name} ({descriptor.ModuleType.FullName} 与 {byName[descriptor.Name].ModuleType.FullName})");
                }

                byName[descriptor.Name] = descriptor;
                descriptors.Add(descriptor);
            }

            var seeds = descriptors
                .OrderBy(d => d.Priority)
                .ThenBy(d => d.Name, StringComparer.Ordinal)
                .ToList();

            var result = new List<GameModuleDescriptor>(descriptors.Count);
            var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void Visit(GameModuleDescriptor descriptor, Stack<string> path)
            {
                if (visiting.Contains(descriptor.Name))
                {
                    throw new GameModuleException($"模块依赖成环: {string.Join(" -> ", path.Reverse())} -> {descriptor.Name}");
                }

                if (visited.Contains(descriptor.Name))
                {
                    return;
                }

                visiting.Add(descriptor.Name);
                path.Push(descriptor.Name);
                foreach (var dependencyName in descriptor.Dependencies)
                {
                    if (!byName.TryGetValue(dependencyName, out var dependency))
                    {
                        if (satisfied.Contains(dependencyName))
                        {
                            continue;
                        }

                        throw new GameModuleException($"模块 {descriptor.Name} 声明的依赖 [{dependencyName}] 未在扫描结果中找到，也不在已安装模块中");
                    }

                    Visit(dependency, path);
                }

                path.Pop();
                visiting.Remove(descriptor.Name);
                visited.Add(descriptor.Name);
                result.Add(descriptor);
            }

            var stack = new Stack<string>();
            foreach (var seed in seeds)
            {
                Visit(seed, stack);
            }

            for (var i = 0; i < result.Count; i++)
            {
                result[i].InstallOrder = i;
            }

            return result;
        }
    }

    internal static class ModuleAttributeExtensions
    {
        public static GameModuleAttribute GetCustomAttributeInternal(this Type type)
        {
            var attribute = (GameModuleAttribute)Attribute.GetCustomAttribute(type, typeof(GameModuleAttribute));
            if (attribute == null)
            {
                throw new GameModuleException($"模块 {type.FullName} 缺少 {nameof(GameModuleAttribute)}");
            }

            return attribute;
        }
    }
}
