using UnityEngine;

namespace CustomCode
{
    public class FeedBack : MonoBehaviour
    {
        public GameObject feedbackGameObject;

        public void Show()
        {
            feedbackGameObject.SetActive(true);
        }

        public void Hide()
        {
            feedbackGameObject.SetActive(false);
        }

        public void SetFeedbackMaterial(Material material)
        {
            feedbackGameObject.GetComponent<InstancedRenderersHierarchy>().material = material;
        }
    }
}

