using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VFEAncients.HarmonyPatches;

public static class BuildingPatches
{
    public static void Do(Harmony harm)
    {
        // Проверяем, существует ли метод AddHumanlikeOrders перед патчингом
        var addHumanlikeOrdersMethod = AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders");
        if (addHumanlikeOrdersMethod != null)
        {
            harm.Patch(addHumanlikeOrdersMethod,
                postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(AddCarryJobs)));
        }
        
        harm.Patch(AccessTools.Method(typeof(SteadyEnvironmentEffects), nameof(SteadyEnvironmentEffects.FinalDeteriorationRate),
                new[] { typeof(Thing), typeof(bool), typeof(bool), typeof(TerrainDef), typeof(List<string>) }),
            postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(AddDeterioration)));
            
        // Проверяем, существует ли метод MakeNewToils перед патчингом
        var makeNewToilsMethod = AccessTools.Method(typeof(JobDriver_Hack), "MakeNewToils");
        if (makeNewToilsMethod != null)
        {
            harm.Patch(makeNewToilsMethod, postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(FixHacking)));
        }
        
        harm.Patch(AccessTools.Method(typeof(PowerNet), nameof(PowerNet.PowerNetTick)),
            transpiler: new HarmonyMethod(typeof(BuildingPatches), nameof(PowerNetOnSolarFlareTranspiler)),
            postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(PowerNetOnSolarFlarePostfix)));
    }

    public static IEnumerable<CodeInstruction> PowerNetOnSolarFlareTranspiler(IEnumerable<CodeInstruction> codeInstructions)
    {
        var get_PowerOn = AccessTools.Method(typeof(CompPowerTrader), "get_PowerOn");
        var powerCompsField = AccessTools.Field(typeof(PowerNet), "powerComps");
        var found = false;
        var codes = codeInstructions.ToList();
        
        // Проверяем, существует ли тип CompSolarPowerUp перед его использованием
        var solarPowerUpType = AccessTools.TypeByName("VFEAncients.CompSolarPowerUp");
        if (solarPowerUpType == null)
        {
            // Если тип не существует, возвращаем оригинальные инструкции
            foreach (var code in codes)
                yield return code;
            yield break;
        }
        
        var powerUpActiveMethod = AccessTools.Method(solarPowerUpType, nameof(CompSolarPowerUp.PowerUpActive), new[] { typeof(CompPower) });
        
        for (var i = 0; i < codes.Count; i++)
        {
            var code = codes[i];
            yield return code;
            if (!found && i > 4 && code.opcode == OpCodes.Brfalse_S && codes[i - 1].Calls(get_PowerOn) && codes[i - 4].LoadsField(powerCompsField))
            {
                found = true;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, powerCompsField);
                yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<CompPowerTrader>), "get_Item"));
                yield return new CodeInstruction(OpCodes.Call, powerUpActiveMethod);
                yield return new CodeInstruction(OpCodes.Brtrue_S, code.operand);
            }
        }
    }

    public static void PowerNetOnSolarFlarePostfix(PowerNet __instance)
    {
        // Проверяем, существует ли метод PowerNetTickSolarFlare перед его использованием
        var powerNetTickSolarFlareMethod = AccessTools.Method(typeof(PowerNet), "PowerNetTickSolarFlare");
        if (powerNetTickSolarFlareMethod != null)
        {
            powerNetTickSolarFlareMethod.Invoke(__instance, null);
        }
    }

    public static IEnumerable<Toil> FixHacking(IEnumerable<Toil> toils, JobDriver_Hack __instance)
    {
        var idx = 0;
        foreach (var toil in toils)
        {
            if (!__instance.job.targetA.Thing.def.hasInteractionCell)
                switch (idx)
                {
                    case 0:
                        toil.initAction = delegate { toil.actor.pather.StartPath(toil.actor.jobs.curJob.GetTarget(TargetIndex.A), PathEndMode.Touch); };
                        break;
                    case 1:
                        toil.endConditions = new List<Func<JobCondition>>
                        {
                            () => toil.actor.CanReachImmediate(toil.actor.jobs.curJob.GetTarget(TargetIndex.A), PathEndMode.Touch)
                                ? JobCondition.Ongoing
                                : JobCondition.Incompletable,
                            () => toil.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing.TryGetComp<CompHackable>().IsHacked
                                ? JobCondition.Succeeded
                                : JobCondition.Ongoing
                        };
                        break;
                }

            idx++;
            yield return toil;
        }
    }

    public static void AddDeterioration(Thing t, List<string> reasons, ref float __result)
    {
        // Проверяем, существует ли тип CompNeedsContainment перед его использованием
        CompProperties needsContainmentType = new CompProperties(AccessTools.TypeByName("VFEAncients.CompNeedsContainment"));
        if (needsContainmentType != null)
        {
            var comp = t.TryGetComp(needsContainmentType);
            if (comp != null)
            {
                var shouldDeteriorateProperty = AccessTools.Property(needsContainmentType.compClass, "ShouldDeteriorate");
                if (shouldDeteriorateProperty != null)
                {
                    var shouldDeteriorate = (bool)shouldDeteriorateProperty.GetValue(comp);
                    if (shouldDeteriorate)
                    {
                        __result += StatDefOf.DeteriorationRate.Worker.GetBaseValueFor(StatRequest.For(t)) * 0.5f;
                        reasons?.Add("VFEAncients.DeterioratingUncontained".Translate());
                    }
                }
            }
        }
    }

    public static void AddCarryJobs(List<FloatMenuOption> opts, Vector3 clickPos, Pawn pawn)
    {
        foreach (var localTargetInfo4 in GenUI.TargetsAt(clickPos, TargetingParameters.ForPawns(), true))
        {
            var target = localTargetInfo4.Pawn;
            if ((target.IsColonist && target.Downed) || target.IsPrisonerOfColony) 
            {
                // Проверяем, существует ли тип CompGeneTailoringPod перед его использованием
                var geneTailoringPodType = AccessTools.TypeByName("CompGeneTailoringPod");
                if (geneTailoringPodType != null)
                {
                    var addCarryToPodJobsMethod = AccessTools.Method(geneTailoringPodType, "AddCarryToPodJobs");
                    if (addCarryToPodJobsMethod != null)
                    {
                        addCarryToPodJobsMethod.Invoke(null, new object[] { opts, pawn, target });
                    }
                }
            }
        }

        foreach (var info in GenUI.TargetsAt(clickPos, TargetingParameters.ForPawns()))
        {
            var target = info.Pawn;
            if (target.Downed) 
            {
                // Проверяем, существует ли тип CompBioBattery перед его использованием
                var bioBatteryType = AccessTools.TypeByName("VFEAncients.CompBioBattery");
                if (bioBatteryType != null)
                {
                    var addCarryToBatteryJobsMethod = AccessTools.Method(bioBatteryType, "AddCarryToBatteryJobs");
                    if (addCarryToBatteryJobsMethod != null)
                    {
                        addCarryToBatteryJobsMethod.Invoke(null, new object[] { opts, pawn, target });
                    }
                }
            }
        }
    }
}