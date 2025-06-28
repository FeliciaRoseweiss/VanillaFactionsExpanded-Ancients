using RimWorld;
using Verse;
using Verse.Sound;

namespace VFEAncients
{
    public class HediffCompProperties_SoundOnRemove : HediffCompProperties
    {
        public SoundDef sound;

        public HediffCompProperties_SoundOnRemove()
        {
            compClass = typeof(HediffComp_SoundOnRemove);
        }
    }

    public class HediffComp_SoundOnRemove : HediffComp
    {
        public HediffCompProperties_SoundOnRemove Props => (HediffCompProperties_SoundOnRemove)props;

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (Props.sound != null)
            {
                Props.sound.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
            }
        }
    }

    public class HediffComp_MetaMorph : HediffComp
    {
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
        }
    }
}
