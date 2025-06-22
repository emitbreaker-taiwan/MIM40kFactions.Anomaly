using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MIM40kFactions.Anomaly
{
    public class IncidentWorker_Assault : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return false;
            }

            IncidentDefExtension modExtension = def.GetModExtension<IncidentDefExtension>();
            if (modExtension == null || modExtension.targetMod.NullOrEmpty() || modExtension.targetGroupKindDef == null)
            {
                Log.Warning("Mod Extension has configuration error.");
                return false;
            }

            if (!ModsConfig.IsActive(modExtension.targetMod))
                return false;

            parms.pawnGroupKind = modExtension.targetGroupKindDef;
            return base.TryExecuteWorker(parms);
        }

        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            parms.faction = Faction.OfEntities;
            return true;
        }

        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            IncidentDefExtension modExtension = def.GetModExtension<IncidentDefExtension>();
            RaidStrategyDef raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            if (modExtension.selectedRaidStrategyDef != null)
                raidStrategy = modExtension.selectedRaidStrategyDef;
            parms.raidStrategy = raidStrategy;
        }

        protected override string GetLetterLabel(IncidentParms parms)
        {
            return parms.raidStrategy.letterLabelEnemy;
        }

        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            string text = string.Format(parms.raidArrivalMode.textEnemy, parms.pawnCount).CapitalizeFirst();
            text += "\n\n";
            text += parms.raidStrategy.arrivalTextEnemy;
            if (parms.pawnGroups != null && parms.PawnGroupCount > 1)
            {
                text += "\n\n" + "AttackingFromMultipleDirections".Translate();
            }

            int num = pawns.Count((Pawn e) => e.kindDef == PawnKindDefOf.ShamblerGorehulk);
            if (num == 1)
            {
                text += "\n\n" + "LetterText_ShamblerGorehulk".Translate();
            }
            else if (num > 1)
            {
                text += "\n\n" + "LetterText_ShamblerGorehulkPlural".Translate();
            }

            return text;
        }
    }
}
