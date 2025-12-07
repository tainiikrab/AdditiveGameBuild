using UnityEngine;

public class SupportModel : MonoBehaviour
{
    [Header("Impulse Settings")] public float upwardForce = 2f; // как сильно подбрасывает вверх
    public float sideForce = 1.5f; // сила бокового толчка
    public Vector3 additionalTorque = new(80f, 50f, 30f); // вращение при падении
    [SerializeField] private LayerMask nonInteractableLayer;

    public void Fall()
    {
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(nonInteractableLayer.value, 2));

        // Detach from parent
        transform.parent = null;

        // Add Rigidbody if not already present
        var rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.mass = 1f;
        rb.useGravity = true;

        // ---------------------------------
        // ADD IMPULSE FORCE UP & SIDE
        // ---------------------------------

        // Randomize left/right
        var dir = Random.value < 0.5f ? -1f : 1f;

        // Build impulse vector
        var impulse =
            Vector3.up * upwardForce +
            Vector3.right * sideForce * dir;

        rb.AddForce(impulse, ForceMode.Impulse);

        // Add random torque so they spin nicely
        rb.AddTorque(
            new Vector3(
                additionalTorque.x * Random.Range(0.7f, 1.3f),
                additionalTorque.y * Random.Range(0.7f, 1.3f),
                additionalTorque.z * Random.Range(0.7f, 1.3f)
            ),
            ForceMode.Impulse
        );

        // Destroy after 2 seconds
        Destroy(gameObject, 2f);
    }
}