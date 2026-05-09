using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using Modding;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace HKTags
{
    public class HKTags : Mod
    {
        internal static HKTags Instance;

        public override string GetVersion() => "26.5.0.0";

        public string UnloadEverything(string SceneName)
        {
            Log("[INFO] :: Loading new scene, clearing every attribute and tag.");

            Tags.ClearAllRegisters();
            Attributes.ClearAllRegisters();

            return SceneName;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += UnloadEverything;
        }
    }

    public class DestroyListener : MonoBehaviour
    {
        public Action<GameObject> Destroyed;

        public void OnDestroy()
        {
            Destroyed?.Invoke(this.gameObject);
        }
    }

    public static class Tags
    {
        public static HKTags Mod => HKTags.Instance;
        public static Dictionary<GameObject, List<string>> GameObjectTags = new Dictionary<GameObject, List<string>>();

        public static void RegisterGameObject(GameObject GameObj)
        {
            if (!GameObj || GameObjectTags.ContainsKey(GameObj)) {
                Mod.LogWarn("[WARN] :: You cannot register a GameObject that has already been registered or does not exist.");

                return;
            }

            GameObjectTags.Add(GameObj, new List<string>());

            GameObj.AddComponent<DestroyListener>().Destroyed += (GameObject GameObj) => {
                GameObjectTags.Remove(GameObj);
            };
        }

        public static void AddTag(GameObject GameObj, string Tag)
        {
            if (GameObj && GameObjectTags.ContainsKey(GameObj)) 
            {
                GameObjectTags[GameObj].Add(Tag);
            } else
            {
                Mod.LogWarn("[WARN] :: Attempt to set a tag on a GameObject that is not registered or does not exist.");

                return;
            }
        }

        public static bool HasTag(GameObject GameObj, string Tag)
        {
            if (GameObj && GameObjectTags.ContainsKey(GameObj))
            {
                if (GameObjectTags[GameObj].Contains(Tag))
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                Mod.LogWarn("[WARN] :: Attempt to know if a tag exists on a GameObject that is not registered or does not exist.");

                return false;
            }
        }

        public static void RemoveTag(GameObject GameObj, string Tag)
        {
            if (GameObj && GameObjectTags.ContainsKey(GameObj))
            {
                if (!GameObjectTags[GameObj].Contains(Tag)) 
                {
                    Mod.LogWarn("[WARN] :: Attempt to remove a tag that does not exist.");
                }

                GameObjectTags[GameObj].Remove(Tag);
            }
            else
            {
                Mod.LogWarn("[WARN] :: Attempt to set a tag on a GameObject that is not registered or does not exist.");

                return;
            }
        }

        // plural

        public static void RemoveAllTags(GameObject GameObj)
        { 
            if (GameObj)
            {
                GameObjectTags[GameObj].Clear();
            }
        }

        // internal BUT public helpers just in case
        public static void ClearAllRegisters()
        { 
            GameObjectTags.Clear(); 
        }
    }

    public static class Attributes
    {
        public static HKTags Mod => HKTags.Instance;
        public static Dictionary<GameObject, Dictionary<string, object>> GameObjectAttributes = new Dictionary<GameObject, Dictionary<string, object>>();

        public static void RegisterGameObject(GameObject GameObj)
        {
            if (!GameObj || GameObjectAttributes.ContainsKey(GameObj))
            {
                Mod.LogWarn("[WARN] :: You cannot register a GameObject that has already been registered or does not exist.");

                return;
            }

            GameObjectAttributes.Add(GameObj, new Dictionary<string, object>());

            GameObj.AddComponent<DestroyListener>().Destroyed += (GameObject GameObj) => {
                GameObjectAttributes.Remove(GameObj);
            };
        }

        public static void AddAttribute(GameObject GameObj, string Attribute, object Value)
        {
            if (GameObj && GameObjectAttributes.ContainsKey(GameObj))
            {
                GameObjectAttributes[GameObj].Add(Attribute, Value);
            }
            else
            {
                Mod.LogWarn("[WARN] :: Attempt to set a tag on a GameObject that is not registered or does not exist.");

                return;
            }
        }

        public static bool HasAttribute(GameObject GameObj, string Attribute)
        {
            if (GameObj && GameObjectAttributes.ContainsKey(GameObj))
            {
                if (GameObjectAttributes[GameObj].ContainsKey(Attribute))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Mod.LogWarn("[WARN] :: Attempt to know if a tag exists on a GameObject that is not registered or does not exist.");

                return false;
            }
        }

        public static object GetAttribute(GameObject GameObj, string Attribute)
        {
            if (GameObj && GameObjectAttributes.ContainsKey(GameObj))
            {
                if (GameObjectAttributes[GameObj].ContainsKey(Attribute))
                {
                    return GameObjectAttributes[GameObj][Attribute];
                }
                else
                {
                    Mod.LogWarn($"[WARN] :: GameObj does not have any attribute named {Attribute}.");

                    return null;
                }
            }
            else
            {
                Mod.LogWarn("[WARN] :: Attempt to know if a tag exists on a GameObject that is not registered or does not exist.");

                return null;
            }
        }

        // plural

        public static void RemoveAllAttributes(GameObject GameObj)
        {
            if (GameObj)
            {
                GameObjectAttributes[GameObj].Clear();
            }
        }

        // internal BUT public helpers just in case
        public static void ClearAllRegisters()
        { 
            GameObjectAttributes.Clear(); 
        }
    }
}
