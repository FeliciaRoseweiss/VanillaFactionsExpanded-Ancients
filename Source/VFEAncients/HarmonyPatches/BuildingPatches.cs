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
        try
        {
            // Безопасное патчинг с проверкой существования методов
            var addHumanlikeOrdersMethod = AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders");
            if (addHumanlikeOrdersMethod != null)
            {
                harm.Patch(addHumanlikeOrdersMethod,
                    postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(AddCarryJobs)));
                Log.Message("VFEA: BuildingPatches - AddHumanlikeOrders patch applied");
            }
            else
            {
                Log.Warning("VFEA: BuildingPatches - AddHumanlikeOrders method not found");
            }

            var deteriorationMethod = AccessTools.Method(typeof(SteadyEnvironmentEffects), nameof(SteadyEnvironmentEffects.FinalDeteriorationRate),
                new[] { typeof(Thing), typeof(bool), typeof(bool), typeof(TerrainDef), typeof(List<string>) });
            if (deteriorationMethod != null)
            {
                harm.Patch(deteriorationMethod,
                    postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(AddDeterioration)));
                Log.Message("VFEA: BuildingPatches - FinalDeteriorationRate patch applied");
            }
            else
            {
                Log.Warning("VFEA: BuildingPatches - FinalDeteriorationRate method not found");
            }

            var hackingMethod = AccessTools.Method(typeof(JobDriver_Hack), "MakeNewToils");
            if (hackingMethod != null)
            {
                harm.Patch(hackingMethod, 
                    postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(FixHacking)));
                Log.Message("VFEA: BuildingPatches - MakeNewToils patch applied");
            }
            else
            {
                Log.Warning("VFEA: BuildingPatches - JobDriver_Hack.MakeNewToils method not found");
            }

            var powerNetTickMethod = AccessTools.Method(typeof(PowerNet), nameof(PowerNet.PowerNetTick));
            if (powerNetTickMethod != null)
            {
                harm.Patch(powerNetTickMethod,
                    transpiler: new HarmonyMethod(typeof(BuildingPatches), nameof(PowerNetOnSolarFlareTranspiler)),
                    postfix: new HarmonyMethod(typeof(BuildingPatches), nameof(PowerNetOnSolarFlarePostfix)));
                Log.Message("VFEA: BuildingPatches - PowerNetTick patch applied");
            }
            else
            {
                Log.Warning("VFEA: BuildingPatches - PowerNet.PowerNetTick method not found");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"VFEA: BuildingPatches - Error during patching: {ex.Message}");
        }
    }

    public static IEnumerable<CodeInstruction> PowerNetOnSolarFlareTranspiler(IEnumerable<CodeInstruction> codeInstructions)
    {
        var get_PowerOn = AccessTools.Method(typeof(CompPowerTrader), "get_PowerOn");
        var powerCompsField = AccessTools.Field(typeof(PowerNet), "powerComps");
        var found = false;
        var codes = codeInstructions.ToList();
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
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(CompSolarPowerUp), nameof(CompSolarPowerUp.PowerUpActive), new[] { typeof(CompPower) }));
                yield return new CodeInstruction(OpCodes.Brtrue_S, code.operand);
            }
        }
    }

    public static void PowerNetOnSolarFlarePostfix(PowerNet __instance)
    {
        __instance.PowerNetTickSolarFlare();
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
        if (t.TryGetComp<CompNeedsContainment>(out var comp) && comp.ShouldDeteriorate)
        {
            __result += StatDefOf.DeteriorationRate.Worker.GetBaseValueFor(StatRequest.For(t)) * 0.5f;
            reasons?.Add("VFEAncients.DeterioratingUncontained".Translate());
        }
    }

    public static void AddCarryJobs(List<FloatMenuOption> opts, Vector3 clickPos, Pawn pawn)
    {
        foreach (var localTargetInfo4 in GenUI.TargetsAt(clickPos, TargetingParameters.ForPawns(), true))
        {
            var target = localTargetInfo4.Pawn;
            if ((target.IsColonist && target.Downed) || target.IsPrisonerOfColony) CompGeneTailoringPod.AddCarryToPodJobs(opts, pawn, target);
        }

        foreach (var info in GenUI.TargetsAt(clickPos, TargetingParameters.ForPawns()))
        {
            var target = info.Pawn;
            if (target.Downed) CompBioBattery.AddCarryToBatteryJobs(opts, pawn, target);
        }
    }
}