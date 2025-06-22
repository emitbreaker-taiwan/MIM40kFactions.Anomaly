using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIM40kFactions;
using Verse;
using RimWorld;

namespace MIM40kFactions.Anomaly
{
    public class CompProducesNecrodermis : ThingComp
    {
        public CompProperties_ProducesNecrodermis Props => (CompProperties_ProducesNecrodermis)props;

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            StatDrawEntry statDrawEntry = NecrodermisStatDrawEntry(parent as Pawn);
            statDrawEntry.overridesHideStats = true;
            yield return statDrawEntry;
        }
        public static float NecrodermisPerDay(Pawn pawn)
        {
            CompProducesNecrodermis compProducesNecrodermis = pawn.TryGetComp<CompProducesNecrodermis>();
            if (compProducesNecrodermis == null)
            {
                return 0f;
            }

            float num = compProducesNecrodermis?.Props.necrodermisDensity ?? 1f;
            return pawn.BodySize * num;
        }
        public static StatDrawEntry NecrodermisStatDrawEntry(Pawn pawn)
        {
            CompProducesNecrodermis compProducesNecrodermis = pawn.TryGetComp<CompProducesNecrodermis>();
            StringBuilder stringBuilder = new StringBuilder("StatsReport_NecrodermisGeneration_Desc".Translate());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("StatsReport_BaseValue".Translate() + ": 1");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("StatsReport_BodySize".Translate(pawn.BodySize.ToString("F2")) + ": x" + pawn.BodySize.ToStringPercent());
            if (compProducesNecrodermis != null)
            {
                stringBuilder.AppendLine("StatsReport_NecrodermisDensityMultiplier".Translate() + ": x" + compProducesNecrodermis.Props.necrodermisDensity.ToStringPercent());
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("StatsReport_FinalValue".Translate() + ": " + NecrodermisPerDay(pawn).ToString("F1"));
            return new StatDrawEntry(StatCategoryDefOf.Containment, "StatsReport_NecrodermisGeneration".Translate(), NecrodermisPerDay(pawn).ToString(), stringBuilder.ToString(), 100);
        }
    }
}
