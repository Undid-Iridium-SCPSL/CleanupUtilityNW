using System;
using System.Collections.Generic;
using MapGeneration;
using PluginAPI.Core;
using UnityEngine;

namespace CleanupUtilityNW
{
    public static class ExtenstionMethodDict
    {
        public static void AddOrUpdateDictionaryEntry(this Dictionary<Player, Boolean> dict, Player key, bool value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
        
        private static readonly RaycastHit[] RayCastParent = new RaycastHit[1];
        
        public static FacilityZone findZone(this GameObject currentObject){
            Ray downRay = new(currentObject.transform.position, Vector3.down);

            if (Physics.RaycastNonAlloc(downRay, RayCastParent, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
                return RayCastParent[0].collider.gameObject.GetComponentInParent<RoomIdentifier>().Zone;

            Ray upRay = new(currentObject.transform.position, Vector3.up);

            if (Physics.RaycastNonAlloc(upRay, RayCastParent, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
                return RayCastParent[0].collider.gameObject.GetComponentInParent<RoomIdentifier>().Zone;

            return FacilityZone.None;
        }

    }
}