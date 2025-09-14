using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class CloudMover : MonoBehaviour
{
    [Tooltip("Units per second the cloud drifts left.")]
    public float speed = 1f;

    [Tooltip("X position where the cloud destroys itself.")]
    public float destroyX = -50f;

    void Awake()
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = false;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}
