using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static UnityEngine.GraphicsBuffer;

namespace CustomCode
{
    [Serializable]
    public class XRTargetFilterByPartType : MonoBehaviour, IXRHoverFilter, IXRSelectFilter
    {
        public bool canProcess => true;

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            AssemblyPart assemblyPart = interactable.transform.GetComponent<AssemblyPart>();
            AssemblyTarget assemblyTarget = interactor.transform.GetComponent<AssemblyTarget>();
            if (assemblyPart == null || assemblyTarget == null)
            {
                return false;
            }

            return assemblyPart.partType == assemblyTarget.partType ? true : false;
        }

        public void AddFilter(XRSocketInteractor interactor)
        {
            interactor.hoverFilters.Add(this);
            interactor.selectFilters.Add(this);
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            AssemblyPart assemblyPart = interactable.transform.GetComponent<AssemblyPart>();
            AssemblyTarget assemblyTarget = interactor.transform.GetComponent<AssemblyTarget>();
            if (assemblyPart == null || assemblyTarget == null)
            {
                return false;
            }

            return assemblyPart.partType == assemblyTarget.partType ? true : false;
        }
    }
}