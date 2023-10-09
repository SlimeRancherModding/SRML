using System.Collections.Generic;
using SRML.SR.Templates.Components;
using SRML.SR.Utils.BaseObjects;
using SRML.SR.Utils.Debug;
using UnityEngine;

namespace SRML.SR.Templates
{
    /// <summary>
    /// Represents a template game object, used to contain templates to turn into
    /// Runtime Prefabs
    /// </summary>
    public class GameObjectTemplate
    {
        private readonly List<ICreateComponent> components = new List<ICreateComponent>();

        public readonly List<GameObjectTemplate> children = new List<GameObjectTemplate>();

        public string Name { get; set; } = "Template Object";
        public string Tag { get; set; } = null;
        public LayerMask Layer { get; set; } = LayerMask.NameToLayer("Default");

        public Vector3 Position { get; set; } = Vector3.zero;
        public Vector3 Rotation { get; set; } = Vector3.zero;
        public Vector3 Scale { get; set; } = Vector3.one;

        public MarkerType DebugMarker { get; private set; } = MarkerType.None;

        private event System.Action<GameObject> AfterChildren;
        private readonly List<string> actionOnStart = new List<string>();
        private readonly List<string> actionOnAwake = new List<string>();

        public GameObjectTemplate(string name, params ICreateComponent[] comps)
        {
            Name = name;
            components.AddRange(comps);
        }

        public GameObjectTemplate Clone(string name = null, bool cloneChildren = false)
        {
            GameObjectTemplate clone = new GameObjectTemplate(name ?? Name, components.ToArray());

            clone.SetTransform(Position, Rotation, Scale);
            clone.SetDebugMarker(DebugMarker);
            clone.Tag = Tag;
            clone.Layer = Layer;

            foreach (GameObjectTemplate child in children)
            {
                clone.AddChild(cloneChildren ? child.Clone() : child);
            }

            return clone;
        }

        public GameObjectTemplate SetTransform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            return this;
        }

        public GameObjectTemplate SetTransform(ObjectTransformValues trans)
        {
            Position = trans.position;
            Rotation = trans.rotation;
            Scale = trans.scale;
            return this;
        }

        public GameObjectTemplate SetDebugMarker(MarkerType type, float scaleMult = 1)
        {
            DebugMarker = type;

            if (BaseObjects.markerMaterials.ContainsKey(type))
            {
                AddChild(new GameObjectTemplate("DebugMarker",
                    new Create<MeshFilter>((filter) => filter.sharedMesh = BaseObjects.cubeMesh),
                    new Create<MeshRenderer>((render) => render.sharedMaterial = BaseObjects.markerMaterials[type]),
                    new Create<DebugMarker>((marker) =>
                    {
                        marker.scaleMult = scaleMult;
                        marker.type = type;
                    })
                ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one)); ;
            }

            return this;
        }

        public GameObjectTemplate AddAfterChildren(System.Action<GameObject> action)
        {
            AfterChildren += action;
            return this;
        }

        public GameObjectTemplate AddStartAction(string actionID)
        {
            actionOnStart.Add(actionID);
            return this;
        }

        public GameObjectTemplate AddAwakeAction(string actionID)
        {
            actionOnAwake.Add(actionID);
            return this;
        }

        public GameObjectTemplate AddChild(GameObjectTemplate template)
        {
            children.Add(template);
            return this;
        }

        public GameObjectTemplate AddComponents(params ICreateComponent[] comps)
        {
            components.AddRange(comps);
            components.RemoveAll(c => c == null);
            return this;
        }

        public GameObjectTemplate RemoveComponents(params ICreateComponent[] comps)
        {
            foreach (ICreateComponent comp in comps)
                components.Remove(comp);

            return this;
        }

        public GameObjectTemplate FindChild(string name, bool parcial = false)
        {
            foreach (GameObjectTemplate child in children)
            {
                if (child.Name.StartsWith(name) && parcial || child.Name.Equals(name) && !parcial)
                    return child;
            }

            return null;
        }

        public GameObject ToGameObject(GameObject parent)
        {
            GameObject obj;
            if (parent == null)
            {
                obj = new GameObject(SRML.Utils.ReflectionUtils.GetRelevantAssembly().GetName().Name.ToLower() + "." + Name);
                SRML.Utils.GameObjectUtils.Prefabitize(obj);
            }
            else
            {
                obj = new GameObject(Name);
                obj.transform.parent = parent.transform;
            }

            obj.transform.localPosition = Position;
            obj.transform.localEulerAngles = Rotation;
            obj.transform.localScale = Scale;

            if (actionOnAwake.Count > 0)
            {
                ActionOnAwake comp = obj.AddComponent<ActionOnAwake>();
                comp.actions = actionOnAwake;
            }

            if (actionOnStart.Count > 0)
            {
                ActionOnStart comp = obj.AddComponent<ActionOnStart>();
                comp.actions = actionOnStart;
            }

            foreach (ICreateComponent comp in components)
            {
                if (comp == null)
                    continue;

                comp.AddComponent(obj);
            }

            if (Tag != null) obj.tag = Tag;
            if (Layer != LayerMask.NameToLayer("Default")) obj.layer = Layer;

            foreach (GameObjectTemplate child in children)
                child.ToGameObject(obj);

            AfterChildren?.Invoke(obj);

            return obj;
        }
    }
}
