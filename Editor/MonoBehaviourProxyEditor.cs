using UnityEditor;
using UnityEngine;
namespace Kurisu.MonoReference.Editor
{
    [CustomEditor(typeof(MonoBehaviourProxy))]
    public class MonoBehaviourProxyEditor : UnityEditor.Editor
    {
        private SerializedProperty reference;
        private SerializedProperty asm;
        private MonoBehaviourProxy Proxy => target as MonoBehaviourProxy;
        private SerializedObject referenceObject;
        private string referenceASM;
        private void OnEnable()
        {
            reference = serializedObject.FindProperty("monoBehaviorReference");
            asm = reference.FindPropertyRelative("assemblyQualifiedName");
            RefreshReference();
        }
        private void OnDisable()
        {
            referenceObject?.Dispose();
            if (Proxy.ReferencedMonoBehaviour != null && EditorUtility.IsDirty(Proxy.ReferencedMonoBehaviour))
            {
                //Serialize reference monoBehaviour
                Proxy.Serialize();
                EditorUtility.SetDirty(target);
            }
        }
        public override void OnInspectorGUI()
        {
            if (referenceASM != Proxy.GetReference())
            {
                RefreshReference();
            }
            GUILayout.Label("MonoBehaviour Proxy", new GUIStyle(GUI.skin.label) { fontSize = 15, alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.PropertyField(reference);
            if (referenceObject == null)
            {
                EditorGUILayout.HelpBox("Reference is null, which is not expected", MessageType.Info);
                return;
            }
            DrawReferenceObjectInspector();
        }
        private void DrawReferenceObjectInspector()
        {
            EditorGUI.BeginChangeCheck();
            referenceObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = referenceObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    //Skip m_Script field
                    if (!enterChildren) EditorGUILayout.PropertyField(iterator, true);
                }
                enterChildren = false;
            }
            referenceObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
        private void RefreshReference()
        {
            referenceObject?.Dispose();
            referenceObject = null;
            referenceASM = Proxy.GetReference();
            if (!IsValidBehavior())
            {
                // Deserialize fail when type can not be found
                if (!Proxy.Deserialize()) return;
                Proxy.ReferencedMonoBehaviour.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;
            }
            referenceObject = new SerializedObject(Proxy.ReferencedMonoBehaviour);
        }
        private bool IsValidBehavior()
        {
            return Proxy.ReferencedMonoBehaviour != null && Proxy.ReferencedMonoBehaviour.GetType().AssemblyQualifiedName == asm.stringValue;
        }
    }
}