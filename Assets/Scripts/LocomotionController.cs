using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController teleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    public bool enableTeleport { get; set; } = true;

    // Update is called once per frame
    void LateUpdate()
    {
        if (teleportRay && enableTeleport)
        {
            teleportRay.gameObject.SetActive(CheckActivatedTeleport(teleportRay));
        }
    }

    public bool CheckActivatedTeleport(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActive, activationThreshold);
        return isActive;
    }
}
