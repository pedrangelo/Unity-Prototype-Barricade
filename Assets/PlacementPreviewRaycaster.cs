using UnityEngine;

public class SurfacePreviewRaycaster : MonoBehaviour
{
    public float maxDistance = 10f; // Maximum distance a ray can travel
    public float angleBetweenRays = 90f; // Angle between the two horizontal rays, kept constant
    public LayerMask hitLayers; // Layer mask to specify which layers the rays can hit
    private float tiltAngle = 0f; // The current tilt angle, adjusted by mouse wheel
    public float tiltSensitivity = 5f; // How sensitive the tilt is to mouse wheel scrolling

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            // Adjust the tilt angle based on the mouse wheel scroll, affecting the tilt direction around a horizontal axis
            tiltAngle += Input.GetAxis("Mouse ScrollWheel") * tiltSensitivity;

            CastRays();
        }
    }

    private void CastRays()
    {
        // Calculate the base direction for the two rays without the tilt
        Vector3 baseDirection1 = Quaternion.Euler(0, -angleBetweenRays / 2, 0) * transform.forward;
        Vector3 baseDirection2 = Quaternion.Euler(0, angleBetweenRays / 2, 0) * transform.forward;

        // Apply the tiltAngle to rotate each ray around the camera's right axis (for a sideways tilt effect)
        Vector3 direction1 = Quaternion.AngleAxis(tiltAngle, transform.right) * baseDirection1;
        Vector3 direction2 = Quaternion.AngleAxis(tiltAngle, transform.right) * baseDirection2;

        Ray ray1 = new Ray(transform.position, direction1);
        Ray ray2 = new Ray(transform.position, direction2);

        RaycastHit hit1, hit2;

        // Cast the first ray
        bool isHit1 = Physics.Raycast(ray1, out hit1, maxDistance, hitLayers);
        // Cast the second ray
        bool isHit2 = Physics.Raycast(ray2, out hit2, maxDistance, hitLayers);

        // Visualize the rays
        if (isHit1)
        {
            Debug.DrawRay(transform.position, direction1 * hit1.distance, Color.blue);
        }
        else
        {
            Debug.DrawRay(transform.position, direction1 * maxDistance, Color.red);
        }

        if (isHit2)
        {
            Debug.DrawRay(transform.position, direction2 * hit2.distance, Color.blue);
        }
        else
        {
            Debug.DrawRay(transform.position, direction2 * maxDistance, Color.red);
        }

        // If both rays hit, cast a third ray between the hit points
        if (isHit1 && isHit2)
        {
            Vector3 thirdRayDirection = (hit2.point - hit1.point).normalized;
            float thirdRayDistance = Vector3.Distance(hit1.point, hit2.point);

            Debug.DrawRay(hit1.point, thirdRayDirection * thirdRayDistance, Color.green);
        }
    }
}
