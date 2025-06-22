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
    public class IncidentWorker_FlayedOne : IncidentWorker_EntitySwarm
    {
        protected virtual IntRange SwarmLifespanTicksRange { get; } = new IntRange(150000, 210000);

        protected override PawnGroupKindDef GroupKindDef => def.GetModExtension<IncidentDefExtension>().targetGroupKindDef;

        protected override LordJob GenerateLordJob(IntVec3 entry, IntVec3 dest)
        {
            return new LordJob_ShamblerSwarm(entry, dest);
        }

        protected override List<Pawn> GenerateEntities(IncidentParms parms, float points)
        {
            IncidentDefExtension modExtension = def.GetModExtension<IncidentDefExtension>();
            if (modExtension == null || !modExtension.targetMod.NullOrEmpty() || modExtension.targetGroupKindDef == null)
            {
                Log.Warning("Mod Extension has configuration error.");
                return null;
            }

            if (!ModsConfig.IsActive(modExtension.targetMod))
                return null;

            List<Pawn> list = Utility_Necron.GenerateNecronAssault(parms, points, modExtension.targetGroupKindDef);
            SetupSwarmHediffs(list, SwarmLifespanTicksRange);
            return list;
        }

        protected void SetupSwarmHediffs(List<Pawn> shamblers, IntRange lifespanRange)
        {
            foreach (Pawn shambler in shamblers)
            {
                Hediff firstHediffOfDef = shambler.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Shambler);
                firstHediffOfDef.CurStage.becomeVisible = false;
                HediffComp_DisappearsAndKills hediffComp_DisappearsAndKills = firstHediffOfDef?.TryGetComp<HediffComp_DisappearsAndKills>();
                if (firstHediffOfDef == null || hediffComp_DisappearsAndKills == null)
                {
                    Log.ErrorOnce("ShamblerSwarm spawned pawn without Shambler hediff", 63426234);
                    continue;
                }

                hediffComp_DisappearsAndKills.disabled = false;
                hediffComp_DisappearsAndKills.ticksToDisappear = lifespanRange.RandomInRange;
            }
        }

        protected override void SendLetter(IncidentParms parms, List<Pawn> entities)
        {
            TaggedString baseLetterLabel = ((entities.Count > 1) ? "LetterLabelEMNC_FlayedOnesArrived".Translate() : "LetterLabelEMNC_FlayedOneArrived".Translate());
            TaggedString baseLetterText = ((entities.Count > 1) ? "LetterEMNC_FlayedOnesArrived".Translate(entities.Count) : "LetterShamblerArrived".Translate());
            int num = entities.Count((Pawn e) => e.kindDef == PawnKindDefOf.ShamblerGorehulk);

            SendStandardLetter(baseLetterLabel, baseLetterText, def.letterDef, parms, entities);
        }
    }
}
