using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Globalization;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
using Verse.AI;
using Verse.Grammar;
using Verse.Sound;
using HarmonyLib;

namespace MIM40kFactions.Anomaly
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("rimworld.emitbreaker.MIM.WH40k.Anomaly");
            //EMWH_ResearchTabPatch
            harmony.Patch(AccessTools.Method(typeof(MainTabWindow_Research), "DrawProjectInfo", new System.Type[1] { typeof(Rect) }), postfix: new HarmonyMethod(typeof(EMWH_ResearchTabPatch).GetMethod("DrawProjectInfoNecronPostfix")));
            //EMWH_MutantWeaponPatch
            harmony.Patch(AccessTools.Method(typeof(MutantUtility), "SetFreshPawnAsMutant", new System.Type[2] { typeof(Pawn), typeof(MutantDef) }), prefix: new HarmonyMethod(typeof(EMWH_MutantWeaponPatch).GetMethod("SetFreshPawnAsMutantPrefix")));
            harmony.Patch(AccessTools.Method(typeof(Pawn_MutantTracker), "HandleEquipment"), prefix: new HarmonyMethod(typeof(EMWH_MutantWeaponPatch).GetMethod("HandleEquipmentPrefix")));
            harmony.Patch(AccessTools.Method(typeof(Game), "FinalizeInit"), postfix: new HarmonyMethod(typeof(EMWH_UnlockForNecronsPatch).GetMethod("FinalizeInit_Postfix")));
        }
    }

    public static class EMWH_ResearchTabPatch
    {
        [HarmonyPostfix]
        public static void DrawProjectInfoNecronPostfix(Rect rect, ref ResearchTabDef ___curTabInt, ref ResearchProjectDef ___selectedProject)
        {
            if (!ModsConfig.IsActive("emitbreaker.MIM.WH40k.NC.Core"))
                return;

            if (___curTabInt != Utility_ResearchTabDef.Named("EMNC_Necron_Research"))
            {
                if (___curTabInt == null)
                {
                    Log.Message("Cannot get ResearchTabDef EMNC_Necron_Research");
                }
                return;
            }

            //var mainTabWindow_Research = new MainTabWindow_Research();
            //var drawProjectProgress = mainTabWindow_Research.GetType().GetMethod("DrawProjectProgress", BindingFlags.NonPublic | BindingFlags.Instance);
            //var drawStartButton = mainTabWindow_Research.GetType().GetMethod("DrawStartButton", BindingFlags.NonPublic | BindingFlags.Instance);
            //var drawProjectPrimaryInfo = mainTabWindow_Research.GetType().GetMethod("DrawProjectPrimaryInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            //var drawProjectScrollView = mainTabWindow_Research.GetType().GetMethod("DrawProjectScrollView", BindingFlags.NonPublic | BindingFlags.Instance);

            int num = 2;
            float num2 = (75f * (float)num);
            Rect rect2 = rect;
            rect2.yMin = rect.yMax - num2;
            rect2.yMax = rect.yMax;
            Rect rect3 = rect2;
            Rect rect4 = rect2;
            rect4.y = rect2.y - 30f;
            rect4.height = 28f;
            rect2 = rect2.ContractedBy(10f);
            rect2.y += 5f;
            Text.Font = GameFont.Medium;
            string key = "ActiveProjectPlural";
            Widgets.Label(rect4, key.Translate());
            Text.Font = GameFont.Small;
            Rect startButRect = new Rect
            {
                y = rect4.y - 55f - 10f,
                height = 55f,
                x = rect.center.x - rect.width / 4f,
                width = rect.width / 2f + 20f
            };
            Widgets.DrawMenuSection(rect3);

            if (ModsConfig.AnomalyActive && ___curTabInt == Utility_ResearchTabDef.Named("EMNC_Necron_Research"))
            {
                Rect rect5 = rect2;
                rect5.height = rect2.height / 2f;
                Rect rect6 = rect5;
                rect6.yMin = rect2.yMax - rect5.height;
                rect6.yMax = rect2.yMax;
                ResearchProjectDef project = Find.ResearchManager.GetProject(Utility_KnowledgeCategoryDef.Named("EMNC_Basic"));
                ResearchProjectDef project2 = Find.ResearchManager.GetProject(Utility_KnowledgeCategoryDef.Named("EMNC_Advanced"));
                if (project == null && project2 == null)
                {
                    using (new TextBlock((TextAnchor)4))
                    {
                        Widgets.Label(rect2, "NoProjectSelected".Translate());
                    }
                }
                else
                {
                    float prefixWidth = DefDatabase<KnowledgeCategoryDef>.AllDefs.Max((KnowledgeCategoryDef x) => Text.CalcSize(x.LabelCap + ":").x);
                    DrawProjectProgress(rect5, project, Utility_KnowledgeCategoryDef.Named("EMNC_Basic").LabelCap, prefixWidth);
                    DrawProjectProgress(rect6, project2, Utility_KnowledgeCategoryDef.Named("EMNC_Advanced").LabelCap, prefixWidth);
                }
            }
        }
        private static readonly Texture2D ResearchBarFillTex = SolidColorMaterials.NewSolidColorTexture(TexUI.ResearchMainTabColor);
        private static readonly Texture2D ResearchBarBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.1f));
        private static void DrawProjectProgress(Rect rect, ResearchProjectDef project, string prefixTitle = null, float prefixWidth = 75f)
        {
            Rect rect2 = rect;
            if (!string.IsNullOrEmpty(prefixTitle))
            {
                Rect rect3 = rect2;
                rect3.width = prefixWidth;
                rect2.xMin = rect3.xMax + 10f;
                using (new TextBlock((TextAnchor)3))
                {
                    Widgets.Label(rect3, prefixTitle + ":");
                }
            }

            if (project == null)
            {
                using (new TextBlock((TextAnchor)4))
                {
                    Widgets.Label(rect2, "NoProjectSelected".Translate());
                }

                return;
            }

            rect2 = rect2.ContractedBy(15f);
            Widgets.FillableBar(rect2, project.ProgressPercent, ResearchBarFillTex, ResearchBarBGTex, doBorder: true);
            Text.Anchor = (TextAnchor)4;
            string label = project.ProgressApparentString + " / " + project.CostApparent.ToString("F0");
            Widgets.Label(rect2, label);
            Rect rect4 = rect2;
            rect4.y = rect2.y - 22f;
            rect4.height = 22f;
            float x = Text.CalcSize(project.LabelCap).x;
            Widgets.Label(rect4, project.LabelCap.Truncate(rect4.width));
            if (x > rect4.width)
            {
                TooltipHandler.TipRegion(rect4, project.LabelCap);
                Widgets.DrawHighlightIfMouseover(rect4);
            }

            Text.Anchor = (TextAnchor)0;
        }
    }

    public static class EMWH_MutantWeaponPatch
    {
        [HarmonyPrefix]
        public static bool SetFreshPawnAsMutantPrefix(Pawn pawn, MutantDef mutant)
        {
            if (!pawn.RaceProps.Humanlike)
                return true;

            MutantDefExtension modExtension = mutant.GetModExtension<MutantDefExtension>();
            if (modExtension == null)
                return true;

            if (!modExtension.canEquipWeapon)
                return true;

            RotStage rotStage = (mutant.useCorpseGraphics ? Gen.RandomEnumValue<RotStage>(disallowFirstValue: false) : RotStage.Fresh);

            if (ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.Xenotype != XenotypeDefOf.Baseliner && rotStage == RotStage.Dessicated)
            {
                rotStage = RotStage.Fresh;
            }

            if (rotStage == RotStage.Dessicated)
            {
                pawn.apparel?.DestroyAll();
            }

            MutantUtility.SetPawnAsMutantInstantly(pawn, mutant, rotStage);
            return false;
        }
        [HarmonyPrefix]
        public static bool HandleEquipmentPrefix(Pawn_MutantTracker __instance, Pawn ___pawn)
        {
            MutantDefExtension modExtension = __instance.Def.GetModExtension<MutantDefExtension>();
            if (modExtension == null)
                return true;

            if (!modExtension.canEquipWeapon)
                return true;

            HandleEquipmentAlternative(___pawn, __instance.Def);

            return false;
        }
        private static void HandleEquipmentAlternative(Pawn pawn, MutantDef def)
        {
            if (pawn.apparel == null)
            {
                return;
            }

            // For 1.5
            //if (!def.canWearApparel)
            //{
            //    if (pawn.MapHeld != null)
            //    {
            //        pawn.apparel.DropAll(pawn.Position);
            //    }
            //    else
            //    {
            //        pawn.apparel.DestroyAll();
            //    }
            //}

            // For 1.6
            if (def.disableApparel)
            {
                if (pawn.MapHeld != null)
                {
                    pawn.apparel.DropAll(pawn.Position);
                }
                else
                {
                    pawn.apparel.DestroyAll();
                }
            }

            if (!def.isConsideredCorpse)
            {
                return;
            }

            foreach (Apparel item in pawn.apparel.WornApparel)
            {
                item.WornByCorpse = true;
            }
        }
    }
    public static class EMWH_UnlockForNecronsPatch
    {
        public static void FinalizeInit_Postfix()
        {
            // Replace "Necron" with your actual faction defName
            if (Faction.OfPlayer.def.defName != "EMNC_Szarekhan" && Faction.OfPlayer.def.defName != "EMNC_Sautekh")
                return;

            foreach (var codex in DefDatabase<EntityCodexEntryDef>.AllDefs)
            {
                // Check if any discoveredResearchProjects has a defName starting with "EMNC_Necron_"
                if (codex.discoveredResearchProjects != null &&
                    codex.discoveredResearchProjects.Any(rp => rp.defName != null && rp.defName.StartsWith("EMNC_Necron_")))
                {
                    if (!codex.Discovered)
                    {
                        Find.EntityCodex.Discovered(codex);
                    }
                }
            }
        }
    }
}
