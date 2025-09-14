using UnityEngine;

namespace YgoCGPTE.Camera
{
    /// <summary>
    /// Basic free-fly camera using WASD controls with optional smoothing.
    /// </summary>
    public class FreeCameraController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Movement speed in units per second.")]
        private float speed = 5f;

        [SerializeField]
        [Tooltip("Interpolation factor applied each frame.")]
        private float smoothing = 10f;

        private Vector3 targetPosition;

        private void Awake()
        {
            targetPosition = transform.position;
        }

        private void Update()
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                direction += transform.forward;
            if (Input.GetKey(KeyCode.S))
                direction -= transform.forward;
            if (Input.GetKey(KeyCode.A))
                direction -= transform.right;
            if (Input.GetKey(KeyCode.D))
                direction += transform.right;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0f)
            {
                direction.Normalize();
                targetPosition += direction * speed * Time.deltaTime;
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        }
    }
}
