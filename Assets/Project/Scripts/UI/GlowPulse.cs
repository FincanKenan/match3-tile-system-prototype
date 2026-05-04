using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    private float _baseScale;
    private float _pulseSpeed;
    private float _pulseAmount;

    public void Init(float speed, float amount)
    {
        _pulseSpeed = speed;
        _pulseAmount = amount;
        _baseScale = transform.localScale.x;
    }

    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmount;
        float scale = _baseScale + pulse;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}