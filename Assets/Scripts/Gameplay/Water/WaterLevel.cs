using UnityEngine;
public class WaterLevel : MonoBehaviour
{
    [Header("Water Level Settings")]
    [SerializeField] private float riseSpeed = 0.1f;

    private bool rising = true;
    private Vector3 baseLocalPosition;
    private void Awake()
    {
        baseLocalPosition = transform.localPosition;
    }
    private void Update()
    {
        if (!rising) return;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
    }
    public void SetRising(bool value) => rising = value;

    public void ResetLevel()
    {
        transform.localPosition = baseLocalPosition;
    }
    public void Init(Vector3 newBaseWorldPosition, float? newRiseSpeed = null)
    {
        transform.position = newBaseWorldPosition;
        baseLocalPosition = transform.localPosition;
        if (newRiseSpeed.HasValue) riseSpeed = newRiseSpeed.Value;
        rising = false;
    }
}