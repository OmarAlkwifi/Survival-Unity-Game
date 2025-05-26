using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tensori.FPSHandsHorrorPack
{
    public class FPSHandsController : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        public bool IsAiming = false;

        [Header("Object References")]
        [SerializeField] private Transform handsParentTransform = null;
        [SerializeField] private Transform handsTransform = null;
        [SerializeField] private Transform itemSlotTransform = null;

        [Header("Item Settings")]
        [Tooltip("The item currently held by the hands.")]
        [SerializeField] private GameObject heldItem = null;

        private Vector2 movementBouncePositionOffset = Vector2.zero;
        private Vector3 handsPositionOffset = Vector3.zero;
        private Vector3 handsPositionVelocity = Vector3.zero;
        private Vector3 handsEulerOffset = Vector3.zero;
        private Vector3 handsEulerVelocity = Vector3.zero;

        private void LateUpdate()
        {
            UpdateInput();
            UpdateMovementBounce(deltaTime: Time.deltaTime);
            UpdateHandsPosition();
        }

        private void UpdateInput()
        {
            if (Input.GetKeyDown(aimKey))
                IsAiming = true;
            if (Input.GetKeyUp(aimKey))
                IsAiming = false;

            if (Input.GetKeyDown(interactKey))
            {
                Debug.Log("Interacted");
                // Add interaction logic here
            }
        }

        private void UpdateMovementBounce(float deltaTime)
        {
            if (handsParentTransform == null || heldItem == null)
                return;

            float sine = Mathf.Sin(Time.time * 2.0f);
            float cos = Mathf.Cos(Time.time * 1.0f);

            movementBouncePositionOffset += new Vector2(
                deltaTime * ((0.5f - cos) * 2f) * 0.1f, // Example horizontal bounce
                deltaTime * sine * 0.05f // Example vertical bounce
            );

            Vector2 dampingForce =
                (-movementBouncePositionOffset * 10f) - // Spring stiffness
                (deltaTime * movementBouncePositionOffset * 5f); // Damping

            movementBouncePositionOffset += deltaTime * dampingForce;
        }

        private void UpdateHandsPosition()
        {
            if (handsTransform == null || handsParentTransform == null)
                return;

            handsPositionOffset = Vector3.SmoothDamp(
                current: handsPositionOffset,
                target: Vector3.zero,
                currentVelocity: ref handsPositionVelocity,
                smoothTime: 0.1f,
                maxSpeed: float.MaxValue,
                deltaTime: Time.deltaTime);

            handsEulerOffset = Vector3.SmoothDamp(
                current: handsEulerOffset,
                target: Vector3.zero,
                currentVelocity: ref handsEulerVelocity,
                smoothTime: 0.1f,
                maxSpeed: float.MaxValue,
                deltaTime: Time.deltaTime);

            Vector3 targetPosition = handsParentTransform.position + handsParentTransform.TransformVector(handsPositionOffset + (Vector3)movementBouncePositionOffset);
            Quaternion targetRotation = handsParentTransform.rotation * Quaternion.Euler(handsEulerOffset);

            handsTransform.SetPositionAndRotation(targetPosition, targetRotation);
        }

        public void SetHeldItem(GameObject item)
        {
            if (heldItem != null)
            {
                Destroy(heldItem);
            }

            heldItem = Instantiate(item, itemSlotTransform);
            Debug.Log("New item equipped: " + heldItem.name);
        }

        public GameObject GetHeldItem()
        {

            return heldItem;
        }
    }
}
