using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace ProfitableWeapons
{
    public class ProfitableWeapons : Mod
    {

        public static ProfitableWeaponsSettings settings;
        public static Harmony harmonyInstance;

        public ProfitableWeapons(ModContentPack content) : base(content)
        {
            #if DEBUG
                Log.Error("XeoNovaDan left debugging enabled in Profitable Weapons - please let him know!");
            #endif

            settings = GetSettings<ProfitableWeaponsSettings>();
            harmonyInstance = new Harmony("XeoNovaDan.ProfitableWeapons");
        }

        public override string SettingsCategory() => "ProfitableWeaponsSettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }

    }
}
