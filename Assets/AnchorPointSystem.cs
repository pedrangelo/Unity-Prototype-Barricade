using UnityEngine;

public class AnchorPointSpawner : MonoBehaviour
{
    public GameObject anchorPointPrefab; // Assign this in the inspector
    public float maxDistance = 10f; // Max distance for the raycast
    public float angleBetweenRays = 90f; // Angle between the two horizontal rays
    public LayerMask hitLayers; // Layer mask to specify which layers the rays can hit

    void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button clicked while holding right mouse button
            {
                TryPlaceAnchorPoints();
            }
        }
    }

    void TryPlaceAnchorPoints()
    {
        Vector3 direction1 = Quaternion.Euler(0, -angleBetweenRays / 2, 0) * transform.forward;
        Vector3 direction2 = Quaternion.Euler(0, angleBetweenRays / 2, 0) * transform.forward;

        RaycastHit hit1, hit2;
        bool isHit1 = Physics.Raycast(transform.position, direction1, out hit1, maxDistance, hitLayers);
        bool isHit2 = Physics.Raycast(transform.position, direction2, out hit2, maxDistance, hitLayers);

        if (isHit1 && isHit2)
        {
            // Create a new container for the anchor points
            GameObject currentPairContainer = new GameObject("AnchorPair");
            currentPairContainer.AddComponent<AnchorPairManager>();

            // Spawn anchors at the hit points
            SpawnAnchorPointAt(hit1.point, hit1.normal, currentPairContainer, hit1.collider.attachedRigidbody, true); // Anchor A
            SpawnAnchorPointAt(hit2.point, hit2.normal, currentPairContainer, hit2.collider.attachedRigidbody, false); // Anchor B

            // After placing both anchors, trigger any post-placement logic (e.g., connecting the anchors with a plank)
            currentPairContainer.GetComponent<AnchorPairManager>().CalculateAndSpawnPlank();
            Destroy(currentPairContainer);
        }
    }

    void SpawnAnchorPointAt(Vector3 hitPoint, Vector3 hitNormal, GameObject parentContainer, Rigidbody hitRigidbody, bool isAnchorA)
    {
        GameObject anchorPoint = Instantiate(anchorPointPrefab, hitPoint, Quaternion.LookRotation(hitNormal), parentContainer.transform);
        AnchorScript anchorScript = anchorPoint.GetComponent<AnchorScript>();
        if (anchorScript != null)
        {
            anchorScript.SetNormal(hitNormal);
            anchorScript.SetSurfaceRigidbody(hitRigidbody); // Store the hit surface's Rigidbody
        }
        else
        {
            Debug.LogError("Anchor prefab does not have an AnchorScript component attached.");
        }

        anchorPoint.name = isAnchorA ? "AnchorA" : "AnchorB";
        Debug.Log(isAnchorA ? "Placed Anchor A" : "Placed Anchor B", anchorPoint);
    }
}
