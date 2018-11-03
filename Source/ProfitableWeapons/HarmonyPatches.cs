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

        static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            HarmonyInstance h = HarmonyInstance.Create("XeoNovaDan.ProfitableWeapons");

            // HarmonyInstance.DEBUG = true;

            h.Patch(AccessTools.Method(typeof(Pawn_InventoryTracker), nameof(Pawn_InventoryTracker.DropAllNearPawn)),
                new HarmonyMethod(patchType, nameof(PrefixDropAllNearPawn)), null);

            h.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.TryDropEquipment)), null,
                new HarmonyMethod(patchType, nameof(PostfixTryDropEquipment)));

            h.Patch(AccessTools.Method(typeof(Verb_LaunchProjectile), "TryCastShot"), null,
                new HarmonyMethod(patchType, nameof(PostfixTryCastShot)));

            h.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "TryCastShot"), null,
                new HarmonyMethod(patchType, nameof(PostfixTryCastShot)));

            //// Try and patch Mending
            try
            {
                ((Action)(() =>
                {
                    if (ModCompatibilityCheck.MendingIsActive)
                    {
                        Log.Message("Profitable Weapons :: MendAndRecycle detected as active in load order. Patching...");

                        h.Patch(AccessTools.Method(typeof(MendAndRecycle.JobDriver_Mend), "DoBill"), null,
                            new HarmonyMethod(typeof(HarmonyPatches), nameof(RemoveScavengedWeaponFlag)));

                    }
                }))();
            }
            catch (TypeLoadException)
            {
                Log.Message("Profitable Weapons :: MendAndRecycle not detected as active in load order.");
            }

            // Dynamically patch all ThingDefs that are weapons
            foreach (ThingDef weaponDef in DefDatabase<ThingDef>.AllDefs.Where(d => d.IsWeapon && !d.HasComp(typeof(CompLootedWeapon))))
            {
                // 70% sell price factor - nice 'n' easy
                weaponDef.SetStatBaseValue(StatDefOf.SellPriceFactor, 0.7f);

                // CompLootedWeapon
                if (weaponDef.comps == null)
                    weaponDef.comps = new List<CompProperties>();
                weaponDef.comps.Add(new CompProperties
                {
                    compClass = typeof(CompLootedWeapon)
                });
            }

        }

        public static void PrefixDropAllNearPawn(Pawn_InventoryTracker __instance, Pawn ___pawn, ref ThingOwner ___innerContainer)
        {
            if (ProfitableWeaponsSettings.flagInventoryWeapons)
                foreach (Thing thing in ___innerContainer)
                    if (thing.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                        lootedComp.CheckLootedWeapon(___pawn);
        }
        
        public static void PostfixTryDropEquipment(Pawn_EquipmentTracker __instance, Pawn ___pawn, ref ThingWithComps eq)
        {
            eq.TryGetComp<CompLootedWeapon>()?.CheckLootedWeapon(___pawn);
        }

        public static void PostfixTryCastShot(Verb __instance, bool __result)
        {
            if (__result && __instance.EquipmentSource?.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                lootedComp.ModifyAttackCounter(__instance);
        }

        // Thanks NIA!

        public static void RemoveScavengedWeaponFlag(MendAndRecycle.JobDriver_Mend __instance, Toil __result)
        {
            if (ProfitableWeaponsSettings.mendingRemoveLootedFlag)
            {
                var mendingDelegate = __result.tickAction;
                var weapon = __instance.job.GetTarget(MendAndRecycle.JobDriver_DoBill.objectTI).Thing;
                __result.tickAction = () =>
                {
                    mendingDelegate();
                    if (weapon != null && !weapon.Destroyed && weapon.HitPoints >= weapon.MaxHitPoints)
                        weapon.TryGetComp<CompLootedWeapon>()?.RemoveLootedWeaponFlag();
                };
            }
        }

    }
}
