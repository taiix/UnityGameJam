using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float defaultMoveDelay = 2f;
    [SerializeField] private string slideSoundName = "SlidingBookshelf";

    private Vector3 targetPosition;
    private Vector3 originalPosition;

    private bool isMoving;
    private bool moveDirection;          // true = left (negative z in your current logic)
    private bool hasMoved;
    private bool movementSoundPlayed;    // ensures PlaySound is called only once per move after delay

    private float moveDelayTimer;

    private void Awake()
    {
        originalPosition = transform.position;
        moveDelayTimer = defaultMoveDelay;
    }

    // Starts movement away from original position.
    public void MoveBookShelf(bool moveLeft, float moveDistance, float delayTime)
    {
        float direction = moveLeft ? -1f : 1f;
        targetPosition = originalPosition + new Vector3(0f, 0f, direction * moveDistance);

        moveDirection = moveLeft;
        isMoving = true;
        hasMoved = false;
        movementSoundPlayed = false;
        moveDelayTimer = delayTime > 0f ? delayTime : defaultMoveDelay;
    }

    // Starts movement back to original position.
    public void MoveBackToOriginal(float delayTime)
    {
        targetPosition = originalPosition;

        // Determine direction sign for movement (true means we will move negatively)
        moveDirection = transform.position.z > originalPosition.z;
        isMoving = true;
        hasMoved = false;
        movementSoundPlayed = false;
        moveDelayTimer = delayTime > 0f ? delayTime : defaultMoveDelay;
    }

    private void Update()
    {
        if (!isMoving || hasMoved) return;

        // Countdown delay
        moveDelayTimer -= Time.deltaTime;
        if (moveDelayTimer > 0f) return;

        // Play the sliding sound ONCE when movement actually starts (after delay)
        if (!movementSoundPlayed)
        {
            if (SoundManager.instance != null)
                SoundManager.instance.PlaySound(slideSoundName); // Will play only once
            movementSoundPlayed = true;
        }

        // Movement step
        float direction = moveDirection ? -1f : 1f;
        Vector3 movement = new Vector3(0f, 0f, direction * moveSpeed * Time.deltaTime);
        transform.position += movement;

        bool reached =
            (moveDirection && transform.position.z <= targetPosition.z) ||
            (!moveDirection && transform.position.z >= targetPosition.z);

        if (reached)
        {
            transform.position = targetPosition;
            isMoving = false;
            hasMoved = true;
            // Reset delay timer in case you reuse without parameters
            moveDelayTimer = defaultMoveDelay;
        }
    }
}
