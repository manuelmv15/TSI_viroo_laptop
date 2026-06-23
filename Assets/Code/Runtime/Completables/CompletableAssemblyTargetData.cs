using UnityEngine;
using UnityEngine.Events;

namespace CustomCode
{
    public class CompletableAssemblyTargetData : CompletableItemData
    {
        public bool disableInteractableWhenPlaced;
        public float distanceTolerance;
        public float angleTolerance;
        public bool allowHoveringCompletion;
        public float hoveringTimeToComplete;
    }
}
