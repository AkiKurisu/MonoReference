using UnityEngine;
using System;
namespace Kurisu.MonoReference
{
    [Serializable]
    public class MonoBehaviorReference : IEquatable<MonoBehaviorReference>
    {
        [SerializeField]
        private string assemblyQualifiedName;
        public string AssemblyQualifiedName => assemblyQualifiedName;
        public bool Equals(MonoBehaviorReference other)
        {
            return other.AssemblyQualifiedName == AssemblyQualifiedName;
        }

        public static implicit operator string(MonoBehaviorReference reference)
        {
            return reference.AssemblyQualifiedName;
        }
    }
}
