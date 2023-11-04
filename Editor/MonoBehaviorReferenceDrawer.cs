using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.MonoReference.Editor
{
    [CustomPropertyDrawer(typeof(MonoBehaviorReference))]
    public class MonoBehaviorReferenceDrawer : PropertyDrawer
    {
        private const string NullType = "Null";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var reference = property.FindPropertyRelative("assemblyQualifiedName");
            var type = Type.GetType(reference.stringValue);
            string id = type != null ? $"{type.Assembly.GetName().Name} {type.Namespace} {type.Name}" : NullType;
            if (EditorGUI.DropdownButton(position, new GUIContent(id), FocusType.Keyboard))
            {
                var provider = ScriptableObject.CreateInstance<ReferencedMonoBehaviourSearchWindow>();
                provider.Initialize((proxyType) =>
                {
                    reference.stringValue = proxyType?.AssemblyQualifiedName ?? NullType;
                    property.serializedObject.ApplyModifiedProperties();
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
            EditorGUI.EndProperty();
        }
    }
    public class ReferencedMonoBehaviourSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private Texture2D _indentationIcon;
        private Action<Type> typeSelectCallBack;
        public void Initialize(Action<Type> typeSelectCallBack)
        {
            this.typeSelectCallBack = typeSelectCallBack;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Select ReferencedMonoBehavior"), 0),
                new(new GUIContent("<Null>", _indentationIcon)) { level = 1, userData = null }
            };
            List<Type> nodeTypes = FindSubClasses(typeof(ReferencedMonoBehaviour));
            var groups = nodeTypes.GroupBy(t => t.Assembly);
            foreach (var group in groups)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group.Key.GetName().Name}"), 1));
                var subGroups = group.GroupBy(x => x.Namespace);
                foreach (var subGroup in subGroups)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {subGroup.Key}"), 2));
                    foreach (var type in subGroup)
                    {
                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name, _indentationIcon)) { level = 3, userData = type });
                    }
                }
            }
            return entries;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            typeSelectCallBack?.Invoke(type);
            return true;
        }
        private static List<Type> FindSubClasses(Type father)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(father) && !t.IsAbstract).ToList();
        }
    }
}
