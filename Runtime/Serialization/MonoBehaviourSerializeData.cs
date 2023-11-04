using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
#if UNITY_EDITOR
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
#else
        public MonoBehaviourSerializeData(){}
#endif
        public void Deserialize(ReferencedMonoBehaviour referencedMonoBehaviour)
        {
            if (string.IsNullOrEmpty(serializeJsonData)) return;
            JsonUtility.FromJsonOverwrite(serializeJsonData, referencedMonoBehaviour);
            JObject obj = JObject.Parse(serializeJsonData);
            var objectMap = objectSerializeMap.ToDictionary();
            var behaviourType = referencedMonoBehaviour.GetType();
            foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
            {
                if (prop.Name == "instanceID")
                {
                    if (!objectMap.TryGetValue((int)prop.Value, out UObject uObject)) continue;
                    var field = behaviourType.FindField(prop.Path);
                    if (field == null)
                    {
                        Debug.LogWarning($"Can not find field from path : {prop.Path}");
                        continue;
                    }
                    field.SetValue(referencedMonoBehaviour, uObject);
                }
            }
        }

    }
    public static class ReflectionExtension
    {
        public static FieldInfo FindField(this Type sourceType, string path)
        {
            string[] tokens = path.Split('.');
            FieldInfo fieldInfo = null;
            for (int i = 0; i < tokens.Length - 1; ++i)
            {
                fieldInfo = sourceType.FindField(tokens[i], BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    return null;
                }
                sourceType = fieldInfo.FieldType;
            }
            return fieldInfo;
        }
        public static FieldInfo FindField(this Type type, string fieldName, BindingFlags flags)
        {
            FieldInfo field = type.GetField(fieldName, flags);
            if (field == null)
            {
                return type.BaseType?.FindField(fieldName, flags | BindingFlags.DeclaredOnly);
            }
            return field;
        }
    }
}
