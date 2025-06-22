using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MIM40kFactions
{

    public class CompProperties_EquipRequirements : CompProperties
    {
        [NoTranslate]
        public List<string> apparelTags;

        public CompProperties_EquipRequirements() => this.compClass = typeof(CompEquipRequirements);
    }
}
