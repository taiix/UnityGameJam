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
        float direction = moveLeft ? -1f : 1f;
        targetPosition = originalPosition + new Vector3(0, 0f, direction * moveDistance);

        moveDirection = moveLeft;
        isMoving = true;
        hasMoved = false;
    }


    public void MoveBackToOriginal(float delayTime)
    {
        targetPosition = originalPosition;
        moveDirection = transform.position.z > originalPosition.z; 
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
            Vector3 movement = new Vector3(0, 0f, direction * moveSpeed * Time.deltaTime);
            transform.position += movement;

            bool reached =
                (moveDirection && transform.position.z <= targetPosition.z) ||
                (!moveDirection && transform.position.z >= targetPosition.z);

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
