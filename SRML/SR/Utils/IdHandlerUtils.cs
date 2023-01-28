using SRML.SR;
using SRML.SR.SaveSystem;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.Utils
{
    public static class IdHandlerUtils
    {
        static IdDirector globalIdDirector;

        public static IdDirector GlobalIdDirector
        {
            get
            {
                if (!globalIdDirector)
                {
                    var g = new GameObject();
                    globalIdDirector = g.AddComponent<IdDirector>();
                }
                return globalIdDirector;
            }
        }

        internal static Dictionary<Type, GameObject> idHandlerPrefabs = new Dictionary<Type, GameObject>();
        static IdHandlerUtils()
        {
            SRCallbacks.PreSaveGameLoaded += (s) =>
            {
                foreach (IdHandler handler in UnityEngine.Object.FindObjectsOfType<IdHandler>())
                {
                    if (!idHandlerPrefabs.ContainsKey(handler.GetType()))
                        idHandlerPrefabs[handler.GetType()] = PrefabUtils.CopyPrefab(handler.gameObject);
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

            newG.GetComponent<T>().director = GlobalIdDirector;
            GlobalIdDirector.persistenceDict[newG.GetComponent<T>()] = ModdedStringRegistry.IsModdedString(id) ? id : ModdedStringRegistry.ClaimID(GetPrefix<T>(), id);

            return newG;
        }

        /// <summary>
        /// Create a new instance of an IdHandler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
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
