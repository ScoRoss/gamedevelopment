using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FMS_AdvancedZombieAI
{
    public class WeaponSway : MonoBehaviour
    {
        public float swayAmount = 0.1f;
        public float swaySpeed = 5.0f;
        public float returnSpeed = 2.0f;

        private Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.localPosition;
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 sway = new Vector3(-mouseX * swayAmount, -mouseY * swayAmount, 0);

            Vector3 targetPosition = initialPosition + sway;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swaySpeed);

            if (Mathf.Abs(mouseX) < 0.01f && Mathf.Abs(mouseY) < 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * returnSpeed);
            }
        }
    }
}
