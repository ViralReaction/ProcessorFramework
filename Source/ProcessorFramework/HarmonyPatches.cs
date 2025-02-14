using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace ProcessorFramework
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("Syrchalis.Rimworld.UniversalFermenter");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
    [HarmonyPatch(typeof(Building_FermentingBarrel), nameof(Building_FermentingBarrel.GetInspectString))]
    public class OldBarrel_GetInspectStringPatch
    {
        [HarmonyPrefix]
        public static bool OldBarrel_GetInspectString_Postfix(ref string __result)
        {
            __result = "PF_OldBarrelInspectString".Translate();
            return false;
        }
    }

    [HarmonyPatch(typeof(MainTabWindow_Inspect), nameof(MainTabWindow_Inspect.CurTabs), MethodType.Getter)]
    public class CurTabsPatch
    {
        private static List<object> _cachedSelectedObjects = new();
        private static IEnumerable<InspectTabBase> _cachedResult;

        [HarmonyPostfix]
        public static void CurTabs_Postfix(ref IEnumerable<InspectTabBase> __result)
        {
            List<object> objects = Find.Selector.SelectedObjects;
            if (objects == null || objects.Count == 0) return;

            int objectCount = objects.Count;

            // Ensure cached selection exists before comparing
            if (_cachedSelectedObjects.Count == objectCount)
            {
                if (_cachedResult == null) return;
                bool sameSelection = true;
                for (int i = 0; i < objectCount; i++)
                {
                    if (_cachedSelectedObjects[i] != objects[i]) // Compare by reference
                    {
                        sameSelection = false;
                        break;
                    }
                }

                if (sameSelection)
                {
                    __result = _cachedResult;
                    return;
                }
            }

            // Reset cache when selection differs
            _cachedSelectedObjects.Clear();
            _cachedSelectedObjects.AddRange(objects);
            _cachedResult = null;

            // Ensure objects is still valid before accessing index 0
            if (objects[0] is not ThingWithComps firstThing || firstThing.Faction != Faction.OfPlayerSilentFail)
            {
                return;
            }

            // Ensure all objects are the same type and belong to the player
            for (int i = 1; i < objectCount; i++)
            {
                if (objects[i] is not ThingWithComps thing || thing.Faction != Faction.OfPlayerSilentFail || thing.def != firstThing.def)
                {
                    return;
                }
            }
            if (firstThing.TryGetComp<CompProcessor>() == null) return;

            _cachedResult = firstThing.GetInspectTabs();
            __result = _cachedResult;

        }
    }
}
