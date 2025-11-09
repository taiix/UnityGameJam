using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public GameObject doorToClose;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorToClose.GetComponent<Animator>().SetTrigger("closeDoor");
        }
    }
}
