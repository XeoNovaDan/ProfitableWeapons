using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.AI;
using Harmony;

namespace ProfitableWeapons
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance h = HarmonyInstance.Create("XeoNovaDan.ProfitableWeapons");

            h.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "TryDropEquipment"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CheckScavengedWeapon)), null);

            h.Patch(AccessTools.Method(typeof(Pawn_InventoryTracker), "DropAllNearPawn"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CheckScavengedWeaponDrop)), null);

            // Try and patch Mending

            try
            {
                ((Action)(() =>
                {
                    if (ModCompatibilityCheck.MendingIsActive)
                    {
                        Log.Message("[Viable Weapon Trading]: Mending detected as active in load order. Patching...");

                        h.Patch(AccessTools.Method(typeof(Mending.JobDriver_Mend), "DoBill"), null,
                            new HarmonyMethod(typeof(HarmonyPatches), nameof(RemoveScavengedWeaponFlag)));

                    }
                }))();
            }
            catch (TypeLoadException) { }

        }

        // Prefix Pawn_EquipmentTracker TryDropEquipment

        public static void CheckScavengedWeapon(Pawn_EquipmentTracker __instance, ref ThingWithComps eq)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            eq.TryGetComp<CompScavengedWeapon>()?.CheckScavengedWeapon(pawn);
        }

        // Prefix Pawn_InventoryTracker DropAllNearPawn

        public static void CheckScavengedWeaponDrop(Pawn_InventoryTracker __instance)
        {
            if (ProfitableWeaponsSettings.flagInventoryWeapons)
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                ThingOwner<Thing> inner = Traverse.Create(__instance).Field("innerContainer").GetValue<ThingOwner<Thing>>();
                List<Thing> templist = Traverse.Create(__instance).Field("tmpThingList").GetValue<List<Thing>>();
                templist.Clear();
                templist.AddRange(inner);
                for (int i = 0; i < templist.Count; i++)
                {
                    if (templist[i] != null && templist[i] is ThingWithComps)
                    {
                        templist[i].TryGetComp<CompScavengedWeapon>()?.CheckScavengedWeapon(pawn);
                    }
                }
            }
        }

        // Postfix Mending.JobDriver_Mend DoBill - Thanks NIA!

        public static void RemoveScavengedWeaponFlag(Mending.JobDriver_Mend __instance, Toil __result)
        {
            if (ProfitableWeaponsSettings.mendingRemoveScavengedFlag)
            {
                var mendingDelegate = __result.tickAction;
                var weapon = __instance.job.GetTarget(Mending.JobDriver_DoBill.objectTI).Thing;
                __result.tickAction = () =>
                {
                    mendingDelegate();
                    if (weapon != null && !weapon.Destroyed && weapon.HitPoints == weapon.MaxHitPoints)
                    {
                        CompScavengedWeapon comp = weapon.TryGetComp<CompScavengedWeapon>();
                        if (comp != null)
                        {
                            comp.RemoveScavengedWeaponFlag();
                        }
                    }
                };
            }
        }

    }
}
