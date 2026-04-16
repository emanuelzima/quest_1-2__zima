using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float platformSpeed;

    [SerializeField]
    private Vector3 start;

    [SerializeField]
    private Vector3 end;

    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    void Start()
    {
        this.previousPosition = this.transform.position;
    }

    void FixedUpdate()
    {
        float pingPong = Mathf.PingPong(Time.fixedTime * this.platformSpeed, 1.0f);
        var newPosition = Vector3.Lerp(this.start, this.end, pingPong);
        this.transform.localPosition = newPosition;

        Vector3 displacement = this.transform.position - this.previousPosition;
        this.currentVelocity = displacement / Time.fixedDeltaTime;

        this.previousPosition = this.transform.position;
    }

    public Vector3 GetVelocity()
    {
        return this.currentVelocity;
    }
}