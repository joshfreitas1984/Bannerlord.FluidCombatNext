﻿using Bannerlord.FluidCombatNext.Helper;
using Bannerlord.FluidCombatNext.HotKeys;
using HarmonyLib;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace Bannerlord.FluidCombatNext.Patches
{
    //Patch the polling thread that handles input
    [HarmonyPatch(typeof(MissionMainAgentController), "ControlTick")]
    public static class OverheadAttackPatch
    {
        /// <summary>
        /// Whether or not we have first recorded the key down
        /// </summary>
        public static bool CachedKeyDown = false;
       
        public static void Postfix(MissionMainAgentController __instance)
        {
            try
            {
                if (OverheadAttackKey.IsKeyDown && !CachedKeyDown)
                {
                    //Cancel any previous attack by adding a block to movement flags
                    __instance.Mission.MainAgent.MovementFlags |= Agent.MovementControlFlag.DefendUp;
                    CachedKeyDown = true;
                }
                else if (CachedKeyDown)
                {
                    //Add attack to movement flags
                    __instance.Mission.MainAgent.MovementFlags |= Agent.MovementControlFlag.AttackUp;
                }
            }
            catch (Exception ex)
            {
                MessageHelper.HandleError("OverheadAttack Failed", ex.ToString());
            }
        }
    }
}
