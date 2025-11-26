// CameraShake.cs
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private float _shakeDuration = 0f;
    private float _shakeMagnitude = 0.1f; // 控制晃动幅度（初始较小）
    private float _shakeSpeed = 5f;       // 控制晃动频率

    private Vector3 _originalPos;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _originalPos = transform.localPosition;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeMagnitude *
                Mathf.Sin(_shakeSpeed * Time.time);
            _shakeDuration -= Time.deltaTime;
        }
        else
        {
            _shakeDuration = 0f;
            transform.localPosition = _originalPos;
        }
    }

    // 调用此方法触发晃动（例如出牌/上菜）
    public void TriggerShake(float duration = 0.3f, float magnitude = 0.15f, float speed = 8f)
    {
        // 参数调优建议：duration ≤ 0.4s, magnitude ≤ 0.2, 避免低频大幅晃动（易眩晕）
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
        _shakeSpeed = speed;
    }
}