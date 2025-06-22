using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace MIM40kFactions.Anomaly
{
    public class IncidentWorker_PitAttack : IncidentWorker
    {
        private const int MaxIterations = 100;

        private const float NecronPointsFactor = 0.6f;

        private static readonly IntRange NecronSpawnDelayTicks = new IntRange(180, 180);

        private static readonly IntRange PitBurrowEmergenceDelayRangeTicks = new IntRange(420, 420);

        private static readonly LargeBuildingSpawnParms BurrowSpawnParms = new LargeBuildingSpawnParms
        {
            maxDistanceToColonyBuilding = -1f,
            minDistToEdge = 10,
            attemptNotUnderBuildings = true,
            canSpawnOnImpassable = false,
            attemptSpawnLocationType = SpawnLocationType.Outdoors
        };

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!ModsConfig.AnomalyActive)
                return false;

            IncidentDefExtension modExtension = def.GetModExtension<IncidentDefExtension>();
            if (modExtension == null || modExtension.targetMod.NullOrEmpty())
            {
                Log.Warning("Mod Extension has configuration error.");
                return false;
            }

            if (!ModsConfig.IsActive(modExtension.targetMod))
                return false;

            _ = (Map)parms.target;
            return base.CanFireNowSub(parms);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!ModsConfig.AnomalyActive)
                return false;

            IncidentDefExtension modExtension = def.GetModExtension<IncidentDefExtension>();
            if (modExtension == null || !modExtension.targetMod.NullOrEmpty() || modExtension.targetGroupKindDef == null)
            {
                Log.Warning("Mod Extension has configuration error.");
                return false;
            }

            if (!ModsConfig.IsActive(modExtension.targetMod))
                return false;

            Map map = (Map)parms.target;
            float num = parms.points * 0.6f;
            List<Thing> list = new List<Thing>();
            int num2 = 0;
            while (num > 0f)
            {
                if (!LargeBuildingCellFinder.TryFindCell(out var cell, map, BurrowSpawnParms.ForThing(ThingDefOf.PitBurrow)))
                {
                    return false;
                }

                float num3 = Mathf.Min(num, 500f);
                Thing item = Utility_Necron.SpawnNecronsFromPitBurrowEmergence(cell, map, num3, PitBurrowEmergenceDelayRangeTicks, NecronSpawnDelayTicks, modExtension.targetGroupKindDef);
                list.Add(item);
                num -= num3;
                num2++;
                if (num2 > 100)
                {
                    break;
                }
            }

            SendStandardLetter(def.letterLabel, (list.Count > 1) ? def.letterTextPlural : def.letterText, def.letterDef, parms, list);
            return true;
        }
    }
}
