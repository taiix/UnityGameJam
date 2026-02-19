using UnityEngine;

public class StaircaseLoop : MonoBehaviour
{
    [Header("Anchors")]
    public Transform fromAnchor;   
    public Transform toAnchor;     

    [Header("Options")]
    public bool loopingEnabled = true;
    public float cooldownSeconds = 0.25f;

    float _cooldownUntil;

    void OnTriggerEnter(Collider other)
    {
        if (!loopingEnabled) return;
        if (Time.time < _cooldownUntil) return;

        
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        var mover = rb.GetComponent<CharacterMovement>();
        if (mover == null) return;

        Teleport(rb, mover);
        _cooldownUntil = Time.time + cooldownSeconds;
    }

    void Teleport(Rigidbody rb, CharacterMovement mover)
    {
        Transform player = rb.transform;

        Vector3 localPos = fromAnchor.InverseTransformPoint(player.position);
        Quaternion localRot = Quaternion.Inverse(fromAnchor.rotation) * player.rotation;

        Vector3 newPos = toAnchor.TransformPoint(localPos);
        Quaternion newRot = toAnchor.rotation * localRot;

        rb.position = newPos;
        rb.rotation = newRot;

        Vector3 localVel = fromAnchor.InverseTransformDirection(rb.linearVelocity);
        rb.linearVelocity = toAnchor.TransformDirection(localVel);

        mover.SyncYawFromCamera(player); 
    }
}
