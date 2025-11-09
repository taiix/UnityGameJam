using System;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [Header("Attach Target")]
    public Transform attachParent;

    [Header("Local Pose On Attach")]
    public Vector3 swordPosition = new Vector3(0, 0.075f, 0.02f);
    [SerializeField] private Vector3 swordRotation = new Vector3(133.965f, -6.997009f, -274.438f);

    public bool HasSword { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (HasSword) return;
        var a = other.GetComponent<Ingredient>();
        if (a == null) return;

        if (!a.isPickedUp) a.PickUp();
        else return;

        var pickup = other.GetComponent<PickupInteractable>();
        if (pickup == null) return;

        pickup.EndHold();

        Transform parent = attachParent != null ? attachParent : transform;
        Transform itemT = pickup.transform;
        itemT.SetParent(parent, worldPositionStays: false);
        itemT.localPosition = swordPosition;
        itemT.localEulerAngles = swordRotation;

        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

        }

        HasSword = true;
        Destroy(pickup);
    }
}
