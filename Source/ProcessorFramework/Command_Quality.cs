using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ProcessorFramework
{
    public class Command_Quality : Command_Action
    {
        public QualityCategory qualityToTarget;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                List<FloatMenuOption> qualityfloatMenuOptions = new List<FloatMenuOption>();
                foreach (QualityCategory quality in Enum.GetValues(typeof(QualityCategory)))
                {
                    qualityfloatMenuOptions.Add(
                        new FloatMenuOption(
                            quality.GetLabel(),
                            () => ChangeQuality(qualityToTarget, quality),
                            (Texture2D)ProcessorFramework_Utility.qualityMaterials[quality].mainTexture,
                            Color.white,
                            MenuOptionPriority.Default,
                            null,
                            null,
                            0f,
                            null,
                            null
                        )
                    );
                }
                return qualityfloatMenuOptions;
            }
        }

        internal static void ChangeQuality(QualityCategory qualityToTarget, QualityCategory quality)
        {
            foreach (Thing thing in Find.Selector.SelectedObjects.OfType<Thing>())
            {
                CompProcessor comp = thing.TryGetComp<CompProcessor>();
                if (comp != null && comp.activeProcesses.Any(x => x.processDef.usesQuality))
                {
                    foreach (ActiveProcess activeProcess in comp.activeProcesses/*.Where(x => x.TargetQuality == qualityToTarget)*/)
                    {
                        activeProcess.TargetQuality = quality;
                        comp.cachedTargetQualities[activeProcess.processDef] = quality;
                    }
                }
            }
        }
    }
}
