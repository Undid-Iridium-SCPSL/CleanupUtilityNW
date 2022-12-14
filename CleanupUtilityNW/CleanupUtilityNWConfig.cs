using System;
using System.Collections.Generic;
using System.ComponentModel;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Zones;
using FacilityZone = MapGeneration.FacilityZone;

namespace CleanupUtilityNW
{
    public class Config
    {

        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug logs should be shown.
        /// </summary>
        [Description("Whether debug logs should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether items should be cleaned up.
        /// </summary>
        [Description("Gets or sets a value indicating whether items should be cleaned up.")]
        public bool CleanupItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether ragdolls should be cleaned up.
        /// </summary>
        [Description("Gets or sets a value indicating whether ragdolls should be cleaned up.")]
        public bool CleanupRagDolls { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets value indicating whether to clean in pocket dimension.
        /// </summary>
        [Description("Gets or sets a value indicating whether to clean in pocket dimension.")]
        public bool CleanInPocket { get; set; } = false;

        /// <summary>
        /// Gets or sets the time, in seconds, between each check of the list of items to delete.
        /// </summary>
        [Description("The time, in seconds, between each check of the list of items to delete.")]
        public float CheckInterval { get; set; } = 2f;

        /// <summary>
        /// Gets or sets the time, in seconds, between each check of the list of ragdolls to delete.
        /// </summary>
        [Description("The time, in seconds, between each check of the list of ragdolls to delete.")]
        public float CheckRagDollInterval { get; set; } = 2f;

        /// <summary>
        /// Gets or sets a collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.
        /// </summary>
        [Description("A collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.")]
        public Dictionary<ItemType, float> ItemFilter { get; set; } = new()
        {
            { ItemType.KeycardJanitor, 600f },
            { ItemType.KeycardScientist, 600f },
            { ItemType.KeycardResearchCoordinator, 600f },
            { ItemType.KeycardZoneManager, 600f },
            { ItemType.KeycardGuard, 600f },
            { ItemType.KeycardNTFOfficer, 600f },
            { ItemType.KeycardContainmentEngineer, 600f },
            { ItemType.KeycardNTFLieutenant, 600f },
            { ItemType.KeycardNTFCommander, 600f },
            { ItemType.KeycardFacilityManager, 600f },
            { ItemType.KeycardChaosInsurgency, 600f },
            { ItemType.KeycardO5, 600f },
            { ItemType.Radio, 600f },
            { ItemType.GunCOM15, 600f },
            { ItemType.Medkit, 600f },
            { ItemType.Flashlight, 600f },
            { ItemType.MicroHID, 600f },
            { ItemType.SCP500, 600f },
            { ItemType.SCP207, 600f },
            { ItemType.Ammo12gauge, 600f },
            { ItemType.GunE11SR, 600f },
            { ItemType.GunCrossvec, 600f },
            { ItemType.Ammo556x45, 600f },
            { ItemType.GunFSP9, 600f },
            { ItemType.GunLogicer, 600f },
            { ItemType.GrenadeHE, 600f },
            { ItemType.GrenadeFlash, 600f },
            { ItemType.Ammo44cal, 600f },
            { ItemType.Ammo762x39, 600f },
            { ItemType.Ammo9x19, 600f },
            { ItemType.GunCOM18, 600f },
            { ItemType.SCP018, 600f },
            { ItemType.SCP268, 600f },
            { ItemType.Adrenaline, 600f },
            { ItemType.Painkillers, 600f },
            { ItemType.Coin, 600f },
            { ItemType.ArmorLight, 600f },
            { ItemType.ArmorCombat, 600f },
            { ItemType.ArmorHeavy, 600f },
            { ItemType.GunRevolver, 600f },
            { ItemType.GunAK, 600f },
            { ItemType.GunShotgun, 600f },
            { ItemType.SCP330, 600f },
            { ItemType.SCP2176, 600f },
            { ItemType.SCP244a, 600f },
            { ItemType.SCP244b, 600f },
            { ItemType.SCP1853, 600f },
            { ItemType.ParticleDisruptor, 600f },
            { ItemType.GunCom45, 600f },
        };
        
        /// <summary>
        /// Gets or sets a collection of ItemTypes that should be deleted by Zone.
        /// </summary>
        [Description("Filter on what zone item type can be cleared from.")]
        public Dictionary<ItemType, HashSet<FacilityZone>> ZoneFilter { get; set; } = new()
        {
            { ItemType.KeycardJanitor, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardScientist, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardResearchCoordinator, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardZoneManager, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardGuard, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardNTFOfficer, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardContainmentEngineer, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardNTFLieutenant, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardNTFCommander, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardFacilityManager, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardChaosInsurgency, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.KeycardO5, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Radio, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunCOM15, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Medkit, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Flashlight, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.MicroHID, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP500, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP207, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Ammo12gauge, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunE11SR, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunCrossvec, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Ammo556x45, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunFSP9, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunLogicer, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GrenadeHE, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GrenadeFlash, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Ammo44cal, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Ammo762x39, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Ammo9x19, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunCOM18, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP018, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP268, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Adrenaline, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Painkillers, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.Coin, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.ArmorLight, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.ArmorCombat, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.ArmorHeavy, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunRevolver, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunAK, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunShotgun, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP330, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP2176, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP244a, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP244b, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.SCP1853, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.ParticleDisruptor, new HashSet<FacilityZone>() { FacilityZone.None } },
            { ItemType.GunCom45, new HashSet<FacilityZone>() { FacilityZone.None } },
        };

        /// <summary>
        /// Gets or sets existence time limit for ragdolls.
        /// </summary>
        [Description("Time limit for ragdoll existence.")]
        public float RagdollExistenceLimit { get; set; } = 10;

        /// <summary>
        /// Gets or sets a acceptable cleanup Zones.
        /// </summary>
        [Description("Filter on what zone ragdolls can be cleared from.")]
        public HashSet<FacilityZone> RagdollAcceptableZones { get; set; } = new HashSet<FacilityZone>()
        {
            FacilityZone.None,
            FacilityZone.Other,
            FacilityZone.Entrance,
            FacilityZone.Surface,
            FacilityZone.HeavyContainment,
            FacilityZone.LightContainment,
        };
    }
}