using System;
using System.Linq;
using RimWorld;
using TacticalGroups;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ColonyGroupsHotkeys
{
    public static class Extensions
    {
        // Modifier methods

        public static TaggedString Translate(this Modifier mod) => Utils.EnumTranslationKey(mod).Translate();

        public static bool MatchModifier(this Modifier mod, EventModifiers modifiers) => mod switch
        {
            Modifier.Shift => modifiers == EventModifiers.Shift,
            Modifier.Alt => modifiers == EventModifiers.Alt,
            Modifier.Control => modifiers == EventModifiers.Control,
            Modifier.ShiftAlt => modifiers == (EventModifiers.Shift | EventModifiers.Alt),
            Modifier.ShiftControl => modifiers == (EventModifiers.Shift | EventModifiers.Control),
            Modifier.ControlAlt => modifiers == (EventModifiers.Control | EventModifiers.Alt),
            Modifier.ShiftControlAlt => modifiers == (EventModifiers.Shift | EventModifiers.Control | EventModifiers.Alt),
            _ => false
        };

        // TacticalGroups.ColonistGroup methods

        public static int CountPred(this ColonistGroup group, Func<Pawn, bool> pred) => group.pawns.Count(pred);

        public static void SelectGroup(this ColonistGroup group)
        {
            group.SelectAll();
            Utils.Message("ColGrpHotkeys_msg_selectedGroup".Translate(group.curGroupName));
        }

        public static void DraftGroup(this ColonistGroup group) => DraftGroup(group, true);

        public static void DraftGroup(this ColonistGroup group, bool withMessages = true)
        {
            var drafted = group.CountPred(pawn => pawn.drafter.Drafted);
            group.Draft();
            var newlyDrafted = group.CountPred(pawn => pawn.drafter.Drafted) - drafted;
            if (newlyDrafted > 0)
            {
                TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
                if (withMessages)
                    Utils.Message("ColGrpHotkeys_msg_drafted".Translate(newlyDrafted));
            }
            else
            {
                if (withMessages)
                    Utils.Error("ColGrpHotkeys_msg_alreadyDrafted".Translate());
            }
            group.SelectAll();
        }

        public static void UndraftGroup(this ColonistGroup group)
        {
            var drafted = group.CountPred(pawn => pawn.drafter.Drafted);
            group.Undraft();
            var undrafted = drafted - group.CountPred(pawn => pawn.drafter.Drafted);
            if (undrafted > 0)
            {
                Utils.Message("ColGrpHotkeys_msg_undrafted".Translate(undrafted));
            }
            else
            {
                Utils.Error("ColGrpHotkeys_msg_alreadyUndrafted".Translate());
            }
        }

        public static void ToBattleStations(this ColonistGroup group)
        {
            DraftGroup(group, false);
            var count = 0;
            foreach (Pawn activePawn in group.ActivePawns.Where(pawn => group.formations?.ContainsKey(pawn) ?? false))
            {
                var job = JobMaker.MakeJob(JobDefOf.Goto, group.formations[activePawn]);
                job.locomotionUrgency = LocomotionUrgency.Sprint;
                activePawn.jobs.TryTakeOrderedJob(job);
                count++;
            }
            if (count > 0)
            {
                Utils.Message("ColGrpHotkeys_msg_battleStations".Translate(count));
            }
            else
            {
                Utils.Error("ColGrpHotkeys_msg_noBattleStations".Translate());
            }
        }

        public static void SetGroupToCurrentSelection(this ColonistGroup group)
        {
            var selectedPawns = Find.Selector.SelectedPawns;
            if (selectedPawns.Count == 0)
            {
                Utils.Error("ColGrpHotkeys_msg_noSelection".Translate());
                return;
            }
            var removed = selectedPawns.Except(group.pawns).ToList();
            foreach (var pawn in selectedPawns)
            {
                group.Add(pawn);
            }
            foreach (var pawn in removed)
            {
                group.Disband(pawn);
            }
            Utils.Message("ColGrpHotkeys_msg_groupUpdated".Translate(group.curGroupName, selectedPawns.Count));
        }

    }
}