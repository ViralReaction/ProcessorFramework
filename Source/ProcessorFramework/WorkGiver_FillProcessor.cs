using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ProcessorFramework
{
    [HotSwappable]
    public class WorkGiver_FillProcessor : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.GetComponent<MapComponent_Processors>().thingsWithProcessorComp;
        }

        public override bool Prioritized => true;
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            MapComponent_Processors mapComp = pawn.Map.GetComponent<MapComponent_Processors>();
            return !mapComp.thingsWithProcessorComp.Any();
        }
        public override float GetPriority(Pawn pawn, TargetInfo t)
        {
            CompProcessor comp = t.Thing.TryGetComp<CompProcessor>();
            if (comp != null)
            {
                return 1f / comp.SpaceLeft;
            }
            return 0f;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            CompProcessor comp = t.TryGetComp<CompProcessor>();

            if (comp == null || comp.enabledProcesses.EnumerableNullOrEmpty()) return false;

            ProcessDef smallestProcess = null;

            if (comp.Props.parallelProcesses || comp.activeProcesses == null || comp.activeProcesses.Count == 0)
            {
                float smallestFactor = float.MaxValue;

                foreach (var processDef in comp.enabledProcesses.Keys)
                {
                    if (processDef.capacityFactor < smallestFactor)
                    {
                        smallestFactor = processDef.capacityFactor;
                        smallestProcess = processDef;
                    }
                }
            }
            else
            {
                smallestProcess = comp.activeProcesses[0].processDef;
            }
            //process with smallest capacity factor, if not empty and no parallel processes the current active process is taken instead
            if (comp.SpaceLeftFor(smallestProcess) < 1) return false; //check if enough space for one ingredient for smallest process

            if (!comp.TemperatureOk)
            {
                JobFailReason.Is("BadTemperature".Translate().ToLower());
                return false;
            }

            if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null
                || t.IsForbidden(pawn)
                || !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 10, 0, null, forced)
                || t.IsBurning())
            {
                return false;
            }

            if (FindIngredient(pawn, comp) == null)
            {
                JobFailReason.Is("PF_NoIngredient".Translate());
                return false;
            }
            return true;
        }


        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            CompProcessor comp = t.TryGetComp<CompProcessor>();
            Thing ingredient = FindIngredient(pawn, comp);
            ProcessDef processDef = null;
            foreach (KeyValuePair<ProcessDef, ProcessFilter> kvp in comp.enabledProcesses)
            {
                if (!kvp.Value.allowedIngredients.Contains(ingredient.def)) continue;
                processDef = kvp.Key;
                break;
            }

            int count = 0;
            if (processDef != null)
            {
                int spaceLeft = comp.SpaceLeftFor(processDef);
                int stackCount = ingredient.stackCount;

                if (processDef.useStatForEfficiency)
                {
                    float ingredientEfficiency = ingredient.GetStatValue(processDef.efficiencyStat, false);
                    float processBaselineValue = processDef.statBaselineValue;
                    float efficiency = ingredientEfficiency / processBaselineValue;
                    spaceLeft = Mathf.RoundToInt(spaceLeft / efficiency);
                }

                int availableCarrySpace = pawn.carryTracker.AvailableStackSpace(ingredient.def);

                count = Mathf.Min(spaceLeft, stackCount, availableCarrySpace);
            }
            Job job = new Job(DefOf.FillProcessor, t, ingredient)
            {
                count = count
            };
            return job;
        }

        private Thing FindIngredient(Pawn pawn, CompProcessor comp)
        {
            //Needs to check that space left is enough to accomodate one ingredient before sending to JobDriver
            HashSet<ThingDef> validIngredients = comp.ValidIngredients;

            bool validator(Thing x)
            {
                if (x.IsForbidden(pawn)) return false;

                if (!validIngredients.Contains(x.def)) return false;

                ProcessDef processDef = null;
                foreach (var kvp in comp.enabledProcesses)
                {
                    if (kvp.Value.allowedIngredients.Contains(x.def))
                    {
                        processDef = kvp.Key;
                        break;
                    }
                }

                if (processDef == null) return false;

                if (!pawn.CanReserve(x, 1, Mathf.Min(comp.SpaceLeftFor(processDef), x.stackCount, pawn.carryTracker.AvailableStackSpace(x.def))) 
                    || comp.SpaceLeftFor(processDef) < 1)
                {
                    return false;
                }
                return true;
            }
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator, pawn.Map.GetComponent<MapComponent_Processors>().PotentialIngredients);
        }
    }
}