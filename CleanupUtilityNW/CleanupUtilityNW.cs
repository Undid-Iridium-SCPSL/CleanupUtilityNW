using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;

namespace CleanupUtilityNW
{
    /// <summary>
    /// 
    /// </summary>
    public class CleanupUtilityNW
    {
 
        [PluginConfig]
        public Config Config;
        
        /// <summary>
        /// Gets a static instance of the <see cref="CleanupUtilityNW"/> class.
        /// </summary>
        public static CleanupUtilityNW Instance { get; private set; }

        public Dictionary<Player, bool> playerPocketChecker { get; set; }

        private List<BasicRagdoll> ragDollDataInternal
        {
            get;
            set;
        }
        
        public List<BasicRagdoll> allRagdolls
        {
            // get
            // {
            //     // if (ragDollDataInternal.IsEmpty())
            //     // {
            //     //     ragDollDataInternal = (from r in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>()
            //     //         orderby r.Info.CreationTime descending
            //     //         select r).ToList<BasicRagdoll>();
            //     // }
            //     //
            //     // return ragDollDataInternal;
            //     return (from r in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>()
            //     orderby r.Info.CreationTime descending
            //     select r).ToList();
            // }
            get;
            set;
        }


        [PluginEntryPoint("CleanupUtilityNW", "1.0.0", "CLean up items by zone, item, and time", "Undid Iridium")]
        void LoadPlugin()
        {
            Instance = this;
            playerPocketChecker = new();
            harmony = new Harmony($"com.Undid-Iridium.CleanupUtilityNW.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();

            PickupChecker = new PickupChecker(this);
            PluginAPI.Events.EventManager.RegisterEvents(this);
            Log.Debug("We have started our plugin CleanupUtilityNW!!", Instance.Config.Debug);
        }

        [PluginEvent(ServerEventType.RoundStart)]
        void OnRoundStarted()
        {
            PickupChecker.OnRoundStarted();
        }
        
        [PluginEvent(ServerEventType.RoundStart)]
        void OnRestartingRound()
        {
            PickupChecker.OnRestartingRound();
        }

        [PluginEvent(ServerEventType.PlayerEnterPocketDimension)]
        void OnPocketEnter(Player curPlayer)
        {
            PickupChecker.OnPocketEnter(curPlayer);
        }
        
        [PluginEvent(ServerEventType.PlayerExitPocketDimension)]
        void OnPocketExit(IPlayer curPlayer, Boolean allowed)
        {
            PickupChecker.OnPocketExit(Player.Get(curPlayer.ReferenceHub));
        }

        [PluginEvent(ServerEventType.RagdollSpawn)]
        void onRagDollSpawn(IPlayer player, IRagdollRole ragdoll, DamageHandlerBase baseHandler)
        {
            //TODO NW is not calling this AFAIK, broken for now. I don't want to search EVERY item in existence
            //to determine if it is a ragdoll (which is the command available, and how it does it)
            allRagdolls.Add(ragdoll.Ragdoll); 
        }
        
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void OnRoleChange(IPlayer curPlayer, PlayerRoleBase roleBase, RoleTypeId typeId, RoleChangeReason reason)
        {
            PickupChecker.OnRoleChange(Player.Get(curPlayer.ReferenceHub));
        }

        public Harmony harmony { get; set; }

        /// <summary>
        /// Gets an instance of the <see cref="PickupChecker"/> class.
        /// </summary>
        public PickupChecker PickupChecker { get; private set; }

    }
}