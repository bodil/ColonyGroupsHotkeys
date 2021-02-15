using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ColonyGroupsHotkeys
{
    public static class Extensions
    {
        public static TaggedString Translate(this Modifier mod)
        {
            return Utils.EnumTranslationKey(mod).Translate();
        }

        public static bool MatchModifier(this Modifier mod, EventModifiers modifiers)
        {
            switch (mod)
            {
                case Modifier.Shift:
                    return modifiers == EventModifiers.Shift;
                case Modifier.Alt:
                    return modifiers == EventModifiers.Alt;
                case Modifier.Control:
                    return modifiers == EventModifiers.Control;
                case Modifier.ShiftAlt:
                    return modifiers == (EventModifiers.Shift | EventModifiers.Alt);
                case Modifier.ShiftControl:
                    return modifiers == (EventModifiers.Shift | EventModifiers.Control);
                case Modifier.ControlAlt:
                    return modifiers == (EventModifiers.Control | EventModifiers.Alt);
                case Modifier.ShiftControlAlt:
                    return modifiers == (EventModifiers.Shift | EventModifiers.Control | EventModifiers.Alt);
                default: return false;
            }
        }

    }

    public enum Modifier { Disabled, Shift, Control, Alt, ShiftAlt, ShiftControl, ControlAlt, ShiftControlAlt }

    public class Settings : ModSettings
    {
        public bool beQuiet;
        public Modifier groupSetModifier;
        public Modifier groupDraftModifier;
        public Modifier groupUndraftModifier;
        public Modifier groupBattleStationsModifier;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref beQuiet, "beQuiet", false);
            Scribe_Values.Look(ref groupSetModifier, "groupSetModifier", Modifier.Disabled);
            Scribe_Values.Look(ref groupDraftModifier, "groupDraftModifier", Modifier.Disabled);
            Scribe_Values.Look(ref groupUndraftModifier, "groupUndraftModifier", Modifier.Disabled);
            Scribe_Values.Look(ref groupBattleStationsModifier, "groupBattleStationsModifier", Modifier.Disabled);
            base.ExposeData();
        }

        public void SetGroupSetModifier(Modifier mod)
        {
            groupSetModifier = mod;
            if (groupDraftModifier == mod) groupDraftModifier = Modifier.Disabled;
            if (groupUndraftModifier == mod) groupUndraftModifier = Modifier.Disabled;
            if (groupBattleStationsModifier == mod) groupBattleStationsModifier = Modifier.Disabled;
        }

        public void SetGroupDraftModifier(Modifier mod)
        {
            groupDraftModifier = mod;
            if (groupSetModifier == mod) groupSetModifier = Modifier.Disabled;
            if (groupUndraftModifier == mod) groupUndraftModifier = Modifier.Disabled;
            if (groupBattleStationsModifier == mod) groupBattleStationsModifier = Modifier.Disabled;
        }

        public void SetGroupUndraftModifier(Modifier mod)
        {
            groupUndraftModifier = mod;
            if (groupDraftModifier == mod) groupDraftModifier = Modifier.Disabled;
            if (groupSetModifier == mod) groupSetModifier = Modifier.Disabled;
            if (groupBattleStationsModifier == mod) groupBattleStationsModifier = Modifier.Disabled;
        }

        public void SetGroupBattleStationsModifier(Modifier mod)
        {
            groupBattleStationsModifier = mod;
            if (groupDraftModifier == mod) groupDraftModifier = Modifier.Disabled;
            if (groupUndraftModifier == mod) groupUndraftModifier = Modifier.Disabled;
            if (groupSetModifier == mod) groupSetModifier = Modifier.Disabled;
        }

        private static List<FloatMenuOption> EnumSelector<A>(Action<A> action) where A : Enum
        {
            var options = new List<FloatMenuOption>();
            foreach (int item in Enum.GetValues(typeof(A)))
            {
                options.Add(new FloatMenuOption(("ColGrpHotkeys_" + typeof(A).Name + "_" + Enum.GetName(typeof(A), item)).Translate(), delegate
                {
                    action((A)Enum.ToObject(typeof(A), item));
                }));
            }
            return options;
        }

        public static void EnumButton<A>(Listing_Standard list, TaggedString label, A value, Action<A> update) where A : Enum
        {
            if (list.ButtonTextLabeled(label, Utils.EnumTranslationKey<A>(value).Translate()))
            {
                FloatMenu dropdown = new FloatMenu(EnumSelector<A>(update)) { vanishIfMouseDistant = true };
                Find.WindowStack.Add(dropdown);
            }
        }

        public void Dialog(Rect inRect)
        {
            var rect = inRect.ContractedBy(10);
            Listing_Standard list = new Listing_Standard
            {
                ColumnWidth = (rect.width / 2) - 17
            };
            list.Begin(rect);

            Text.Font = GameFont.Medium;
            list.Label("ColGrpHotkeys_settings_options".Translate());
            Text.Font = GameFont.Small;
            list.GapLine();
            list.CheckboxLabeled("ColGrpHotkeys_settings_beQuiet_title".Translate(), ref beQuiet, "ColGrpHotkeys_settings_beQuiet_desc".Translate());

            list.NewColumn();
            Text.Font = GameFont.Medium;
            list.Label("ColGrpHotkeys_settings_modifiers".Translate());
            Text.Font = GameFont.Small;
            list.GapLine();
            EnumButton(list, "ColGrpHotkeys_settings_groupSetModifier_title".Translate(), groupSetModifier, SetGroupSetModifier);
            EnumButton(list, "ColGrpHotkeys_settings_groupDraftModifier_title".Translate(), groupDraftModifier, SetGroupDraftModifier);
            EnumButton(list, "ColGrpHotkeys_settings_groupUndraftModifier_title".Translate(), groupUndraftModifier, SetGroupUndraftModifier);
            EnumButton(list, "ColGrpHotkeys_settings_groupBattleStationsModifier_title".Translate(), groupBattleStationsModifier, SetGroupBattleStationsModifier);

            list.End();

        }
    }
}