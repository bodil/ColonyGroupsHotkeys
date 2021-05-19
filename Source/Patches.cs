using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ColonyGroupsHotkeys.Patches
{
    [StaticConstructorOnStartup]
    public class Patcher
    {
        public static Harmony instance;
        static Patcher()
        {
            instance = new Harmony("bodilpwnz.ColonyGroupsHotkeys");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(UIRoot))]
    [HarmonyPatch("UIRootOnGUI")]
    [HarmonyPatch(new Type[0])]
    internal static class UIRoot_OnGUI_Patch
    {
        [HarmonyPostfix]
        private static void OnGUIHook()
        {
            ColonyGroupsHotkeys.Instance?.OnGUI();
        }
    }

    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    internal static class DraftController_GetGizmos_Patch
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> values, Pawn_DraftController __instance)
        {
            var pawn = __instance.pawn;
            var newGizmos = values.ToList();
            if (pawn?.GetBattleStation() != null)
            {
                (var draft, var draftIndex) = values.Select((gizmo, index) => (gizmo as Command_Toggle, index)).Where(item => item.Item1 != null && item.Item1.icon == TexCommand.Draft).FirstOrDefault();
                var insertAtIndex = newGizmos.Count > 0 ? 1 : 0;
                var draftAllowed = true;
                if (draft != null)
                {
                    draftAllowed = !draft.disabled;
                    insertAtIndex = draftIndex + 1;
                }
                if (draftAllowed)
                {
                    newGizmos.Insert(insertAtIndex, Gizmo_BattleStationsButton.MakeGizmo(pawn));
                }
            }
            foreach (var gizmo in newGizmos) { yield return gizmo; }
        }
    }
}
