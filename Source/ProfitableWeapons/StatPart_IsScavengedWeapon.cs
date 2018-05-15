using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProfitableWeapons
{
    public class StatPart_IsScavengedWeapon : StatPart
    {

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing)
            {
                ThingWithComps weapon = req.Thing as ThingWithComps;
                CompScavengedWeapon comp = weapon.TryGetComp<CompScavengedWeapon>();
                if (comp != null) { 
                    bool IsScavengedWeapon = comp.IsScavengedWeapon;
                    if (weapon != null && IsScavengedWeapon)
                    {
                        val *= scavengedSellPriceFactorMult;
                    }
                }
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing)
            {
                ThingWithComps weapon = req.Thing as ThingWithComps;
                CompScavengedWeapon comp = weapon.TryGetComp<CompScavengedWeapon>();
                if (comp != null)
                {
                    bool IsScavengedWeapon = comp.IsScavengedWeapon;
                    if (weapon != null && IsScavengedWeapon)
                    {
                        return "StatsReport_IsScavengedWeapon".Translate() + ": x" + scavengedSellPriceFactorMult.ToStringPercent();
                    }
                }
            }
            return null;
        }

        private float scavengedSellPriceFactorMult = ProfitableWeaponsSettings.scavengeSellMultFactor;

    }
}
