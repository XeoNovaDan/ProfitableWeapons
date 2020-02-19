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
using HarmonyLib;

namespace ProfitableWeapons
{

    [StaticConstructorOnStartup]
    public static class Patch_Pawn_EquipmentTracker
    {

        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.TryDropEquipment))]
        public static class TryDropEquipment
        {

            public static void Postfix(Pawn ___pawn, ref ThingWithComps eq)
            {
                // Try to flag equipped weapon as looted
                if (eq.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                    lootedComp.CheckLootedWeapon(___pawn);
            }

        }

    }
}
