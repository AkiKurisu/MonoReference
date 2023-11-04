using System;
using UnityEngine;
namespace Kurisu.MonoReference
{
    /// <summary>
    /// Proxy class to add referenced behaviour when loaded
    /// </summary>
    public class MonoBehaviourProxy : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviorReference monoBehaviorReference;
        [SerializeField, HideInInspector]
        private MonoBehaviourSerializeData serializeData;
        public ReferencedMonoBehaviour ReferencedMonoBehaviour { get; internal set; }
        private void Awake()
        {
            Deserialize();
            ReferencedMonoBehaviour.OnDeserialize();
        }
        public Type GetBehaviourType()
        {
            if (string.IsNullOrEmpty(monoBehaviorReference.AssemblyQualifiedName)) return null;
            return Type.GetType(monoBehaviorReference.AssemblyQualifiedName);
        }
        public MonoBehaviorReference GetReference()
        {
            return monoBehaviorReference;
        }
        public bool Deserialize()
        {
            var type = GetBehaviourType();
            if (type == null)
            {
                ReferencedMonoBehaviour = null;
                return false;
            }
            ReferencedMonoBehaviour = (ReferencedMonoBehaviour)gameObject.AddComponent(GetBehaviourType());
            serializeData.Deserialize(ReferencedMonoBehaviour);
            return true;
        }
        public void Serialize()
        {
            serializeData = new MonoBehaviourSerializeData(ReferencedMonoBehaviour);
        }
    }
}