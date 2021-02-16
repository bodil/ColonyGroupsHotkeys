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
        public static KeyBindingDef? SelectColony1;
        public static KeyBindingDef? SelectColony2;
        public static KeyBindingDef? SelectColony3;
        public static KeyBindingDef? SelectColony4;
        public static KeyBindingDef? SelectColony5;
        public static KeyBindingDef? SelectColony6;
        public static KeyBindingDef? SelectColony7;
        public static KeyBindingDef? SelectColony8;
        public static KeyBindingDef? SelectColony9;
        public static KeyBindingDef? SelectColony10;
        public static KeyBindingDef? SelectColony11;
        public static KeyBindingDef? SelectColony12;
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

        public static List<KeyBindingDef?> ColonyGroupKeys() =>
            new List<KeyBindingDef?> {
                SelectColony1,
                SelectColony2,
                SelectColony3,
                SelectColony4,
                SelectColony5,
                SelectColony6,
                SelectColony7,
                SelectColony8,
                SelectColony9,
                SelectColony10,
                SelectColony11,
                SelectColony12,

            };

        public static List<KeyBindingDef?> PawnGroupKeys() =>
            new List<KeyBindingDef?> {
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

    }
}
