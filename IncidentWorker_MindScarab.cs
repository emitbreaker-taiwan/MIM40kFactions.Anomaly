using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace MIM40kFactions.Anomaly
{
    public class IncidentWorker_MindScarab : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!ModsConfig.IsActive("emitbreaker.MIM.WH40k.NC.Core"))
                return false;

            Map map = (Map)parms.target;
            IntVec3 result = parms.spawnCenter;
            if (!result.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Hostile))
            {
                return false;
            }

            Rot4 rot = Rot4.FromAngleFlat((map.Center - result).AngleFlat);
            GenSpawn.Spawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named("EMNC_Entities_NecronCanoptekScarabSwarm"), Faction.OfEntities, PawnGenerationContext.NonPlayer, map.Tile)), result, map, rot);
            return true;
        }
    }
}
