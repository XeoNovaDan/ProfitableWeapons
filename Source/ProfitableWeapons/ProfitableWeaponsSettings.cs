using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;

namespace ProfitableWeapons
{
    public class ProfitableWeaponsSettings : ModSettings
    {

        public static bool flagInventoryWeapons = true;

        public static bool flagFromWellUsed = true;

        public static float lootedSellPriceMultFactor = 0.25f;

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
            options.CheckboxLabeled("SettingFlagInventoryWeapons".Translate(), ref flagInventoryWeapons, "SettingFlagInventoryWeaponsToolTip".Translate());
            options.Gap();
            options.CheckboxLabeled("SettingFlagWellUsedWeapons".Translate(), ref flagFromWellUsed, "SettingFlagWellUsedWeaponsToolTip".Translate());
            options.Gap();
            options.SliderLabeled("SettingLootedSellMultFactor".Translate(), ref lootedSellPriceMultFactor, lootedSellPriceMultFactor.ToStringPercent(), 0, 1, "SettingLootedSellMultFactorToolTip".Translate());
            options.Gap();
            Text.Font = GameFont.Medium;
            options.Label("MendAndRecycle");
            Text.Font = GameFont.Small;
            options.Gap(6);
            if (ModCompatibilityCheck.MendingIsActive)
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
            Scribe_Values.Look(ref lootedSellPriceMultFactor, "lootedSellPriceMultFactor", 0.25f);
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
