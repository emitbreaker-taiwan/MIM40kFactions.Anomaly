using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace MIM40kFactions.Anomaly
{
    public class CompProperties_ProducesNecrodermis : CompProperties
    {
        public float necrodermisDensity = 1f;

        public CompProperties_ProducesNecrodermis()
        {
            compClass = typeof(CompProducesNecrodermis);
        }
    }
}
