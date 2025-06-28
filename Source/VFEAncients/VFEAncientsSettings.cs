using UnityEngine;
using Verse;

namespace VFEAncients
{
    public class VFEAncientsSettings : ModSettings
    {
        public bool enableGloryKillMusic = true;
        
        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableGloryKillMusic, "enableGloryKillMusic", true);
            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Enable Glory Kill Music", ref enableGloryKillMusic);
            listingStandard.End();
        }
    }
}