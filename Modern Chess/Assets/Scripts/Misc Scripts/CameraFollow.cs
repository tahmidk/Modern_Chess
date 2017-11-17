using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum Behavior { INSTANT, FOLLOW }
    public static CameraFollow Instance { get; set; }

    public Transform target;
    public Behavior mode;
    public float speed = 10f;
    public Vector3 offset;

    private void Start()
    {
        Instance = this;
        offset = new Vector3(-8, 10, -11);
    }

    void LateUpdate ()
    {
        // If not at the destination, move towards the destination
        if (target)
        {
            Vector3 destPosition = target.position + offset;

            if (this.transform.position != destPosition)
            {
                Vector3 smoothedPosition = Vector3.Lerp(this.transform.position, destPosition, speed);
                transform.position = smoothedPosition;
            }

            if (mode == Behavior.INSTANT && this.transform.position == destPosition)
                target = null;
        }

        
	}
}
