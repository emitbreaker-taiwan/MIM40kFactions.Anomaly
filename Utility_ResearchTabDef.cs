using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace MIM40kFactions.Anomaly
{
    public class Utility_ResearchTabDef
    {
        public static ResearchTabDef Named(string defName)
        {
            return DefDatabase<ResearchTabDef>.GetNamed(defName);
        }
    }
}
