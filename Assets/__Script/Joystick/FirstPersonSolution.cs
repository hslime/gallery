namespace zFrame.Example
{
    using zFrame.UI;
    using UnityEngine;
    public class FirstPersonSolution : MonoBehaviour
    {
        [SerializeField] Joystick joystick;
        [SerializeField] Joystick joystick2;
        public float speed = 2;
        CharacterController controller;

        [SerializeField] Camera m_Camera;
        public float rotateRange = 2f;
        public float viewRange = 2f;
        float ry = 0, rx = 0;

        void Start()
        {
            controller = GetComponent<CharacterController>();

            joystick.OnValueChanged.AddListener(v =>
            {
                if (v.magnitude != 0)
                {
                    float vx = v.x * (float)Screen.width / 1280f;
                    float vy = v.y * (float)Screen.height / 720f;

                    Vector3 direction = transform.TransformDirection(new Vector3(vx, 0, vy));
                    controller.SimpleMove(direction * speed);
                }
            });

            // joystick2.OnPointerUp.AddListener(v =>
            // {
            //     ry = transform.localEulerAngles.y;
            //     rx = m_Camera.transform.localEulerAngles.y;
            // });
            
            joystick2.OnValueChanged.AddListener(v =>
            {
                if (v.magnitude != 0)
                {
                    float vx = v.x * (float)Screen.width / 1280f;
                    float vy = v.y * (float)Screen.height / 720f;

                    const float minX = -60f;
                    const float maxX = 45f;
                    
                    ry = transform.localEulerAngles.y;
                    rx = m_Camera.transform.localEulerAngles.x;

                    if (rx > maxX)
                        rx -= 360f;
                    if (rx < minX)
                        rx += 360f;

                    float rotationy = ry + vx * rotateRange;
                    float rotationx = Mathf.Clamp(rx - vy * viewRange, minX, maxX);
                    m_Camera.transform.localEulerAngles = new Vector3(rotationx, 0, 0);
                    transform.localEulerAngles = new Vector3(0, rotationy, 0);
                }
            });
        }

        private void Update()
        {
            Vector3 dirInput = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dirInput.x = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dirInput.x = 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                dirInput.z = -1f;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                dirInput.z = 1f;
            }

            if (dirInput != Vector3.zero)
            {
                Vector3 direction = transform.TransformDirection(dirInput);
                controller.SimpleMove(direction * speed);
            }
        }
    }
}
