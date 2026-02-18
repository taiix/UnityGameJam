using UnityEngine;
using UnityEngine.InputSystem;

public class Magic : MonoBehaviour
{
    private RFX4_EffectEvent magicEvents;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magicEvents = GetComponent<RFX4_EffectEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            magicEvents.ActivateEffect();
    }
}
