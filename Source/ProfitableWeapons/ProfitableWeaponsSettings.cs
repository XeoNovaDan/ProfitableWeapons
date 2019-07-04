using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;
using SettingsHelper;

namespace ProfitableWeapons
{
    public class ProfitableWeaponsSettings : ModSettings
    {

        public static bool flagInventoryWeapons = true;

        public static bool flagFromWellUsed = true;

        public static float nonLootedSellPriceMult = 1;

        public static float lootedSellPriceMult = 0.2f;

        public static bool mendingRemoveLootedFlag = true;

        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard options = new Listing_Standard();
            Color defaultColor = GUI.color;
            options.Begin(wrect);

            GUI.color = defaultColor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            options.Gap();

            // Flag inventory weapons
            options.CheckboxLabeled("SettingFlagInventoryWeapons".Translate(), ref flagInventoryWeapons, "SettingFlagInventoryWeaponsToolTip".Translate());
            options.Gap();

            // Flag well-used weapons
            options.CheckboxLabeled("SettingFlagWellUsedWeapons".Translate(), ref flagFromWellUsed, "SettingFlagWellUsedWeaponsToolTip".Translate());
            options.Gap();

            // Non-looted sell price multiplier
            options.AddLabeledSlider("SettingNonLootedSellMult".Translate(), ref nonLootedSellPriceMult, 0, 1, rightAlignedLabel: nonLootedSellPriceMult.ToStringPercent(), roundTo: 0.01f);
            Text.Font = GameFont.Tiny;
            options.Label("SettingNonLootedSellMultNote".Translate());
            Text.Font = GameFont.Small;
            options.Gap();

            // Looted sell price multiplier
            options.AddLabeledSlider("SettingLootedSellMult".Translate(), ref lootedSellPriceMult, 0, 1, rightAlignedLabel: lootedSellPriceMult.ToStringPercent(), roundTo: 0.01f);
            Text.Font = GameFont.Tiny;
            options.Label("SettingLootedSellMultNote".Translate((nonLootedSellPriceMult * lootedSellPriceMult).ToStringPercent()));
            Text.Font = GameFont.Small;
            options.Gap();

            // MendAndRecycle integration
            Text.Font = GameFont.Medium;
            options.Label("MendAndRecycle");
            Text.Font = GameFont.Small;
            options.Gap(6);
            if (ModCompatibilityCheck.Mending)
            {
                options.CheckboxLabeled("MendingRemoveLootedFlag".Translate(), ref mendingRemoveLootedFlag, "MendingRemoveLootedFlagToolTip".Translate());
            }
            else
            {
                GUI.color = Color.grey;
                options.Label("MendingIsNotActive".Translate());
                GUI.color = defaultColor;
            }

            options.End();

            Mod.GetSettings<ProfitableWeaponsSettings>().Write();

        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref flagInventoryWeapons, "flagInventoryWeapons", true);
            Scribe_Values.Look(ref nonLootedSellPriceMult, "nonLootedSellPriceMult", 1);
            Scribe_Values.Look(ref lootedSellPriceMult, "lootedSellPriceMult", 0.2f);
            Scribe_Values.Look(ref flagFromWellUsed, "flagFromWellUsed", true);
            Scribe_Values.Look(ref mendingRemoveLootedFlag, "mendingRemoveLootedFlag", true);
        }

    }

    public class ProfitableWeapons : Mod
    {
        public ProfitableWeaponsSettings settings;

        public ProfitableWeapons(ModContentPack content) : base(content)
        {
            GetSettings<ProfitableWeaponsSettings>();
        }

        public override string SettingsCategory() => "ProfitableWeaponsSettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<ProfitableWeaponsSettings>().DoWindowContents(inRect);
        }
    }
}
