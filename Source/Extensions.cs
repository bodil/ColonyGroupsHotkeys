using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
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

        // Pawn methods

        public static ColonistGroup? GetPawnOrColonyGroup(this Pawn pawn) =>
            TacticUtils.AllPawnGroups.Select(group => (ColonistGroup)group).Concat(TacticUtils.AllColonyGroups.Select(group => (ColonistGroup)group)).Where(group => group.pawns.Contains(pawn)).FirstOrDefault();

        public static IntVec3? GetBattleStation(this Pawn pawn, ColonistGroup? group = null)
        {
            try
            {
                return (group ?? pawn.GetPawnOrColonyGroup())?.formations?[0]?.formations?[pawn];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public static bool ToBattleStations(this Pawn pawn, ColonistGroup? group = null)
        {
            if (pawn.GetBattleStation(group) is IntVec3 position)
            {
                return pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(Jobs.ColGrpHotkeys_Job_ToBattleStations, position), JobTag.DraftedOrder);
            }
            else
            {
                return false;
            }
        }

        // TacticalGroups.ColonistGroup methods

        public static int CountPred(this ColonistGroup group, Func<Pawn, bool> pred) => group.pawns.Count(pred);

        public static void SelectGroup(this ColonistGroup group)
        {
            if (group is ColonyGroup colonyGroup)
            {
                if (ColonyGroupsHotkeys.Instance?.settings.stayOnWorldMap == true && WorldRendererUtility.WorldRenderedNow)
                {
                    CameraJumper.TryJump(CameraJumper.GetWorldTargetOfMap(colonyGroup.Map));
                }
                else
                {
                    var flag = CameraJumper.TryHideWorld();
                    if (Find.CurrentMap != colonyGroup.Map)
                    {
                        Current.Game.CurrentMap = colonyGroup.Map;
                        if (!flag)
                        {
                            SoundDefOf.MapSelected.PlayOneShotOnCamera();
                        }
                    }
                }
            }
            else if (group is CaravanGroup caravanGroup)
            {
                // CaravanGroup doesn't expose the Caravan object, so get it through the group's first pawn.
                var caravan = caravanGroup.pawns[0].GetCaravan();
                CameraJumper.TryJump(caravan);
            }
            if (Utils.SelectOrJumpToGroup(group))
            {
                Utils.Message("ColGrpHotkeys_msg_selectedGroup".Translate(group.curGroupName));
            }
        }

        public static void DraftGroup(this ColonistGroup group)
        {
            group.SelectGroup();
            var drafted = group.CountPred(pawn => pawn.drafter.Drafted);
            group.Draft();
            var newlyDrafted = group.CountPred(pawn => pawn.drafter.Drafted) - drafted;
            if (newlyDrafted > 0)
            {
                TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
                Utils.Message("ColGrpHotkeys_msg_drafted".Translate(newlyDrafted));
            }
            else
            {
                Utils.Error("ColGrpHotkeys_msg_alreadyDrafted".Translate());
            }
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
            group.SelectGroup();
            var count = group.ActivePawns.Select(pawn => pawn.ToBattleStations()).Where(flag => flag).Count();
            if (count > 0)
            {
                TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
                Utils.Message("ColGrpHotkeys_msg_battleStations".Translate(count));
            }
            else
            {
                Utils.Error("ColGrpHotkeys_msg_noBattleStations".Translate());
            }
        }

        public static void SetGroupToCurrentSelection(this PawnGroup group)
        {
            var selectedPawns = Find.Selector.SelectedPawns;
            if (selectedPawns.Count == 0)
            {
                Utils.Error("ColGrpHotkeys_msg_noSelection".Translate());
                return;
            }
            group.pawns = new List<Pawn>();
            group.pawnIcons.Clear();
            group.Add(selectedPawns);
            Utils.Message("ColGrpHotkeys_msg_groupUpdated".Translate(group.curGroupName, group.pawns.Count));
        }

    }
}
