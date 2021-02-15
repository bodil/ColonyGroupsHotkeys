using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TacticalGroups;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ColonyGroupsHotkeys
{
    public static class Utils
    {
        public static string EnumTranslationKey<A>(A value) where A : Enum
        {
            return "ColGrpHotkeys_" + typeof(A).Name + "_" + Enum.GetName(typeof(A), value);
        }

        public static void Message(TaggedString message)
        {
            if (ColonyGroupsHotkeys.Instance?.settings.beQuiet != true)
                Messages.Message(message, MessageTypeDefOf.SilentInput);
        }

        public static void Error(TaggedString message)
        {
            Messages.Message(message, MessageTypeDefOf.RejectInput);
        }

        public static ColonyGroup? GetCurrentColonyGroup()
        {
            return TacticUtils.AllColonyGroups.FirstOrDefault(colony => colony.Map == Find.CurrentMap);
        }

        public static PawnGroup? GetActivePawnGroupByIndex(int index)
        {
            var colony = GetCurrentColonyGroup();
            if (colony == null)
            {
                return null;
            }

            var groups = TacticUtils.GetAllPawnGroupFor(colony);
            if (index >= groups.Count)
            {
                return null;
            }
            return groups[groups.Count - index];
        }

        public static void ActOnPawnGroup(int index, Action<ColonistGroup> action) => ActOnPawnGroup(index, action, (_) => { });

        public static void ActOnPawnGroup(int index, Action<ColonistGroup> action, Action<int> empty)
        {
            if (GetActivePawnGroupByIndex(index) is { } group) { action(group); } else { empty(index); }
        }

        public static void CreateGroup(int index)
        {
            var selected = Find.Selector.SelectedPawns;
            if (selected.Count == 0)
            {
                Error("ColGrpHotkeys_msg_noSelection".Translate());
                return;
            }
            var colony = GetCurrentColonyGroup();
            if (colony == null) return;
            var groups = TacticUtils.GetAllPawnGroupFor(colony);
            if (index > groups.Count)
            {
                Error("ColGrpHotkeys_msg_groupOutOfBounds".Translate(index + 1, groups.Count + 1));
                return;
            }
            if (index < groups.Count)
            {
                throw new Exception("oh no.");
            }
            TacticUtils.TacticalGroups.AddGroup(selected);
            var group = TacticUtils.TacticalGroups.pawnGroups[0];
            Message("ColGrpHotkeys_msg_groupCreated".Translate(group.curGroupName, group.pawns.Count));
        }
    }
}
