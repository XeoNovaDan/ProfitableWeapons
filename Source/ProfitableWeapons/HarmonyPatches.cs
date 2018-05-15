using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ProfitableWeapons
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance h = HarmonyInstance.Create("xeonvoadan.rimworld.profitableweapons.main");

            h.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "TryDropEquipment"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CheckScavengedWeapon)), null, null);

            h.Patch(AccessTools.Method(typeof(Pawn_InventoryTracker), "DropAllNearPawn"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CheckScavengedWeaponDrop)), null, null);
        }

        public static void CheckScavengedWeapon(Pawn_EquipmentTracker __instance, ref ThingWithComps eq)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            eq.TryGetComp<CompScavengedWeapon>()?.CheckScavengedWeapon(pawn);
        }

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

    }
}
