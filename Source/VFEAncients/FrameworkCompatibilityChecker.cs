using System;
using System.Reflection;
using Verse;
using HarmonyLib;

namespace VFEAncients
{
    /// <summary>
    /// Диагностический класс для проверки совместимости с VanillaExpandedFramework
    /// </summary>
    public static class FrameworkCompatibilityChecker
    {
        private static bool? _isVEFLoaded = null;
        private static Assembly _vefAssembly = null;

        public static bool IsVEFLoaded
        {
            get
            {
                if (_isVEFLoaded == null)
                {
                    CheckVEFCompatibility();
                }
                return _isVEFLoaded.Value;
            }
        }

        public static Assembly VEFAssembly => _vefAssembly;

        private static void CheckVEFCompatibility()
        {
            try
            {
                // Попытка найти сборку VanillaExpandedFramework
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var name = assembly.GetName().Name;
                    if (name.Contains("VanillaExpandedFramework") || 
                        name.Contains("VEF") || 
                        name.Contains("VanillaFactionsExpanded.Core"))
                    {
                        _vefAssembly = assembly;
                        _isVEFLoaded = true;
                        Log.Message($"VFEA: Found VEF assembly: {name} v{assembly.GetName().Version}");
                        break;
                    }
                }

                if (_vefAssembly == null)
                {
                    _isVEFLoaded = false;
                    Log.Warning("VFEA: VanillaExpandedFramework not found. Some features may be disabled.");
                }
                else
                {
                    CheckVEFClasses();
                }
            }
            catch (Exception ex)
            {
                _isVEFLoaded = false;
                Log.Error($"VFEA: Error checking VEF compatibility: {ex.Message}");
            }
        }

        private static void CheckVEFClasses()
        {
            var requiredClasses = new[]
            {
                "VEF.ExpandableProjectileDef",
                "VanillaExpandedFramework.ExpandableProjectileDef",
                "VEF.HediffComp_Phasing",
                "VanillaExpandedFramework.HediffComp_Phasing",
                "VEF.Abilities.AbilityDef",
                "VanillaExpandedFramework.Abilities.AbilityDef"
            };

            var foundClasses = 0;
            foreach (var className in requiredClasses)
            {
                try
                {
                    var type = _vefAssembly.GetType(className) ?? 
                              AccessTools.TypeByName(className);
                    
                    if (type != null)
                    {
                        foundClasses++;
                        Log.Message($"VFEA: Found required class: {className}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"VFEA: Could not find class {className}: {ex.Message}");
                }
            }

            Log.Message($"VFEA: Found {foundClasses}/{requiredClasses.Length} required VEF classes");
        }

        /// <summary>
        /// Безопасное получение типа из VEF
        /// </summary>
        public static Type GetVEFType(string typeName)
        {
            if (!IsVEFLoaded || _vefAssembly == null)
                return null;

            try
            {
                return _vefAssembly.GetType(typeName) ?? AccessTools.TypeByName(typeName);
            }
            catch (Exception ex)
            {
                Log.Warning($"VFEA: Could not get VEF type {typeName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Безопасное получение метода из VEF
        /// </summary>
        public static MethodInfo GetVEFMethod(Type type, string methodName, Type[] parameters = null)
        {
            if (type == null)
                return null;

            try
            {
                return parameters == null ? 
                    AccessTools.Method(type, methodName) : 
                    AccessTools.Method(type, methodName, parameters);
            }
            catch (Exception ex)
            {
                Log.Warning($"VFEA: Could not get VEF method {type.Name}.{methodName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Проверка совместимости API для конкретных классов
        /// </summary>
        public static bool CheckAPICompatibility()
        {
            if (!IsVEFLoaded)
                return false;

            var compatibility = true;

            // Проверка основных классов снарядов
            var projectileType = GetVEFType("VEF.ExpandableProjectileDef") ?? 
                                GetVEFType("VanillaExpandedFramework.ExpandableProjectileDef");
            
            if (projectileType == null)
            {
                Log.Warning("VFEA: ExpandableProjectileDef not found in VEF");
                compatibility = false;
            }

            // Проверка классов способностей
            var abilityType = GetVEFType("VEF.Abilities.AbilityDef") ?? 
                             GetVEFType("VanillaExpandedFramework.Abilities.AbilityDef");
            
            if (abilityType == null)
            {
                Log.Warning("VFEA: Ability system not found in VEF");
                compatibility = false;
            }

            // Проверка HediffComp классов
            var hediffCompType = GetVEFType("VEF.HediffComp_Phasing") ?? 
                                GetVEFType("VanillaExpandedFramework.HediffComp_Phasing");
            
            if (hediffCompType == null)
            {
                Log.Warning("VFEA: HediffComp_Phasing not found in VEF");
                compatibility = false;
            }

            return compatibility;
        }
    }
}
