using System.Collections.Generic;
using UnityEngine;

namespace CustomCode
{
    public class GrabbableConfiguration: MonoBehaviour
    {
        public List<GrabbableConfigurationData> grabbablesConfigurations;
    }

    [System.Serializable]
    public class GrabbableConfigurationData
    {
        public GameObject targetGameObject;

        public Transform startTransform;

        public string helpMessage;

        public AudioClip helpAudioClip;

        public string titleMessage;
    }
}
