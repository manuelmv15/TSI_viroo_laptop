using UnityEngine;
using UnityEngine.Events;

namespace CustomCode
{
    public class CompletableItemData : MonoBehaviour
    {
        //public string id;

        private bool completed = false;
        private bool completing = false;

        public bool Completed
        {
            get { return completed; }
            set
            {
                completed = value;
                if (completed)
                {
                    onCompleted.Invoke();
                }
            }
        }

        public bool Completing
        {
            get { return completing; }
            set
            {
                completing = value;
            }
        }

        public UnityEvent onCompleted;
    }
}
