using Verse;
using System.Collections.Generic;
using System.Xml;
using RimWorld;
using UnityEngine;
using System.Text;

namespace MIM40kFactions.Anomaly
{
    public class IncidentDefExtension : DefModExtension
    {
        public string targetMod = "";
        public PawnGroupKindDef targetGroupKindDef;
        public RaidStrategyDef selectedRaidStrategyDef;
    }
}
