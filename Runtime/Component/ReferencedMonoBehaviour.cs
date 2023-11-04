using UnityEngine;
namespace Kurisu.MonoReference
{
    /// <summary>
    /// Custom MonoBehaviour to be referenced
    /// </summary>
    public abstract class ReferencedMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Called when this behaviour is deserialized after attached by proxy
        /// </summary>
        public virtual void OnDeserialize() { }
    }
}
