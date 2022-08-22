//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Modules
{
    public class AttachedModules
    {
        private static List<Type> _allModules;
        private static List<Module> _activeModules;
        private static List<Type> _allTypes;
        private static bool _isLoaded;

        private static readonly object SyncCore = new object();

        public static void LoadModules()
        {
            _allModules = new List<Type>();
            _activeModules = new List<Module>();

            var moduleType = typeof(IModuleBase);

            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(item => item.FullName.Contains("AdvantShop.Module") && !item.FullName.Contains(".CRUSHED")))
                {
                    try
                    {
                        var types = assembly.GetTypes().Where(t => t.GetInterface(moduleType.Name) != null && t.IsClass).ToList();
                        if (types.Count > 0)
                            _allModules.AddRange(types);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn("exception at loading module " + assembly.FullName, ex);
                        var crashedFile = assembly.CodeBase.Replace("file:///", "").Replace(".DLL", ".dll");
                        FileHelpers.DeleteFile(crashedFile + ".CRUSHED");
                        FileHelpers.RenameFile(crashedFile, crashedFile + ".CRUSHED");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("exception at global modules loading", ex);
                _isLoaded = true;
            }

            _activeModules = ModulesRepository.GetModulesFromDb().Where(m => m.IsInstall && m.Active).ToList();
            _isLoaded = true;
        }

        /// <summary>
        /// Get active modules
        /// </summary>
        /// <typeparam name="T">IModule interface</typeparam>
        /// <returns></returns>
        public static List<Type> GetModules<T>()
        {
            return GetModules<T>(false);
        }

        public static List<Type> GetModules<T>(bool ignoreActive)
        {
            if (!_isLoaded || _activeModules == null || _allModules == null)
                LoadModules();

            if (_activeModules == null || _allModules == null)
            {
                return null;
            }

            var type = typeof(T);

            var modules =
                _allModules.Where(
                    item => item != null &&
                        type.IsAssignableFrom(item) &&
                        (ignoreActive || _activeModules.Any(
                            m => String.Equals(item.Name, m.StringId, StringComparison.OrdinalIgnoreCase))))
                    .ToList();

            return modules;
        }

        /// <summary>
        /// Get module by id
        /// </summary>
        /// <param name="stringId">module id</param>
        /// <param name="active">if true return active module</param>
        /// <returns></returns>
        public static Type GetModuleById(string stringId, bool active = false)
        {
            if (!_isLoaded || _activeModules == null || _allModules == null)
                LoadModules();

            if (_activeModules == null || _allModules == null)
            {
                return null;
            }

            var module =
            _allModules.FirstOrDefault(
                item => item != null && String.Equals(item.Name, stringId, StringComparison.OrdinalIgnoreCase));

            if (active)
                return module != null &&
                       _activeModules.Any(m => m != null && String.Equals(module.Name, m.StringId, StringComparison.OrdinalIgnoreCase))
                    ? module
                    : null;

            return module;
        }

        public static List<Type> GetCore<T>()
        {
            lock (SyncCore)
            {
                var type = typeof(T);
                _allTypes = _allTypes ?? new List<Type>();
                var res = _allTypes.Where(x => x != null && x.IsClass && type.IsAssignableFrom(x)).ToList();
                if (res.Count > 0)
                    return res;

                var items = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x != null && x.FullName.StartsWith("AdvantShop") && !x.FullName.Contains("AdvantShop.Module"))
                    .SelectMany(a => a.GetTypes())
                    .Where(x => x != null && x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x))
                    .ToList();

                if (items.Count > 0)
                {
                    _allTypes.AddRange(items);
                    return items;
                }
                return new List<Type>();
            }
        }

        public static List<Type> GetModules()
        {
            if (!_isLoaded || _allModules == null)
                LoadModules();

            if (_allModules == null)
                _allModules = new List<Type>();
            
            return _allModules.Where(x => x != null).ToList();
        }

        public static List<Type> GetAllModules<T>()
        {
            var type = typeof(T);
            return GetModules().Where(x => x != null && type.IsAssignableFrom(x)).ToList();
        }
    }
}