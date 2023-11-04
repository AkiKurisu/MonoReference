using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace Kurisu.MonoReference
{
    [Serializable]
    public class MonoBehaviourSerializeData
    {
        [SerializeField]
        private SerializeDictionary<int, UObject> objectSerializeMap;
        [SerializeField]
        private string serializeJsonData;
        public MonoBehaviourSerializeData(ReferencedMonoBehaviour referencedMonoBehaviour)
        {
            var objectMap = new Dictionary<int, UObject>();
            serializeJsonData = JsonUtility.ToJson(referencedMonoBehaviour);
            JObject obj = JObject.Parse(serializeJsonData);
            foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
            {
                if (prop.Name == "instanceID")
                {
                    var UObject = EditorUtility.InstanceIDToObject((int)prop.Value);
                    if (UObject == null) continue;
                    objectMap.Add((int)prop.Value, UObject);
                }
            }
            objectSerializeMap = new(objectMap);
        }
        public void Deserialize(ReferencedMonoBehaviour referencedMonoBehaviour)
        {
            JObject obj = JObject.Parse(serializeJsonData);
            var objectMap = objectSerializeMap.ToDictionary();
            foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
            {
                if (prop.Name == "instanceID")
                {
                    if (!objectMap.TryGetValue((int)prop.Value, out UObject uObject)) continue;
                    prop.Value = uObject.GetInstanceID();
                }
            }
            JsonUtility.FromJsonOverwrite(obj.ToString(), referencedMonoBehaviour);
        }
    }
}
