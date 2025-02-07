using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float speed = 0.5f; 
    public float scaleSpeed = 0.5f; 

    private Vector3 initialPosition;
    private Vector3 initialScale;

    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    void Update()
    {
       
        transform.position = initialPosition + new Vector3(Mathf.Sin(Time.time * speed), Mathf.Cos(Time.time * speed)) * 0.1f;

        
        transform.localScale = initialScale + new Vector3(Mathf.Sin(Time.time * scaleSpeed), Mathf.Sin(Time.time * scaleSpeed)) * 0.05f;
    }
}
