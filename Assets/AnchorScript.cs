using UnityEngine;

public class AnchorScript : MonoBehaviour
{
    public Vector3 surfaceNormal;
    private Rigidbody attachedSurfaceRigidbody; // Field to store the Rigidbody reference

    public void SetNormal(Vector3 normal)
    {
        surfaceNormal = normal;
    }

    public Vector3 GetNormal()
    {
        return surfaceNormal;
    }

    // Method to set the Rigidbody reference
    public void SetSurfaceRigidbody(Rigidbody rb)
    {
        attachedSurfaceRigidbody = rb;
    }

    // Method to get the Rigidbody reference
    public Rigidbody GetSurfaceRigidbody()
    {
        return attachedSurfaceRigidbody;
    }
}
