using System.Linq;
using UnityEngine;
using Verse;

namespace ColonyGroupsHotkeys
{
    public class ColonyGroupsHotkeys : Mod
    {
        public static ColonyGroupsHotkeys? Instance;

        public override string SettingsCategory()
        {
            return "ColGrpHotkeys_settings".Translate();
        }

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
                    var group = Utils.GetCurrentColonyGroup();
                    if (group != null)
                        Utils.SelectGroup(group);
                    Event.current.Use();
                    return;
                }
                if (KeyBindings.DraftCurrentColony?.JustPressed == true)
                {
                    var group = Utils.GetCurrentColonyGroup();
                    if (group != null)
                        Utils.DraftGroup(group);
                    Event.current.Use();
                    return;
                }
                if (KeyBindings.UndraftCurrentColony?.JustPressed == true)
                {
                    var group = Utils.GetCurrentColonyGroup();
                    if (group != null)
                        Utils.UndraftGroup(group);
                    Event.current.Use();
                    return;
                }
                if (KeyBindings.BattleStationsCurrentColony?.JustPressed == true)
                {
                    var group = Utils.GetCurrentColonyGroup();
                    if (group != null)
                        Utils.ToBattleStations(group);
                    Event.current.Use();
                    return;
                }
                foreach (var obj in KeyBindings.PawnGroupKeys().Select((key, index) => new { key, index }))
                {
                    var key = obj.key;
                    var index = obj.index;
                    if (key?.JustPressed == true)
                    {
                        if (settings.groupSetModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Utils.SetGroupToCurrentSelection, Utils.CreateGroup);
                        }
                        else if (settings.groupDraftModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Utils.DraftGroup);
                        }
                        else if (settings.groupUndraftModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Utils.UndraftGroup);
                        }
                        else if (settings.groupBattleStationsModifier.MatchModifier(Event.current.modifiers))
                        {
                            Utils.ActOnPawnGroup(index, Utils.ToBattleStations);
                        }
                        else
                        {
                            Utils.ActOnPawnGroup(index, Utils.SelectGroup);
                        }
                        Event.current.Use();
                        return;
                    }
                }
            }
        }
    }
}
