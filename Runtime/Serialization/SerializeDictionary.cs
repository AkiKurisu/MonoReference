using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.MonoReference
{
    [Serializable]
    public class SerializeDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys;
        [SerializeField]
        private List<TValue> values;
        private Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() { return target; }
        public SerializeDictionary(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }
        public void OnBeforeSerialize()
        {
            if (target == null)
            {
                keys = new();
                values = new();
                return;
            }
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }
        public void OnAfterDeserialize()
        {
            var count = Mathf.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }
}