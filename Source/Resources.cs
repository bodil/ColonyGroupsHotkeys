using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyGroupsHotkeys
{
    [DefOf]
    public static class Jobs
    {
        public static JobDef? ColGrpHotkeys_Job_ToBattleStations;
    }

    [StaticConstructorOnStartup]
    public static class Textures
    {
        public static readonly Texture2D BattleStationsButton = ContentFinder<Texture2D>.Get("UIPositionLargeActive");
    }
}
