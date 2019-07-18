using SRML.SR;
using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Utils
{
    public static class IdHandlerUtils
    {
        internal static Dictionary<Type, GameObject> idHandlerPrefabs = new Dictionary<Type, GameObject>();
        static IdHandlerUtils()
        {
            SRCallbacks.PreSaveGameLoad += (s) =>
            {
                foreach(IdHandler handler in GameObject.FindObjectsOfType<IdHandler>())
                {
                    if(!idHandlerPrefabs.ContainsKey(handler.GetType()))
                    {
                        idHandlerPrefabs[handler.GetType()] = PrefabUtils.CopyPrefab(handler.gameObject);
                    }
                }
            };
        }


        public static string GetPrefix<T>() where T : IdHandler
        {
            if (!idHandlerPrefabs.TryGetValue(typeof(T), out var prefab)) return null;
            return prefab.GetComponent<T>().IdPrefix();
        }

        static GameObject CreateIdInstanceInternal<T>(string id) where T : IdHandler
        {
            if (!idHandlerPrefabs.TryGetValue(typeof(T), out var gameObj)) return null;
            var newG = GameObjectUtils.InstantiateInactive(gameObj);

            newG.GetComponent<T>().id = ModdedStringRegistry.IsModdedString(id) ? id:ModdedStringRegistry.ClaimID(GetPrefix<T>(),id);

            return newG;
        }
        public static GameObject CreateIdInstance<T>(string id) where T : IdHandler
        {
            var newG = CreateIdInstanceInternal<T>(id);
            newG.SetActive(true);
            return newG;
        }

        public static GameObject CreateIdInstance<T>(string id, Transform parent) where T : IdHandler
        {
            var newG = CreateIdInstanceInternal<T>(id);
            newG.transform.SetParent(parent);
            newG.SetActive(true);
            return newG;
        }

        public static GameObject CreateIdInstance<T>(string id, Vector3 position) where T : IdHandler
        {
            var newG = CreateIdInstanceInternal<T>(id);
            newG.transform.position = position;
            newG.SetActive(true);
            return newG;
        }

        public static GameObject CreateIdInstance<T>(string id, Vector3 position, Quaternion rotation) where T : IdHandler
        {
            var newG = CreateIdInstanceInternal<T>(id);
            newG.transform.position = position;
            newG.transform.rotation = rotation;
            newG.SetActive(true);
            return newG;
        }

        public static GameObject CreateIdInstance<T>(string id, Vector3 position, Transform parent) where T : IdHandler
        {
            var newG = CreateIdInstanceInternal<T>(id);
            newG.transform.position = position;
            newG.transform.SetParent(parent, true);
            newG.SetActive(true);
            return newG;
        }
        public static GameObject CreateIdInstance<T>(string id, Vector3 position, Quaternion rotation, Transform parent) where T : IdHandler
        {
            var newG = CreateIdInstanceInternal<T>(id);
            newG.transform.position = position;
            newG.transform.rotation = rotation;
            newG.transform.SetParent(parent, true);
            newG.SetActive(true);
            return newG;
        }



    }
}
