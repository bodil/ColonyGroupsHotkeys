using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ColonyGroupsHotkeys
{
    [DefOf]
    public static class KeyBindings
    {
        public static KeyBindingDef? SelectCurrentColony;
        public static KeyBindingDef? DraftCurrentColony;
        public static KeyBindingDef? UndraftCurrentColony;
        public static KeyBindingDef? BattleStationsCurrentColony;
        public static KeyBindingDef? SelectGroup1;
        public static KeyBindingDef? SelectGroup2;
        public static KeyBindingDef? SelectGroup3;
        public static KeyBindingDef? SelectGroup4;
        public static KeyBindingDef? SelectGroup5;
        public static KeyBindingDef? SelectGroup6;
        public static KeyBindingDef? SelectGroup7;
        public static KeyBindingDef? SelectGroup8;
        public static KeyBindingDef? SelectGroup9;
        public static KeyBindingDef? SelectGroup10;
        public static KeyBindingDef? SelectGroup11;
        public static KeyBindingDef? SelectGroup12;

        public static List<KeyBindingDef?> PawnGroupKeys()
        {
            KeyBindingDef?[] keys = {
                SelectGroup1,
                SelectGroup2,
                SelectGroup3,
                SelectGroup4,
                SelectGroup5,
                SelectGroup6,
                SelectGroup7,
                SelectGroup8,
                SelectGroup9,
                SelectGroup10,
                SelectGroup11,
                SelectGroup12,
            };
            return new List<KeyBindingDef?>(keys);
        }
    }
}
