using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera MainCamera { get; private set; }


    private float gamePadSensitivity = 150.0f;
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private LayerMask ignoreLayer = 0;

    private Vector3 thirdPersonOffset = new Vector3(0.0f, 0.9f, -4.4f);
    private float thirdPersonSafety; // shortest allowed distance from a collider
    private Vector3 firstPersonOffset = new Vector3(0.0f, 0.5f, 0.0f);
    public float rotationX = 0.0f; // we want this private, but respawns affects this. make a respawn-listener in this script?
    public float rotationY = 0.0f; // we want this private, but respawns affects this. make a respawn-listener in this script?

    [SerializeField] private float aimFOV = 30;
    [SerializeField] private float aimSensitivityMultiplier = 0.1f;
    private float startingGamePadSensitivity;
    private float startingFOV;


    private void Awake()
    {
        MainCamera = GetComponent<Camera>();
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(OnPlayerRespawn);
    }

    private void Start()
    {
        thirdPersonSafety = MainCamera.nearClipPlane;
        startingFOV = MainCamera.fieldOfView;
        startingGamePadSensitivity = gamePadSensitivity;
    }

    private void LateUpdate()
    {
        transform.rotation = UpdateRotation();

        transform.position = playerTransform.position + firstPersonOffset;

        Vector3 newRelativePosition = transform.rotation * thirdPersonOffset;

        RaycastHit rayHit;

        if (Physics.SphereCast(transform.position, thirdPersonSafety, newRelativePosition.normalized, out rayHit, newRelativePosition.magnitude + thirdPersonSafety, ~ignoreLayer))
        {
            newRelativePosition = newRelativePosition.normalized * rayHit.distance;
        }

        transform.position = transform.position + newRelativePosition;
    }

    private Quaternion UpdateRotation()
    {
        rotationY += Input.GetAxisRaw("GP Look X") * gamePadSensitivity * Time.unscaledDeltaTime;
        rotationX += Input.GetAxisRaw("GP Look Y") * gamePadSensitivity * Time.unscaledDeltaTime;

        rotationX = Mathf.Clamp(rotationX, -85.0f, 85.0f);

        return Quaternion.Euler(rotationX, rotationY, 0.0f);
    }

    public void Aiming()
    {
        Camera.main.fieldOfView = aimFOV;
        gamePadSensitivity = startingGamePadSensitivity * aimSensitivityMultiplier;

        RaycastHit hit;
        if (Physics.Linecast(transform.position, transform.forward * 100f, out hit, playerTransform.GetComponent<PlayerStateMachine>().CollisionLayers))
        {
            if (hit.transform.gameObject.layer == 13)
                gamePadSensitivity = (startingGamePadSensitivity * aimSensitivityMultiplier) / 2;
            else
                gamePadSensitivity = startingGamePadSensitivity * aimSensitivityMultiplier;
        }
    }

    public void StopAiming()
    {
        Camera.main.fieldOfView = startingFOV;
        gamePadSensitivity = startingGamePadSensitivity;
    }

    public void OnPlayerRespawn(EventInfo EI)
    {
        PlayerRespawnEventInfo PREI = (PlayerRespawnEventInfo)EI;

        transform.rotation = PREI.GO.transform.rotation;
        rotationY = transform.rotation.eulerAngles.y;
        rotationX = transform.rotation.eulerAngles.x;
    }

    private void OnDestroy()
    {
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<PlayerRespawnEventInfo>(OnPlayerRespawn);
    }
}
