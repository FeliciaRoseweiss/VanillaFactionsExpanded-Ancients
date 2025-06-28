using HarmonyLib;
using UnityEngine;
using Verse;
using VFEAncients.HarmonyPatches;

namespace VFEAncients
{
    public class VFEAncientsMod : Mod
    {
        public static Harmony Harm;

        public static bool YayosCombat;

        public static VFEAncientsSettings settings;

        public VFEAncientsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VFEAncientsSettings>();
            
            // Проверка совместимости с VanillaExpandedFramework
            Log.Message("VFEA: Initializing Vanilla Factions Expanded - Ancients");
            
            if (ModLister.HasActiveModWithName("Yayo's Combat 3 [Adopted]")) 
            {
                YayosCombat = true;
                Log.Message("VFEA: Yayo's Combat 3 detected, enabling compatibility patches.");
            }
            
            Harm = new Harmony("OskarPotocki.VFE.Ancients");
            
            try
            {
                // Применяем патчи в безопасном режиме
                PowerPatches.Do(Harm);
                AbilityPatches.Do(Harm);
                BuildingPatches.Do(Harm);
                PreceptPatches.Do(Harm);
                PointDefensePatches.Do(Harm);
                MetaMorphPatches.Do(Harm);
                StorytellerPatches.Do(Harm);
                MendingPatches.Do(Harm);
                QuestPatches.Do(Harm);
                
                Log.Message("VFEA: All Harmony patches applied successfully.");
            }
            catch (System.Exception ex)
            {
                Log.Error($"VFEA: Error applying Harmony patches: {ex.Message}");
            }
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => Content.Name;
    }
}