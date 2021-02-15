using System;
using System.Collections.Generic;
using RimWorld;
using TacticalGroups;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ColonyGroupsHotkeys
{
    public class Utils
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
            foreach (var colony in TacticUtils.AllColonyGroups)
            {
                if (colony.Map == Find.CurrentMap)
                {
                    return colony;
                }
            }
            return null;
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
            groups.Reverse();
            return groups[index];
        }

        public static void ActOnPawnGroup(int index, Action<ColonistGroup> action)
        {
            ActOnPawnGroup(index, action, (_) => { });
        }

        public static void ActOnPawnGroup(int index, Action<ColonistGroup> action, Action<int> empty)
        {
            var group = GetActivePawnGroupByIndex(index);
            if (group != null)
            {
                action(group);
            }
            else
            {
                empty(index);
            }
        }

        public static int CountPred(ColonistGroup group, Func<Pawn, bool> pred)
        {
            int count = 0;
            foreach (var pawn in group.pawns)
            {
                if (pred(pawn))
                {
                    count++;
                }
            }
            return count;
        }

        public static void SelectGroup(ColonistGroup group)
        {
            group.SelectAll();
            Message("ColGrpHotkeys_msg_selectedGroup".Translate(group.curGroupName));
        }

        public static void DraftGroup(ColonistGroup group)
        {
            DraftGroup(group, true);
        }

        public static void DraftGroup(ColonistGroup group, bool withMessages)
        {
            var drafted = CountPred(group, (pawn) => pawn.drafter.Drafted);
            group.Draft();
            var newlyDrafted = CountPred(group, (pawn) => pawn.drafter.Drafted) - drafted;
            if (newlyDrafted > 0)
            {
                TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
                if (withMessages)
                    Message("ColGrpHotkeys_msg_drafted".Translate(newlyDrafted));
            }
            else
            {
                if (withMessages)
                    Error("ColGrpHotkeys_msg_alreadyDrafted".Translate());
            }
            group.SelectAll();
        }

        public static void UndraftGroup(ColonistGroup group)
        {
            var drafted = CountPred(group, (pawn) => pawn.drafter.Drafted);
            group.Undraft();
            var undrafted = drafted - CountPred(group, (pawn) => pawn.drafter.Drafted);
            if (undrafted > 0)
            {
                Message("ColGrpHotkeys_msg_undrafted".Translate(undrafted));
            }
            else
            {
                Error("ColGrpHotkeys_msg_alreadyUndrafted".Translate());
            }
        }

        public static void ToBattleStations(ColonistGroup group)
        {
            DraftGroup(group, false);
            var count = 0;
            foreach (Pawn activePawn in group.ActivePawns)
            {
                if (group.formations?.ContainsKey(activePawn) ?? false)
                {
                    Job job = JobMaker.MakeJob(JobDefOf.Goto, group.formations[activePawn]);
                    job.locomotionUrgency = LocomotionUrgency.Sprint;
                    activePawn.jobs.TryTakeOrderedJob(job);
                    count++;
                }
            }
            if (count > 0)
            {
                Message("ColGrpHotkeys_msg_battleStations".Translate(count));
            }
            else
            {
                Error("ColGrpHotkeys_msg_noBattleStations".Translate());
            }
        }

        public static void SetGroupToCurrentSelection(ColonistGroup group)
        {
            var selectedPawns = Find.Selector.SelectedPawns;
            if (selectedPawns.Count == 0)
            {
                Error("ColGrpHotkeys_msg_noSelection".Translate());
                return;
            }
            var removed = new List<Pawn>();
            foreach (var pawn in group.pawns)
            {
                if (!selectedPawns.Contains(pawn))
                {
                    removed.Add(pawn);
                }
            }
            foreach (var pawn in selectedPawns)
            {
                group.Add(pawn);
            }
            foreach (var pawn in removed)
            {
                group.Disband(pawn);
            }
            Message("ColGrpHotkeys_msg_groupUpdated".Translate(group.curGroupName, selectedPawns.Count));
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
