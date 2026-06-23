using UnityEngine;

namespace CustomCode
{
    public class BillboardFaceCamera : MonoBehaviour
    {
        private void Update()
        {
            UpdateBillboard();
        }

        private void OnDrawGizmos()
        {
            UpdateBillboard();
        }

        private void UpdateBillboard()
        {
            Camera cam = Camera.main;

#if UNITY_EDITOR
            if (cam == null)
            {
                cam = Camera.current;
            }
#endif
            if (cam != null)
            {
                transform.LookAt(cam.transform.position);
                transform.forward = -transform.forward;
            }
        }
    }
}

