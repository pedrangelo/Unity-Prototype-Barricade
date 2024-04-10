using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public float grabDistance = 2f;
    public float objectHoldDistance = 1f;
    public float grabbableMassLimit = 5f;
    private GameObject heldObject = null;
    private Camera playerCamera;
    private Vector3 holdPosition;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        holdPosition = playerCamera.transform.position + playerCamera.transform.forward * objectHoldDistance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryGrabObject();
            }
            else
            {
                DropObject();
            }
        }

        if (heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void TryGrabObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance))
        {
            Rigidbody hitRigidbody = hit.rigidbody;
            if (hitRigidbody != null && hitRigidbody.mass <= grabbableMassLimit)
            {
                GrabObject(hitRigidbody.gameObject, hitRigidbody);
            }
        }
    }

    private Rigidbody heldObjectRigidbody; // Reference to the Rigidbody of the held object
    public float smoothTime = 0.1f; // Time taken to move towards the target position
    private Vector3 velocity = Vector3.zero; // Velocity reference for smooth damping

    void GrabObject(GameObject obj, Rigidbody objRigidbody)
    {
        heldObject = obj;
        heldObjectRigidbody = objRigidbody;
        heldObjectRigidbody.useGravity = false; // Optionally disable gravity
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectRigidbody.useGravity = true; // Re-enable gravity upon dropping
            heldObject = null;
            heldObjectRigidbody = null;
        }
    }

    void MoveHeldObject()
    {
        if (heldObjectRigidbody == null) return;

        holdPosition = playerCamera.transform.position + playerCamera.transform.forward * objectHoldDistance;

        // Calculate the force vector needed to move the object towards the holdPosition smoothly
        Vector3 forceDirection = holdPosition - heldObject.transform.position;
        Vector3 desiredVelocity = forceDirection / smoothTime; // Desired velocity to reach the target position in the specified smoothTime
        Vector3 force = (desiredVelocity - heldObjectRigidbody.velocity) * heldObjectRigidbody.mass; // Calculate the force required to achieve the desired velocity

        // Apply the force to move the object
        heldObjectRigidbody.AddForce(force);

        // Optional: Align the object's rotation with the camera's rotation for a consistent facing direction
        Quaternion targetRotation = Quaternion.LookRotation(playerCamera.transform.forward);
        heldObjectRigidbody.MoveRotation(Quaternion.Lerp(heldObject.transform.rotation, targetRotation, smoothTime));
    }

}
