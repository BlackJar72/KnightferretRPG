using UnityEngine;

namespace DistantLands.Utility
{
    public class FreeCam : MonoBehaviour
    {
        public float normalSpeed = 5.0f;
        public float fastSpeedMultiplier = 2.0f;
        public float rotationSpeed = 2.0f;

        private float currentSpeedMultiplier = 1.0f;

        void Update()
        {
            HandleInput();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }

        void HandleInput()
        {
            // Toggle between normal and fast speed using the Shift key
            currentSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? fastSpeedMultiplier : 1.0f;

            // Set current speed based on the multiplier
            float currentSpeed = normalSpeed * currentSpeedMultiplier;

            // Handle camera movement
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            float upDown = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;

            Vector3 direction = new Vector3(horizontal, upDown, vertical).normalized;
            Vector3 moveVector = transform.TransformDirection(direction) * currentSpeed * Time.deltaTime;
            transform.Translate(moveVector, Space.World);

            // Handle camera rotation
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed; // Invert Y-axis for more intuitive control

            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(Vector3.right, mouseY, Space.Self);

        }
    }
}