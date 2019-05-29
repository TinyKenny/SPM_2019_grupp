using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private LayerMask ignoreLayer = 0;

    private Vector3 thirdPersonOffset = new Vector3(0.0f, 0.9f, -4.4f);
    private Vector3 firstPersonOffset = new Vector3(0.0f, 0.5f, 0.0f);

    private float gamePadSensitivity = 150.0f;
    private float thirdPersonSafety; // shortest allowed distance from a collider
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerRespawnEventInfo>(OnPlayerRespawn);
    }

    private void Start()
    {
        thirdPersonSafety = GetComponent<Camera>().nearClipPlane;
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
