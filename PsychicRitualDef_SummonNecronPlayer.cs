using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MIM40kFactions.Anomaly
{
    public class PsychicRitualDef_SummonNecronPlayer : PsychicRitualDef_InvocationCircle
    {
        public SimpleCurve necronCombatPointsFromQualityCurve;

        public PawnGroupKindDef pawnGroupKind;

        public FactionDef targetFactionDef;

        public bool assaultColony = false;

        public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            List<PsychicRitualToil> list = base.CreateToils(psychicRitual, parent);
            list.Add(new PsychicRitualToil_SummonNecronPlayer(InvokerRole));
            return list;
        }

    }
}
