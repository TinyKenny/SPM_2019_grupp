using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSensitivity = 10.0f;
    public float gamePadSensitivity = 15.0f;
    public float mouseSensitivity = 1.0f;
    public Transform playerTransform;
    public LayerMask playerLayer;

    [Header("Perspective (first/third person)")]
    public bool thirdPerson = false;

    private Vector3 thirdPersonOffset = new Vector3(0.0f, 0.9f, -4.4f);
    private float thirdPersonSafety = 0.3f; // shortest allowed distance from a collider
    private Vector3 firstPersonOffset = new Vector3(0.0f, 0.5f, 0.0f);
    public float rotationX = 0.0f; // make this private
    public float rotationY = 0.0f; // make this private

    [SerializeField] private float aimFOV = 30;
    [SerializeField] private float aimSensitivityMultiplier = 0.1f;
    private float startingGamePadSensitivity;
    private float startingFOV;


    private void Start()
    {
        startingGamePadSensitivity = gamePadSensitivity;
        startingFOV = Camera.main.fieldOfView;
        //playerTransform = transform.parent;
    }

    private void LateUpdate()
    {
        /*
        if (Input.GetButtonDown("PerspectiveSwitch"))
        {
            thirdPerson = !thirdPerson;
        }
        */

        Quaternion newRotation = UpdateRotation();
        transform.rotation = newRotation;

        transform.position = playerTransform.position + firstPersonOffset; // make the camera first person

        if (thirdPerson)
        {
            /*
             * When setting the position of the third person camera,
             * we want to use the position of the first person camera as our pivot.
             */
            Vector3 newRelativePosition = newRotation * thirdPersonOffset;

            RaycastHit rayHit;

            if (Physics.SphereCast(transform.position, thirdPersonSafety, newRelativePosition.normalized, out rayHit, newRelativePosition.magnitude + thirdPersonSafety, ~playerLayer))
            {
                newRelativePosition = newRelativePosition.normalized * rayHit.distance;
            }

            transform.position = transform.position + newRelativePosition;
        }
    }

    private Quaternion UpdateRotation()
    {
        // Multiply these two by 0.0f if you want to do anything in the inspector while the editor is in "Play"-mode.
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * 0.0f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 0.0f;

        float gamePadX = Input.GetAxisRaw("GP Look X") * gamePadSensitivity * Time.unscaledDeltaTime;
        float gamePadY = Input.GetAxisRaw("GP Look Y") * gamePadSensitivity * Time.unscaledDeltaTime;

        if (new Vector2(mouseX, mouseY).magnitude > new Vector2(gamePadX, gamePadY).magnitude)
        {
            rotationY += mouseX * lookSensitivity;
            rotationX -= mouseY * lookSensitivity;
        }
        else
        {
            rotationY += gamePadX * lookSensitivity;
            rotationX += gamePadY * lookSensitivity;
        }

        rotationX = Mathf.Clamp(rotationX, -85.0f, 85.0f);

        return Quaternion.Euler(rotationX, rotationY, 0.0f);
    }

    public void Aiming()
    {
        Camera.main.fieldOfView = aimFOV;
        gamePadSensitivity = startingGamePadSensitivity * aimSensitivityMultiplier;

        RaycastHit hit;
        if (Physics.Linecast(transform.position, transform.forward * 100f, out hit, playerTransform.GetComponent<PlayerStateMachine>().collisionLayers))
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
}
