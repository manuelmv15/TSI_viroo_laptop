using UnityEngine;
using TMPro;

namespace CustomCode
{
    public class CompletableItemUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textId;

        [SerializeField]
        private TMP_Text textTime;

        [SerializeField]
        private TMP_Text textUserId;

        public void SetData(string id, float time, string userId)
        {
            textId.text = id;
            textTime.text = time.ToString();
            textUserId.text = userId;
        }
    }
}
