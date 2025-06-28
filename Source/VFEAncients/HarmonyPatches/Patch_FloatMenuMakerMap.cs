using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEAncients.HarmonyPatches;

public static class Patch_FloatMenuMakerMap
{
    public static class AddHumanlikeOrders_Fix
    {
        public static bool CanEquip(Pawn pawn, Thing equipment)
        {
            // Default implementation - can be overridden by patches
            return pawn.CanReach(equipment, PathEndMode.OnCell, Danger.Deadly) && 
                   equipment.def.IsWeapon || equipment.def.IsApparel;
        }
    }
}