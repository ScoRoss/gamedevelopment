using UnityEngine;


namespace FMS_AdvancedZombieAI
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        public float walkSpeed = 3.0f;
        public float runSpeed = 6.0f;
        public float crouchSpeed = 1.5f;
        [Space]
        public float GroundDistance = 0.3f;
        public Transform GroundCheck;
        public LayerMask groundMask;
        [Space]
        public float crouchTransitionSpeed = 5f;
        Vector3 velocity;
        public float jumpheight = 3f;
        public float gravity = -7f;
        public float lookSpeed = 2.0f;
        public Animator camHolderAnimator;
        private Camera cam;
        private CharacterController cc;
        private float rotationX = 0;


        [HideInInspector] public bool isWalking = false;
        [HideInInspector] public bool isRunning = false;
        [HideInInspector] public bool isCrouching = false;
        [Space]
        private bool isGrounded;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            cam = GetComponentInChildren<Camera>();
            cc = GetComponent<CharacterController>();
        }

        void Update()
        {
            HandleMovementInput();
            HandleMouseLook();
        }

        void HandleMovementInput()
        {

            isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, groundMask);

            Vector3 moveDir = Vector3.zero;



            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                moveDir.z += 1;
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                moveDir.z -= 1;
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                moveDir.x += 1;
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                moveDir.x -= 1;



            if (Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.S))
            {
                moveDir *= runSpeed;

                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 2, 0), crouchTransitionSpeed * Time.deltaTime);
                cc.height = Mathf.Lerp(cc.height, 2, crouchTransitionSpeed * Time.deltaTime);
                cc.center = Vector3.Lerp(cc.center, new Vector3(0, 1, 0), crouchTransitionSpeed * Time.deltaTime);

                isCrouching = false;
                isWalking = false;
                isRunning = true;
                camHolderAnimator.SetBool("Running", true);

                camHolderAnimator.SetBool("Walking", false);

            }
            else if (Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
            {
                moveDir *= crouchSpeed;


                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 1, 0), crouchTransitionSpeed * Time.deltaTime);
                cc.height = Mathf.Lerp(cc.height, 1.2f, crouchTransitionSpeed * Time.deltaTime);
                cc.center = Vector3.Lerp(cc.center, new Vector3(0, 0.59f, 0), crouchTransitionSpeed * Time.deltaTime);

                isCrouching = true;
                isWalking = false;
                isRunning = false;
            }
            else
            {
                moveDir *= walkSpeed;

                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 2, 0), crouchTransitionSpeed * Time.deltaTime);
                cc.height = Mathf.Lerp(cc.height, 2, crouchTransitionSpeed * Time.deltaTime);
                cc.center = Vector3.Lerp(cc.center, new Vector3(0, 1, 0), crouchTransitionSpeed * Time.deltaTime);

                isCrouching = false;
                isWalking = true;
                isRunning = false;
                camHolderAnimator.SetBool("Walking", true);
                camHolderAnimator.SetBool("Running", false);

            }

            if (moveDir == Vector3.zero)
            {
                isCrouching = false;
                isWalking = false;
                isRunning = false;
                camHolderAnimator.SetBool("Walking", false);
                camHolderAnimator.SetBool("Running", false);

            }



            if (isGrounded && Input.GetKey(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
                isGrounded = false;
            }

            velocity.y += gravity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= Time.deltaTime;

            cc.Move(moveDir);

        }

        void HandleMouseLook()
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

    }
}