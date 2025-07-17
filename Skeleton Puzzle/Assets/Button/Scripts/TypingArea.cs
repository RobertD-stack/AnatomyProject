using UnityEngine;


public class TypingArea : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject leftTypingHand;
    public GameObject rightTypingHand;

    private void OnTriggerEnter(Collider other)
    {
        // Check if this collider belongs to a direct interactor
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        if (interactor == null) return;

        GameObject hand = interactor.gameObject;

        if (hand == leftHand)
        {
            leftTypingHand.SetActive(true);
        }
        else if (hand == rightHand)
        {
            rightTypingHand.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        if (interactor == null) return;

        GameObject hand = interactor.gameObject;

        if (hand == leftHand)
        {
            leftTypingHand.SetActive(false);
        }
        else if (hand == rightHand)
        {
            rightTypingHand.SetActive(false);
        }
    }
}
