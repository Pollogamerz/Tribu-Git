using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualJump : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    private bool isJumping = false;
    private float jumpTime = 0f;

    void Update()
    {
        // Iniciar salto
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            jumpTime = 0f;
        }

        // Simular el salto visual
        if (isJumping)
        {
            jumpTime += Time.deltaTime;

            float progress = jumpTime / jumpDuration;
            float offsetY = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            
            visualTransform.localPosition = new Vector3(0f, offsetY, 0f);

            if (progress >= 1f)
            {
                isJumping = false;
                visualTransform.localPosition = Vector3.zero;
            }
        }
    }
}