using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;

namespace MIM40kFactions.Anomaly
{
    public class Utility_Necron
    {
        private static FloatRange SwarmSizeVariance { get; } = new FloatRange(0.7f, 1.3f);
        public static List<Pawn> GenerateNecronAssault(IncidentParms parms, float points, PawnGroupKindDef GroupKindDef)
        {
            Map map = (Map)parms.target;
            Faction faction = new Faction();
            faction.def = FactionDef.Named("EMNC_Drazak");
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                groupKind = GroupKindDef,
                tile = map.Tile,
                faction = faction,
                points = points * SwarmSizeVariance.RandomInRange
            };
            pawnGroupMakerParms.points = Mathf.Max(pawnGroupMakerParms.points, Faction.OfEntities.def.MinPointsToGeneratePawnGroup(pawnGroupMakerParms.groupKind) * 1.05f);
            return GeneratePawns(pawnGroupMakerParms, Faction.OfEntities).ToList();
        }

        public static Thing SpawnNecronsFromPitBurrowEmergence(IntVec3 cell, Map map, float points, IntRange emergenceDelay, IntRange spawnDelay, PawnGroupKindDef groupKind, FactionDef factionDef = null, bool assaultColony = true)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return null;
            }

            if (!ModsConfig.IsActive("emitbreaker.MIM.WH40k.NC.Core"))
            {
                return null;
            }
            
            Faction targetFaction = new Faction();

            if (factionDef != null)
            {
                targetFaction.def = factionDef;
            }

            List<Pawn> necronsForPoints = GetNecronsForPoints(points, map, groupKind, factionDef != null ? targetFaction : null);
            BuildingGroundSpawner obj = (BuildingGroundSpawner)ThingMaker.MakeThing(ThingDefOf.PitBurrowSpawner);
            obj.emergeDelay = emergenceDelay;
            PitBurrow obj2 = (PitBurrow)obj.ThingToSpawn;
            obj2.emergingFleshbeasts = necronsForPoints;
            obj2.emergeDelay = spawnDelay.RandomInRange;
            obj2.assaultColony = assaultColony;
            GenSpawn.Spawn(obj, cell, map);
            return obj;
        }

        private static List<Pawn> GetNecronsForPoints(float points, Map map, PawnGroupKindDef groupKind, Faction targetFaction = null, bool allowDreadmeld = false)
        {
            if (!ModsConfig.IsActive("emitbreaker.MIM.WH40k.NC.Core"))
                return null;
            Faction faction = new Faction();
            faction.def = FactionDef.Named("EMNC_Drazak");
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms();
            pawnGroupMakerParms.groupKind = groupKind;
            pawnGroupMakerParms.tile = map.Tile;
            pawnGroupMakerParms.faction = faction;
            pawnGroupMakerParms.points = ((points > 0f) ? points : StorytellerUtility.DefaultThreatPointsNow(map));
            pawnGroupMakerParms.points = Mathf.Max(pawnGroupMakerParms.points, faction.def.MinPointsToGeneratePawnGroup(pawnGroupMakerParms.groupKind) * 1.05f);
            if (targetFaction == null)
            {
                targetFaction = Faction.OfEntities;
            }
            return GeneratePawns(pawnGroupMakerParms, targetFaction).ToList();
        }

        private static IEnumerable<Pawn> GeneratePawns(PawnGroupMakerParms parms, Faction faction, bool warnOnZeroResults = true)
        {
            if (parms.groupKind == null)
            {
                Log.Error("Tried to generate pawns with null pawn group kind def. parms=" + parms);
                yield break;
            }

            if (faction == null)
            {
                Log.Error("Tried to generate pawn kinds with null faction. parms=" + parms);
                yield break;
            }

            if (parms.faction.def.pawnGroupMakers.NullOrEmpty())
            {
                Log.Error(string.Concat("Faction ", parms.faction, " of def ", parms.faction.def, " has no PawnGroupMakers."));
                yield break;
            }

            if (!TryGetRandomPawnGroupMaker(parms, out var pawnGroupMaker))
            {
                Log.Error(string.Concat("Faction ", parms.faction, " of def ", parms.faction.def, " has no usable PawnGroupMakers for parms ", parms));
                yield break;
            }

            foreach (Pawn item in pawnGroupMaker.GeneratePawns(parms, warnOnZeroResults))
            {
                item.SetFaction(Faction.OfEntities);
                yield return item;
            }
        }
        private static bool TryGetRandomPawnGroupMaker(PawnGroupMakerParms parms, out PawnGroupMaker pawnGroupMaker)
        {
            if (parms.seed.HasValue)
            {
                Rand.PushState(parms.seed.Value);
            }

            bool result = parms.faction.def.pawnGroupMakers.Where((PawnGroupMaker gm) => gm.kindDef == parms.groupKind && gm.CanGenerateFrom(parms)).TryRandomElementByWeight((PawnGroupMaker gm) => gm.commonality, out pawnGroupMaker);
            if (parms.seed.HasValue)
            {
                Rand.PopState();
            }

            return result;
        }
    }
}
