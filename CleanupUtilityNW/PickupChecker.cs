using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles.Ragdolls;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using UnityEngine;
using FacilityZone = PluginAPI.Core.Zones.FacilityZone;

namespace CleanupUtilityNW
{
    /// <summary>
    /// Handles the cleaning of items.
    /// </summary>
    public class PickupChecker
    {
        private readonly CleanupUtilityNW plugin;
        private Dictionary<ItemPickupBase, float> itemTracker = new();
        private CoroutineHandle cleanupItemsCoroutine;
        private CoroutineHandle cleanupRagDollsCoroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupChecker"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public PickupChecker(CleanupUtilityNW plugin)
        {
            this.plugin = plugin;
        }

        /// <summary>
        /// Adds an item to the tracking queue.
        /// </summary>
        /// <param name="pickup">The item to add.</param>
        /// <param name="currentZone"> Current player zone for player hub. </param>
        /// <param name="curPlayer"> Current player. </param>
        public void Add(ItemPickupBase itemPickupBase, ReferenceHub curHub)
        {
            try
            {
                PickupSyncInfo pickup = itemPickupBase.NetworkInfo;
                Player curPlayer = Player.Get(curHub);
                MapGeneration.FacilityZone currentZone = MapGeneration.FacilityZone.None;
                if(RoomIdentifier.RoomsByCoordinates.TryGetValue(RoomIdUtils.PositionToCoords(pickup.Position), out RoomIdentifier roomIdentifier))
                {
                    currentZone = roomIdentifier.Zone;
                }
                
                bool foundItem = plugin.Config.ItemFilter.TryGetValue(pickup.ItemId, out float time);
                bool isPocket = plugin.playerPocketChecker.TryGetValue(curPlayer, out bool inPocket);
                if (foundItem && time >= 0)
                {
                    if (isPocket && inPocket)
                    {
                        if (plugin.Config.CleanInPocket)
                        {
                            Log.Debug(
                                $"Added a {pickup.ItemId} ({pickup.Serial}) to the tracker to be deleted in {time} seconds from pocket dimension.",
                                plugin.Config.Debug);
                            itemTracker.Add(itemPickupBase, Time.time + time);
                        }

                        return;
                    }

                    if (
                        plugin.Config.ZoneFilter.TryGetValue(pickup.ItemId,
                            out HashSet<MapGeneration.FacilityZone> acceptedZones))
                    {
                        if (acceptedZones.Contains(currentZone))
                        {
                            itemTracker.Add(itemPickupBase, Time.time + time);

                            // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                            if (plugin.Config.Debug)
                            {
                                Log.Debug(
                                    $"Added a {pickup.ItemId} ({pickup.Serial}) to the tracker to be deleted in {time} seconds.", plugin.Config.Debug);
                            }
                        }
                        else if (acceptedZones.Contains(MapGeneration.FacilityZone.None))
                        {
                            itemTracker.Add(itemPickupBase, Time.time + time);

                            // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                            if (plugin.Config.Debug)
                            {
                                Log.Debug(
                                    $"Added a {pickup.ItemId} ({pickup.Serial}) to the tracker to be deleted in {time} seconds with Unspecified marked as acceptable.", plugin.Config.Debug);
                            }
                        }
                        else if (plugin.Config.Debug)
                        {
                            // Added this if user wants to see why item was not added. Again, condition of config file much
                            Log.Debug(
                                $"Could not add item {pickup.ItemId} because zones were not equal current {currentZone} vs accepted {string.Join(Environment.NewLine, acceptedZones)}", plugin.Config.Debug);
                        }

                        return;
                    }

                    // We are going to assume that the user forgot to specify the zone. Therefore, the zone is unspecified.
                    if (plugin.Config.Debug)
                    {
                        Log.Debug(
                            $"Added a {pickup.ItemId} ({pickup.Serial}) to the tracker to be deleted in {time} seconds, defaulting to unspecified zone.", plugin.Config.Debug);
                    }

                    itemTracker.Add(itemPickupBase, Time.time + time);
                }
            }
            catch (Exception ex)
            {
                Log.Debug($"Pickup.add failed because of {ex}", plugin.Config.Debug);
            }
        }

        /// <inheritdoc cref="Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
         
        }

        /// <inheritdoc cref="Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            
            itemTracker.Clear();
            
            if (cleanupItemsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(cleanupItemsCoroutine);
            }

            if (cleanupRagDollsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(cleanupRagDollsCoroutine);
            }

            if (plugin.Config.CleanupItems)
            {
                cleanupItemsCoroutine = Timing.RunCoroutine(CheckItems());
            }

            if (plugin.Config.CleanupRagDolls)
            {
                cleanupRagDollsCoroutine = Timing.RunCoroutine(CheckRagDolls());
            }
            
            Log.Info("Ran through OnRestartingRound");
        }

        /// <summary>
        /// When a player changes role, we will ensure InPocket flag is removed.
        /// </summary>
        /// <param name="ev"> Event for changing role. </param>
        internal void OnRoleChange(Player ev)
        {
            Timing.CallDelayed(plugin.Config.CheckInterval + 5, () =>
            {
                plugin.playerPocketChecker.Remove(ev);
            });
        }

        /// <summary>
        /// When a player dies, we will ensure InPocket flag is removed.
        /// </summary>
        /// <param name="ev"> Event for dying. </param>
        internal void OnDied(Player ev)
        {
            Timing.CallDelayed(plugin.Config.CheckInterval + 5, () =>
            {
                plugin.playerPocketChecker.Remove(ev);
            });
        }

        /// <summary>
        /// When a player enteres the pocket dimension, we will mark them as left.
        /// </summary>
        /// <param name="curPlayer"> Current pocket dimension event info. </param>
        internal void OnPocketExit(Player curPlayer)
        {
            plugin.playerPocketChecker.AddOrUpdateDictionaryEntry(curPlayer, false);
        }


        /// <summary>
        /// When a player enteres the pocket dimension, we will mark them as entered.
        /// </summary>
        /// <param name="curPlayer"> Current pocket dimension event info. </param>
        internal void OnPocketEnter(Player curPlayer)
        {
            plugin.playerPocketChecker.AddOrUpdateDictionaryEntry(curPlayer, true);
        }

        /// <summary>
        /// Coroutine to iteratively check all currently tracked items to be removed within a zone/time limit.
        /// </summary>
        /// <returns> <see cref="IEnumerable{T}"/> which is used to determine how long this generator function should wait. </returns>
        private IEnumerator<float> CheckItems()
        {
            Log.Info($"Check item entered");
            yield return Timing.WaitForSeconds(5);
            while (Round.IsRoundStarted)
            {
                yield return Timing.WaitForSeconds(plugin.Config.CheckInterval);
                if (itemTracker.IsEmpty())
                {
                    continue;
                }

                for (int pos = 0; pos < itemTracker.Count; pos++)
                {
                    
                    KeyValuePair<ItemPickupBase, float> item = itemTracker.ElementAt(pos);
                    PickupSyncInfo pickup = item.Key.NetworkInfo;
                    Log.Debug($"ItemTracker was not empty, what is current item pair {pickup.ItemId}, and base/float" +
                              $" {item.Key} / {item.Value} ", plugin.Config.Debug);
                    CheckItem(pickup, item, pos);
                }
            }
        }

        /// <summary>
        /// Checks the current pickup with the experitation time.
        /// </summary>
        /// <param name="pickup"> <see cref="Pickup"/> to verify whether to delete. </param>
        /// <param name="tupledData"></param>
        /// <param name="pos"></param>
        /// <param name="expirationTime"> Time limit for pickup. </param>
        private void CheckItem(PickupSyncInfo pickup, KeyValuePair<ItemPickupBase, float> tupledData, int pos)
        {

            if (pickup.InUse || tupledData.Key == null)
            {
                itemTracker.Remove(tupledData.Key);
                return;
            }

            if (Time.time < tupledData.Value)
            {
                Log.Debug($"ItemTracker in use or time has not passed {pickup.ItemId}, and base/float" +
                          $" {tupledData.Key} / {tupledData.Value} / (time) {Time.time} ", plugin.Config.Debug);
                return;
            }

            Log.Debug($"Deleting an item of type {pickup.ItemId} ({pickup.Serial}).", plugin.Config.Debug);
            try
            {
                itemTracker.Remove(tupledData.Key);
                tupledData.Key.DestroySelf();
            }
            catch (Exception unableToDelete)
            {
                Log.Info($"Unable to delete item {pickup.ItemId} because of {unableToDelete}");
            }
        }

        /// <summary>
        /// Coroutine to iteratively check all currently ragdolls to be removed within a zone/time limit.
        /// </summary>
        /// <returns> <see cref="IEnumerable{T}"/> which is used to determine how long this generator function should wait. </returns>
        private IEnumerator<float> CheckRagDolls()
        {
            yield return Timing.WaitForSeconds(5);
            while (Round.IsRoundStarted)
            {
                yield return Timing.WaitForSeconds(plugin.Config.CheckRagDollInterval);
                for (int pos = 0; pos < plugin.allRagdolls.Count; pos++)
                {
                    BasicRagdoll curRagdoll = plugin.allRagdolls.ElementAt(pos);
                    CheckRagDoll(curRagdoll);
                }
            }
        }

        /// <summary>
        /// Verifies the ragdoll can be cleanup, and is not null.
        /// </summary>
        /// <param name="curRagdoll"> <see cref="Exiled.API.Features.Ragdoll"/> to potentially clean up. </param>
        private void CheckRagDoll(BasicRagdoll curRagdoll)
        {
            if (curRagdoll is null)
            {
                return;
            }

            if (curRagdoll.Info.ExistenceTime < plugin.Config.RagdollExistenceLimit)
            {
                return;
            }

            //
            Player curPlayer = Player.Get<Player>(curRagdoll.Info.OwnerHub);
            if (plugin.playerPocketChecker.TryGetValue(curPlayer, out bool inPocket))
            {
                if (!inPocket) return;
                Log.Debug($"Deleting a Ragdoll {curRagdoll} in pocket dimension", plugin.Config.Debug);
                NetworkServer.Destroy(curRagdoll.gameObject);
                return;
            }

            Log.Debug($"Ragdoll found in position {curPlayer.Position}", plugin.Config.Debug);
                

            if(RoomIdentifier.RoomsByCoordinates.TryGetValue(RoomIdUtils.PositionToCoords(curRagdoll.CenterPoint.position), out RoomIdentifier roomIdentifier))
            {
                if (!plugin.Config.RagdollAcceptableZones.Contains(roomIdentifier.Zone))
                {
                    Log.Debug($"Ragdoll found in position {curPlayer.Position} had zone {roomIdentifier.Zone} but was set to be cleaned up", plugin.Config.Debug);
                    return;
                }
            }
            else
            {
                Log.Debug($"Ragdoll found in position {curPlayer.Position} could not find a room associated (NWAPI)", plugin.Config.Debug);
                return;
            }
                
          

            Log.Debug($"Deleting a Radoll {curRagdoll} in zone {curRagdoll}", plugin.Config.Debug);
            NetworkServer.Destroy(curRagdoll.gameObject);
        }
    }
}