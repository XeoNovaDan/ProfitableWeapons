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

        #region FlagInventoryWeapons
        public static bool flagInventoryWeapons = true;
        #endregion

        #region ScavengeSellMultFactor
        public static float scavengeSellMultFactor = 0.25f;
        #endregion

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
            options.SliderLabeled("SettingScavengeSellMultFactor".Translate(), ref scavengeSellMultFactor, scavengeSellMultFactor.ToStringPercent(), 0, 1, "SettingScavengeSellMultFactorToolTip".Translate());

            options.End();

            Mod.GetSettings<ProfitableWeaponsSettings>().Write();

        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref flagInventoryWeapons, "flagInventoryWeapons", true);
            Scribe_Values.Look(ref scavengeSellMultFactor, "scavengeSellMultFactor", 0.25f);
        }

    }

    public class ProfitableWeapons : Mod
    {
        public ProfitableWeaponsSettings settings;

        public ProfitableWeapons(ModContentPack content) : base(content)
        {
            GetSettings<ProfitableWeaponsSettings>();
        }

        public override string SettingsCategory() => "Viable Weapon Economy";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<ProfitableWeaponsSettings>().DoWindowContents(inRect);
        }
    }
}
