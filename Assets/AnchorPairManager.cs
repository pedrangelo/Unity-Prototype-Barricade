using UnityEngine;
using UnityEngine.ProBuilder;

public class AnchorPairManager : MonoBehaviour
{
    public float maxDistance = 5f;
    private bool hasCalculated = false;
    public float normalAlignmentTolerance = 5f; // Tolerance in degrees for normal alignment

    // Variables to store the last raycast start and end points for Gizmos
    private Vector3 lastRaycastFrom = Vector3.zero;
    private Vector3 lastRaycastTo = Vector3.zero;

    public void CalculateAndSpawnPlank()
    {
        if (hasCalculated)
        {
            Debug.Log("Calculation has already been done.");
            return;
        }

        if (transform.childCount == 2)
        {
            GameObject anchorA = transform.GetChild(0).gameObject;
            GameObject anchorB = transform.GetChild(1).gameObject;

            Vector3 normalA = anchorA.GetComponent<AnchorScript>().GetNormal();
            Vector3 normalB = anchorB.GetComponent<AnchorScript>().GetNormal();
            Rigidbody surfaceRigidbodyA = anchorA.GetComponent<AnchorScript>().GetSurfaceRigidbody();
            Rigidbody surfaceRigidbodyB = anchorB.GetComponent<AnchorScript>().GetSurfaceRigidbody();

            bool normalsAligned = Vector3.Angle(normalA, normalB) <= normalAlignmentTolerance;
            Debug.Log($"Normals Aligned: {normalsAligned}");

            Vector3 positionA = anchorA.transform.position;
            Vector3 positionB = anchorB.transform.position;

            // Adjusted positions for the raycast, considering the surface normal direction
            float anchorHeight = 0.1f; // Assumi    qng a fixed height for the anchor
            Vector3 adjustedPositionA = positionA + normalA * anchorHeight;
            Vector3 adjustedPositionB = positionB + normalB * anchorHeight;

            // Store the adjusted raycast positions for drawing Gizmos
            lastRaycastFrom = adjustedPositionA;
            lastRaycastTo = adjustedPositionB;

            float distance = Vector3.Distance(adjustedPositionA, adjustedPositionB);
            bool isPathClear = !Physics.Raycast(adjustedPositionA, (adjustedPositionB - adjustedPositionA).normalized, distance);
            Debug.Log($"Raycast from {adjustedPositionA} to {adjustedPositionB}, Path Clear: {isPathClear}");

            if (distance <= maxDistance && normalsAligned && isPathClear)
            {
                Debug.Log("Conditions met, spawning plank.");
                SpawnPlankBetweenAnchors(positionA, positionB, normalA, surfaceRigidbodyA, surfaceRigidbodyB);
                Destroy(anchorA);
                Destroy(anchorB);
            }
            else
            {
                Debug.Log("Conditions not met, plank not spawned.");
            }

            hasCalculated = true;
        }
    }

    void SpawnPlankBetweenAnchors(Vector3 positionA, Vector3 positionB, Vector3 surfaceNormal, Rigidbody surfaceRigidbodyA, Rigidbody surfaceRigidbodyB)
    {
        Vector3 midPoint = (positionA + positionB) / 2;
        float length = Vector3.Distance(positionA, positionB);
        float thickness = 0.1f;
        float width = 0.5f;

        ProBuilderMesh pbMesh = ShapeGenerator.CreateShape(ShapeType.Cube);
        GameObject plank = pbMesh.gameObject;
        pbMesh.transform.localScale = new Vector3(length, width, thickness);

        Vector3 elevatedMidPoint = midPoint + surfaceNormal * (thickness / 2);
        pbMesh.transform.position = elevatedMidPoint;

        Vector3 direction = (positionB - positionA).normalized;
        Quaternion rotationToNormal = Quaternion.LookRotation(direction, surfaceNormal);
        pbMesh.transform.rotation = rotationToNormal * Quaternion.Euler(90f, 0f, 90f);

        // Add Rigidbody and set it to kinematic
        Rigidbody rb = plank.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Ensure there's a collider for physical interactions
        plank.AddComponent<BoxCollider>();

        // Add destruction script
        plank.AddComponent<DestructibleObject>();

        // Debug logs to check if the surfaceRigidbodies are null
        Debug.Log($"surfaceRigidbodyA is {(surfaceRigidbodyA == null ? "null" : "not null")}");
        Debug.Log($"surfaceRigidbodyB is {(surfaceRigidbodyB == null ? "null" : "not null")}");

        // Attach joints if the surface rigidbodies are provided
        if (surfaceRigidbodyA != null && surfaceRigidbodyB != null)
        {
            AttachJointToSurface(plank, positionA, surfaceRigidbodyA);
            AttachJointToSurface(plank, positionB, surfaceRigidbodyB);
            rb.isKinematic = false;
        }

        Debug.Log("Plank spawned between anchors with Rigidbody and joints.");
    }

    void AttachJointToSurface(GameObject plank, Vector3 anchorPosition, Rigidbody surfaceRigidbody)
    {
        // Calculate the local position of the anchor point relative to the plank
        Vector3 localAnchorPosition = plank.transform.InverseTransformPoint(anchorPosition);

        // Create the hinge joint at the anchor's position on the plank
        HingeJoint hinge = plank.AddComponent<HingeJoint>();
        hinge.anchor = localAnchorPosition;
        hinge.connectedBody = surfaceRigidbody;
        hinge.autoConfigureConnectedAnchor = true; // Let Unity automatically configure the connected anchor based on current setup
        hinge.axis = new Vector3(0, 0, -1);
        hinge.breakForce = 20f;
        hinge.breakTorque = 20f;

        Debug.Log("Hinge joint attached to surface.");
    }

    void OnDrawGizmos()
    {
        if (lastRaycastFrom != Vector3.zero && lastRaycastTo != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastRaycastFrom, lastRaycastTo);
        }
    }
}
