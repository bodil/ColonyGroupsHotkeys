using System.Collections.Generic;
using RimWorld;
using TacticalGroups;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ColonyGroupsHotkeys
{
    public class Gizmo_BattleStationsButton : Command_Action
    {
        public Pawn? owner;

        public override void ProcessInput(Event ev)
        {
            owner?.ToBattleStations();
            TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
        }

        public static Command MakeGizmo(Pawn owner) => new Gizmo_BattleStationsButton
        {
            owner = owner,
            defaultLabel = "ColGrpHotkeys_gizmo_battleStations_label".Translate(),
            defaultDesc = "ColGrpHotkeys_gizmo_battleStations_desc".Translate(),
            hotKey = KeyBindings.BattleStationsAction,
            icon = Textures.BattleStationsButton,
            activateSound = SoundDefOf.Tick_Tiny,
        };
    }

    public class JobDriver_ToBattleStations : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddFailCondition(() => !pawn.IsColonistPlayerControlled || pawn.Downed || pawn.drafter == null || pawn.GetBattleStation() == null);
            var toil = new Toil
            {
                initAction = () =>
                {
                    pawn.drafter.Drafted = true;
                    var position = RCellFinder.BestOrderedGotoDestNear(TargetLocA, pawn);
                    var job = JobMaker.MakeJob(JobDefOf.Goto, position);
                    job.locomotionUrgency = LocomotionUrgency.Sprint;
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.DraftedOrder);
#if RIMWORLD_1_2
                    MoteMaker.MakeStaticMote(position, pawn.Map, ThingDefOf.Mote_FeedbackGoto);
#else
                    MoteMaker.MakeStaticMote(position, pawn.Map, ThingDefOf.Mote_RolePositionHighlight);
#endif
                }
            };
            yield return toil;
        }
    }
}
