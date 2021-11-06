using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : Entity
{
    //- Character Control Vars -
    [Header("Character Control")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float jetpackSpeed = 5;
    [SerializeField] private float jumpSpeed = 6;
    [SerializeField] private float jetpackVerticalSpeed = 4;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float smoothMovement = 0.1f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float additionalHeight = 0.2f;

    private float fallingSpeed;

    private Vector2 inputAxis;

    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;
    float velocityZSmoothing;

    private CharacterController characterCtrl;

    //- Input Vars -
    private XRRig rig;

    [Header("Input Sources")]
    [SerializeField] private XRNode leftHandInputSource;
    private InputDevice leftHandDevice;

    [SerializeField] private XRNode rightHandInputSource;
    private InputDevice rightHandDevice;

    enum INPUT_KEYS
    {
        PrimaryButton
    }

    Dictionary<INPUT_KEYS, bool> rightHandState;

    protected override void Awake()
    {
        base.Awake();

        characterCtrl = GetComponent<CharacterController>();
        rig = GetComponentInChildren<XRRig>();
    }

    private void Start()
    {
        leftHandDevice = InputDevices.GetDeviceAtXRNode(leftHandInputSource);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(rightHandInputSource);

        rightHandState = new Dictionary<INPUT_KEYS, bool>
        {
            { INPUT_KEYS.PrimaryButton, false }
        };

        SetState(new PlayerGroundedState(this));
    }

    void CapsuleFollowHeadset()
    {
        characterCtrl.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = rig.cameraInRigSpacePos;
        characterCtrl.center = new Vector3(capsuleCenter.x, characterCtrl.height / 2 + characterCtrl.skinWidth, capsuleCenter.z);
    }

    public void GroundMovement()
    {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);

        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        //Gravity
        if (IsGrounded())
        {
            rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool jumped);

            if (jumped)
            {
                fallingSpeed = jumpSpeed;
            }
            else
            {
                fallingSpeed = 0;
            }
        }
        else
        {
            fallingSpeed += gravity * Time.fixedDeltaTime;
        }

        characterCtrl.Move(((Vector3.up * fallingSpeed) + (direction * speed)) * Time.fixedDeltaTime);
    }

    public void JetpackMovement()
    {
        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);

        rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool turboPressed);

        Vector3 targetVelVector = (headYaw * new Vector3(inputAxis.x, 0, inputAxis.y)).normalized * jetpackSpeed * (turboPressed ? 2.0f : 1.0f);

        rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerPressed);
        leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTriggerPressed);

        rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGripPressed);
        leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGripPressed);

        bool upMovement = rightTriggerPressed && leftTriggerPressed;
        bool downMovement = rightGripPressed && leftGripPressed;

        if (upMovement)
        {
            targetVelVector += Vector3.up * jetpackVerticalSpeed;
        }
        
        if (downMovement)
        {
            targetVelVector -= Vector3.up * jetpackVerticalSpeed;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelVector.x, ref velocityXSmoothing, smoothMovement);
        velocity.z = Mathf.SmoothDamp(velocity.z, targetVelVector.z, ref velocityZSmoothing, smoothMovement);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelVector.y, ref velocityYSmoothing, smoothMovement);

        characterCtrl.Move(velocity * Time.fixedDeltaTime);
    }

    public void ResetMovement()
    {
        fallingSpeed = 0.0f;
    }

    public bool IsGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(characterCtrl.center);
        float rayLength = characterCtrl.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, characterCtrl.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }

    public bool CheckMovementModeChanged()
    {
        rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed);

        if (pressed != rightHandState[INPUT_KEYS.PrimaryButton])
        {
            if (pressed == true)
            {
                rightHandState[INPUT_KEYS.PrimaryButton] = pressed;
                return !IsGrounded();
            }

            rightHandState[INPUT_KEYS.PrimaryButton] = pressed;
        }

        return false;
    }

    public void GetMovementInput()
    {
        leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }
}