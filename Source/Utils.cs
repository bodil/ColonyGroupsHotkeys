using System;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using TacticalGroups;
using Verse;

namespace ColonyGroupsHotkeys
{
    public static class Utils
    {
        public static string EnumTranslationKey<A>(A value) where A : Enum =>
            $"ColGrpHotkeys_{typeof(A).Name}_{Enum.GetName(typeof(A), value)}";

        public static void Message(TaggedString message)
        {
            if (ColonyGroupsHotkeys.Instance?.settings.beQuiet != true)
                Messages.Message(message, MessageTypeDefOf.SilentInput);
        }

        public static void Error(TaggedString message) => Messages.Message(message, MessageTypeDefOf.RejectInput);

        public static ColonyGroup? GetCurrentColonyGroup() =>
            TacticUtils.AllColonyGroups.FirstOrDefault(colony => colony.Map == Find.CurrentMap);

        public static ColonistGroup? GetColonyGroupByIndex(int index) =>
             TacticUtils.AllColonyGroups.Select(group => (ColonistGroup)group)
                   .Concat(TacticUtils.AllCaravanGroups.Select(group => (ColonistGroup)group))
                   .ElementAtOrDefault(index);

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
            return groups[(groups.Count - 1) - index];
        }

        public static void ActOnPawnGroup(int index, Action<PawnGroup> action) => ActOnPawnGroup(index, action, (_) => { });

        public static void ActOnPawnGroup(int index, Action<PawnGroup> action, Action<int> empty)
        {
            if (GetActivePawnGroupByIndex(index) is { } group) { action(group); } else { empty(index); }
        }

        public static void ActOnColonyGroup(int index, Action<ColonistGroup> action)
        {
            if (GetColonyGroupByIndex(index) is { } group) { action(group); }
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
            Debug.Assert(index == groups.Count);
            TacticUtils.TacticalGroups.AddGroup(selected);
            var group = TacticUtils.TacticalGroups.pawnGroups[0];
            Message("ColGrpHotkeys_msg_groupCreated".Translate(group.curGroupName, group.pawns.Count));
        }
    }
}
