using UnityEngine;

public class AutoBalance : MonoBehaviour
{
    [SerializeField] private float Kp;
    [SerializeField] private float Ki;
    [SerializeField] private float Kd;
    private Rigidbody2D rb;

    private float integral;
    private float previousError;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float targetAngle = 0f;

        float currentAngle = transform.eulerAngles.z;
        float error = Mathf.DeltaAngle(currentAngle, targetAngle);

        float derivative = (error - previousError) / Time.fixedDeltaTime;
        integral += error * Time.fixedDeltaTime;

        float torque = Kp * error + Ki * integral + Kd * derivative;

        rb.AddTorque(torque);

        previousError = error;

        //rb.AddTorque(power * Vector3.SignedAngle(Vector3.up, transform.rotation * Vector3.forward, Vector3.back);
    }
}
