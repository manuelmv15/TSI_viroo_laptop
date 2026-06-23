using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Viroo.VisualScripting.Runtime
{
    public class XRSocketInteractorWithTag : XRSocketInteractor
    {
        public string tagToCheck = "";

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            TagScript ts = interactable.transform.GetComponent<TagScript>();
            if (ts == null)
            {
                return false;
            }
            return base.CanHover(interactable) && (ts.tagName == tagToCheck);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            TagScript ts = interactable.transform.GetComponent<TagScript>();
            if (ts == null)
            {
                return false;
            }
            return base.CanSelect(interactable) && (ts.tagName == tagToCheck);
        }

    }
}
