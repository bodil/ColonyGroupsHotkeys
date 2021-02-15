using System.Linq;
using TacticalGroups;
using UnityEngine;
using Verse;

namespace ColonyGroupsHotkeys
{


    public class ColonyGroupsHotkeys : Mod
    {
        public static ColonyGroupsHotkeys? Instance;

        public override string SettingsCategory() => "ColGrpHotkeys_settings".Translate();

        public Settings settings;

        public ColonyGroupsHotkeys(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Settings>();
            Instance = this;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.Dialog(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public void OnGUI()
        {
            if (Current.ProgramState == ProgramState.Playing && Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
            {
                if (KeyBindings.SelectCurrentColony?.JustPressed == true)
                {
                    Utils.GetCurrentColonyGroup()?.SelectGroup();
                    Event.current.Use();
                    return;
                }
                if (KeyBindings.DraftCurrentColony?.JustPressed == true)
                {
                    Utils.GetCurrentColonyGroup()?.DraftGroup();
                    Event.current.Use();
                    return;
                }
                if (KeyBindings.UndraftCurrentColony?.JustPressed == true)
                {
                    Utils.GetCurrentColonyGroup()?.UndraftGroup();
                    Event.current.Use();
                    return;
                }
                if (KeyBindings.BattleStationsCurrentColony?.JustPressed == true)
                {
                    Utils.GetCurrentColonyGroup()?.ToBattleStations();
                    Event.current.Use();
                    return;
                }
                foreach (var (key, index) in KeyBindings.PawnGroupKeys().Select((key, index) => (key, index)))
                {
                    if (key?.JustPressed == true)
                    {
                        if (settings.groupSetModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Extensions.SetGroupToCurrentSelection, Utils.CreateGroup);
                        }
                        else if (settings.groupDraftModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Extensions.DraftGroup);
                        }
                        else if (settings.groupUndraftModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Extensions.UndraftGroup);
                        }
                        else if (settings.groupBattleStationsModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Extensions.ToBattleStations);
                        }
                        else
                        {
                            Utils.ActOnPawnGroup(index, Extensions.SelectGroup);
                        }
                        Event.current.Use();
                        return;
                    }
                }
            }
        }
    }
}
