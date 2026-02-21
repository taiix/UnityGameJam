using UnityEngine;

public class LightCandle : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    private void Start()
    {
        Debug.Log("LightCandle script started");
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.gameObject.TryGetComponent(out CandleInteractable candle))
            {
                candle.Light();
            }
        }
    }
}
