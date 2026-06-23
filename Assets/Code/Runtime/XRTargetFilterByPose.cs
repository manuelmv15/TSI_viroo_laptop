
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine;
using System;

using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


namespace CustomCode
{
    [Serializable]
    public class XRTargetFilterByPose : MonoBehaviour, IXRHoverFilter
    {
        public float positionTolerance = 2.0f; // Allowable position error
        public float rotationTolerance = 180f; // Allowable rotation error in degrees

        public bool canProcess => true;

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            AssemblyPart assemblyPart = interactable.transform.GetComponent<AssemblyPart>();
            AssemblyTarget assemblyTarget = interactor.transform.GetComponent<AssemblyTarget>();
            if (assemblyPart == null || assemblyTarget == null)
            {
                return false;
            }

            // Check position and rotation
            bool isPositionValid = Vector3.Distance(interactable.transform.position, interactor.transform.position) <= positionTolerance;
            bool isRotationValid = Quaternion.Angle(interactable.transform.rotation, interactor.transform.rotation) <= rotationTolerance;

            // Activate socket only if both are valid
            bool isValid = isPositionValid && isRotationValid;

            return isValid;
        }

        public void AddFilter(XRSocketInteractor interactor)
        {
            interactor.hoverFilters.Add(this);
        }
    }
}