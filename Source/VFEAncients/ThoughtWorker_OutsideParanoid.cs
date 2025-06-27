using RimWorld;
using Verse;

namespace VFEAncients
{
    public class ThoughtWorker_OutsideParanoid : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            return p.Spawned && !p.Position.Roofed(p.Map) ? ThoughtState.ActiveDefault : false;
        }
    }
}
