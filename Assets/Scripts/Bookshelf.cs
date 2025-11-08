using UnityEngine;
using System.Collections.Generic;

public class Bookshelf : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool isMoving = false;
    private bool moveDirection; 
    private bool hasMoved = false;
    
    [SerializeField] private float moveDelayTimer = 2f;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    public void MoveBookShelf(bool moveLeft, float moveDistance, float delayTime)
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Rigidbody>(out var rb) && child.GetComponent<BookPickupInteractable>() != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        float direction = moveLeft ? -1f : 1f;
        targetPosition = originalPosition + new Vector3(direction * moveDistance, 0f, 0f);

        moveDirection = moveLeft;
        isMoving = true;
        hasMoved = false;
    }

    // New: move shelf back to its original position
    public void MoveBackToOriginal(float delayTime)
    {
        targetPosition = originalPosition;
        // Decide direction based on current position relative to original
        moveDirection = transform.position.x > originalPosition.x; // if right of original, move left
        isMoving = true;
        hasMoved = false;
    }

    private void Update()
    {
        

        if (isMoving && !hasMoved)
        {
            moveDelayTimer -= Time.deltaTime;

            if (moveDelayTimer > 0f)
            {
                return;
            }

            float direction = moveDirection ? -1f : 1f;
            Vector3 movement = new Vector3(direction * moveSpeed * Time.deltaTime, 0f, 0f);
            transform.position += movement;

            bool reached =
                (moveDirection && transform.position.x <= targetPosition.x) ||
                (!moveDirection && transform.position.x >= targetPosition.x);

            if (reached)
            {
                transform.position = targetPosition;
                isMoving = false;
                hasMoved = true;
                moveDelayTimer = 2f;
            }
        }
    }
}
