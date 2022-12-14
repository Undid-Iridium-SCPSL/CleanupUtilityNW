// -----------------------------------------------------------------------
// <copyright file="ServerCreatePickupPatch.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CleanupUtilityNW;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using UnityEngine;

namespace CleanupUtility.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup"/> to add <see cref="Pickup"/>s to the <see cref="PickupChecker"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class ServerCreatePickupPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            //PickupSyncInfo pickup, ItemPickupBase itemPikcupBase, Player curPlayer
            LocalBuilder PickupSyncInfo = generator.DeclareLocal(typeof(PickupSyncInfo));

            LocalBuilder ItemPickupBase = generator.DeclareLocal(typeof(ItemPickupBase));

            LocalBuilder curPlayer = generator.DeclareLocal(typeof(ReferenceHub));
            
            int offset = 1;
            int index = newInstructions.FindIndex(instruction =>
                instruction.Calls(Method(typeof(ReferenceHub), nameof(ReferenceHub.GetHub), new[]{ typeof(GameObject)}))) + offset;


            //Store reference hub, 
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, curPlayer.LocalIndex)
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret);
            newInstructions.InsertRange(index, new[]
            {
                // new CodeInstruction(OpCodes.Ldstr, "1234"),
                // new CodeInstruction(OpCodes.Ldstr, ""),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string), typeof(string)})),
                // Calls static instance
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(CleanupUtilityNW.CleanupUtilityNW), nameof(CleanupUtilityNW.CleanupUtilityNW.Instance))),
                
                // Since the instance is now on the stack, we will call ProperttyGetter to get to PickupChecker object from our Instance object
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(CleanupUtilityNW.CleanupUtilityNW), nameof(CleanupUtilityNW.CleanupUtilityNW.PickupChecker))),

                /*
                 *      .maxstack 4
                        .locals init (
                            [0] class InventorySystem.Items.Pickups.ItemPickupBase,
                            [1] valuetype InventorySystem.Items.Pickups.PickupSyncInfo
                        )
                */
            
                // Load the variable unto Eval Stack [ItemPickupBase]
                new CodeInstruction(OpCodes.Ldloc_0),
            
                // Calls argument 1 from function call unto EStack (Zone)
                new CodeInstruction(OpCodes.Ldloc, curPlayer.LocalIndex),
            
                // EStack variable used, [PickupChecker (Callvirt arg 0 (Instance)), Pickup (Arg 1 (Param))]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupChecker), nameof(PickupChecker.Add), new[] { typeof(ItemPickupBase), typeof(ReferenceHub) })),
            });

            foreach (CodeInstruction instr in newInstructions)
            {
                yield return instr;
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}