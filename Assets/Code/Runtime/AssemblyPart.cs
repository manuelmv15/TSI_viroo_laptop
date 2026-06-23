using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace CustomCode
{
    public class AssemblyPart : MonoBehaviour
    {
        public string id;
        public string partType;

        public Transform startTransform = null;

        public XRSocketInteractor SocketInteractor { get; set; } = null;

        public bool IsSocketed => SocketInteractor != null;
    }
}
