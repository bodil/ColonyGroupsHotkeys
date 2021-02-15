using System;
using System.Reflection;
using HarmonyLib;
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
}
