using UnityEngine;

// Simple component to lock an object's world position and rotation after placement
public class KunciKedudukan : MonoBehaviour
{
    private Vector3 lockedPosition;
    private Quaternion lockedRotation;
    private bool locked = false;

    public void LockToPose(Vector3 position, Quaternion rotation)
    {
        lockedPosition = position;
        lockedRotation = rotation;
        locked = true;

        // If there is a Rigidbody, freeze it
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Disable any animator to prevent animation-driven movement
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
        // Detach from any parent so transforms from AROrigin or other parents won't move this object
        if (transform.parent != null)
        {
            transform.SetParent(null, true);
        }

        Debug.Log("[KunciKedudukan] Locked object at world position " + lockedPosition + ", rotation " + lockedRotation.eulerAngles);
    }

    void LateUpdate()
    {
        if (!locked) return;
        transform.position = lockedPosition;
        transform.rotation = lockedRotation;
    }
}
