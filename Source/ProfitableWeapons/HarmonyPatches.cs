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
            //HarmonyInstance.DEBUG = true;

            // Do automatic patches
            h.PatchAll();

            // Manual patches
            var tryCastShotPostfix = new HarmonyMethod(patchType, nameof(Postfix_TryCastShot));
            h.Patch(AccessTools.Method(typeof(Verb_LaunchProjectile), "TryCastShot"), postfix: tryCastShotPostfix);
            h.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "TryCastShot"), postfix: tryCastShotPostfix);

            // Try and patch Combat Extended
            if (ModCompatibilityCheck.CombatExtended)
            {
                // Melee verb
                var meleeVerbCE = GenTypes.GetTypeInAnyAssemblyNew("CombatExtended.Verb_MeleeAttackCE", null);
                if (meleeVerbCE != null)
                    h.Patch(AccessTools.Method(meleeVerbCE, "TryCastShot"), postfix: tryCastShotPostfix);
                else
                    Log.Error("Profitable Weapons - Couldn't find CombatExtended.Verb_MeleeAttackCE type to patch");

                // Ranged verb
                var launchProjectileVerbCE = GenTypes.GetTypeInAnyAssemblyNew("CombatExtended.Verb_LaunchProjectileCE", null);
                if (launchProjectileVerbCE != null)
                    h.Patch(AccessTools.Method(launchProjectileVerbCE, "TryCastShot"), postfix: tryCastShotPostfix);
                else
                    Log.Error("Profitable Weapons - Couldn't find CombatExtended.Verb_LaunchProjectileCE type to patch");
            }

            // Try and patch Mending
            if (ModCompatibilityCheck.Mending)
            {
                // Mending JobDriver
                var mendingJobDriver = GenTypes.GetTypeInAnyAssemblyNew("MendAndRecycle.JobDriver_Mend", null);
                if (mendingJobDriver != null)
                {
                    h.Patch(mendingJobDriver.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).First().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).MaxBy(mi => mi.GetMethodBody()?.GetILAsByteArray().Length ?? -1),
                        transpiler: new HarmonyMethod(patchType, nameof(Transpile_MendAndReycle_JobDriver_Mend_MendToil_TickAction)));
                }
                else
                    Log.Error("Profitable Weapons - Couldn't find MendAndRecycle.JobDriver_Mend type to patch");
            }

            // Try and patch Nano Repair Tech
            if (ModCompatibilityCheck.NanoRepairTech)
            {
                // Nano repairing
                var nanoRepair = GenTypes.GetTypeInAnyAssemblyNew("Ogre.NanoRepairTech.NanoRepair", null);
                if (nanoRepair != null)
                    h.Patch(AccessTools.Method(nanoRepair, "ProcessTick"), transpiler: new HarmonyMethod(patchType, nameof(Transpile_NanoRepairTech_NanoRepair_ProcessTick)));
                else
                    Log.Error("Profitable Weapons - Couldn't find Ogre.NanoRepairTech.NanoRepair type to patch");
            }

        }

        [HarmonyPatch(typeof(Pawn_InventoryTracker))]
        [HarmonyPatch(nameof(Pawn_InventoryTracker.DropAllNearPawn))]
        public static class Patch_Pawn_InventoryTracker_DropAllNearPawn
        {

            public static void Prefix(Pawn ___pawn, ref ThingOwner ___innerContainer)
            {
                // If set to flag inventory weapons as looted, go through each item that was in inventory and attempt to flag as looted
                if (ProfitableWeaponsSettings.flagInventoryWeapons)
                    foreach (Thing thing in ___innerContainer)
                        if (thing.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                            lootedComp.CheckLootedWeapon(___pawn);
            }

        }

        [HarmonyPatch(typeof(Pawn_EquipmentTracker))]
        [HarmonyPatch(nameof(Pawn_EquipmentTracker.TryDropEquipment))]
        public static class Patch_Pawn_EquipmentTracker_TryDropEquipment
        {

            public static void Postfix(Pawn ___pawn, ref ThingWithComps eq)
            {
                // Try to flag equipped weapon as looted
                if (eq.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                    lootedComp.CheckLootedWeapon(___pawn);
            }

        }

        public static void Postfix_TryCastShot(Verb __instance, bool __result)
        {
            if (__result && __instance.EquipmentSource?.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                lootedComp.ModifyAttackCounter(__instance);
        }

        #region Patches on other mods
        public static void RemoveUsedWeaponFlag(bool settingAllowed, Thing thing) // Helper method for transpiler
        {
            // If settings allow for Mending to remove looted weapon flags and the thing in question has CompLootedWeapon, remove the looted flag
            if (settingAllowed && thing.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp)
                lootedComp.RemoveLootedWeaponFlag();
        }

        // MendAndRecycle
        public static IEnumerable<CodeInstruction> Transpile_MendAndReycle_JobDriver_Mend_MendToil_TickAction(IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = instructions.ToList();

            var removeDeadmanSettingFieldInfo = AccessTools.Field(GenTypes.GetTypeInAnyAssemblyNew("MendAndRecycle.Settings", null), "removesDeadman");

            for (int i = 0; i < instructionList.Count; i++)
            {
                var instruction = instructionList[i];

                // If instruction checks for 'remove deadman' setting, add call to our helper method before it
                if (instruction.opcode == OpCodes.Ldsfld && instruction.operand == removeDeadmanSettingFieldInfo)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(ProfitableWeaponsSettings), "mendingRemoveLootedFlag")); // ProfitableWeaponsSettings.mendingRemoveLootedFlag
                    yield return new CodeInstruction(OpCodes.Ldloc_0); // thing
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(RemoveUsedWeaponFlag))); // RemoveUsedWeaponFlag(ProfitableWeaponsSettings.mendingRemoveLootedFlag, thing)
                }

                yield return instruction;
            }
        }

        public static void RemoveUsedWeaponFlagNano(bool settingAllowed, Thing thing)
        {
            if (thing.HitPoints >= thing.MaxHitPoints)
                RemoveUsedWeaponFlag(settingAllowed, thing);
        }

        // Nano Repair Tech
        public static IEnumerable<CodeInstruction> Transpile_NanoRepairTech_NanoRepair_ProcessTick(IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = instructions.ToList();
            bool done = false;

            var hitPointsGetter = AccessTools.Property(typeof(Thing), nameof(Thing.HitPoints)).GetGetMethod();

            for (int i = 0; i < instructionList.Count; i++)
            {
                var instruction = instructionList[i];

                // Look for the 'nop' instruction that immediately precedes 'bool flag11 = thing.HitPoints < thing.MaxHitPoints;'
                if (!done &&
                    instructionList[i + 1].opcode == OpCodes.Ldloc_S && ((LocalBuilder)instructionList[i + 1].operand).LocalIndex == 17 &&
                    instructionList[i + 2].opcode == OpCodes.Callvirt && instructionList[i + 2].operand == hitPointsGetter)
                {
                    yield return instruction; // nop
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(ProfitableWeaponsSettings), "nanoRepairRemoveLootedFlag")); // ProfitableWeaponsSettings.nanoRepairRemoveLootedFlag
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 17); // thing
                    instruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(RemoveUsedWeaponFlagNano))); // RemoveUsedWeaponFlagNano(ProfitableWeaponsSettings.nanoRepairRemoveLootedFlag, thing)
                    done = true;
                }

                yield return instruction;
            }
        }
        #endregion

    }
}
