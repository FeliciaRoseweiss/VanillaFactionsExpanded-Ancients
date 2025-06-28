using RimWorld;
using Verse;

namespace VFEAncients
{
    public class DamageMultiplier : IExposable
    {
        public DamageDef damageDef;
        public float multiplier = 1f;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref damageDef, "damageDef");
            Scribe_Values.Look(ref multiplier, "multiplier", 1f);
        }
    }
}