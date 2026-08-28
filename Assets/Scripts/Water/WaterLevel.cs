using UnityEngine;
public class WaterLevel : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 0.1f;
    private bool rising = true;

    private void Update()
    {
        if (!rising) return;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
    }
    public void SetRising(bool value) => rising = value;
}