using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace ProfitableWeapons
{
    public class CompScavengedWeapon : ThingComp
    {

        public bool IsScavengedWeapon
        {
            get
            {
                return isScavengedWeaponInt;
            }
        }

        public void CheckScavengedWeapon(Pawn pawn)
        {
            bool isPlayerOwned = pawn.Faction != null && pawn.Faction.IsPlayer;
            if (!isPlayerOwned)
            {
                if (pawn.guest != null)
                {
                    if (!pawn.guest.IsPrisoner)
                    {
                        isScavengedWeaponInt = true;
                    }
                }
                else
                {
                    isScavengedWeaponInt = true;
                }
            }
        }

        public void RemoveScavengedWeaponFlag()
        {
            isScavengedWeaponInt = false;
        }

        public override string TransformLabel(string label)
        {
            if (isScavengedWeaponInt)
            {
                label += " (" + "ScavengedWeaponChar".Translate() + ")";
            }
            return label;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<bool>(ref isScavengedWeaponInt, "scavenged", false, false);
        }

        private bool isScavengedWeaponInt;

    }
}
