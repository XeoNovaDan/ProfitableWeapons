using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProfitableWeapons
{
    public class StatPart_IsLootedWeapon : StatPart
    {

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing && req.Thing.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp && lootedComp.IsLootedWeapon)
                val *= LootedSellPriceFactorMult;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing && req.Thing.TryGetComp<CompLootedWeapon>() is CompLootedWeapon lootedComp && lootedComp.IsLootedWeapon)
                return "StatsReport_IsLootedWeapon".Translate() + ": x" + LootedSellPriceFactorMult.ToStringPercent();
            return null;
        }

        private float LootedSellPriceFactorMult => ProfitableWeaponsSettings.lootedSellPriceMultFactor;

    }
}
