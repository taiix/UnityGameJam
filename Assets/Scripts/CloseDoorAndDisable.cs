using UnityEngine;

public class CloseDoorAndDisable : MonoBehaviour
{
    public GameObject roomToDisable;

    public void CloseDoor()
    {
        if (roomToDisable != null)
        {
            roomToDisable.SetActive(false);
        }
    }
}