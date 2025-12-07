using UnityEngine;

public class SupportModel : MonoBehaviour
{
    public void Fall()
    {
        // Increment counter
        PostprocessMinigame.removedSupports++;

        // Detach from parent
        transform.parent = null;

        // Add Rigidbody if not already present
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Optional: tweak physics for nicer fall
        rb.mass = 1f;
        rb.useGravity = true;

        // Destroy after 2 seconds
        Destroy(gameObject, 2f);
    }
}