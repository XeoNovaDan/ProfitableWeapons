using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ProfitableWeapons
{
    public class CompLootedWeapon : ThingComp
    {

        private bool isLootedWeaponInt = false;
        private int attackCounter = 0;

        private const int BaseAttacksUntilWellUsedThreshold = 20;

        private bool WellUsedWeapon => attackCounter >= BaseAttacksUntilWellUsedThreshold * ((parent.def.Verbs[0] is VerbProperties verb) ? verb.burstShotCount : 1)
            && ProfitableWeaponsSettings.flagFromWellUsed;

        public bool IsUsedWeapon => isLootedWeaponInt || WellUsedWeapon;

        public void ModifyAttackCounter()
        {
            if (ProfitableWeaponsSettings.flagFromWellUsed)
                attackCounter++;
        }

        public void CheckLootedWeapon(Pawn pawn)
        {
            if (pawn.Faction != null && !pawn.Faction.IsPlayer)
            {
                if (pawn.guest != null && !pawn.guest.IsPrisoner)
                    isLootedWeaponInt = true;
                else
                    isLootedWeaponInt = true;
            }
        }

        public void RemoveLootedWeaponFlag()
        {
            isLootedWeaponInt = false;
        }

        public override string TransformLabel(string label)
        {
            if (isLootedWeaponInt)
                label += " (" + "LootedWeaponChar".Translate() + ")";
            return label;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref isLootedWeaponInt, "looted", false);
            Scribe_Values.Look(ref attackCounter, "attacks", 0);
        }

    }
}
