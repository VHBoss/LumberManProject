using UnityEngine;

public class FurnaceEyes : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform eyeLeft;
    [SerializeField] Transform eyeRight;
    [SerializeField] float rangeX = 0.001f;
    [SerializeField] float rangeY = 0.001f;
    [SerializeField] float minDistance = 1f;   // когда эффект максимальный
    [SerializeField] float maxDistance = 10f;  // когда эффекта почти нет
    [SerializeField] float yOffsetStrength = 0.01f;

    public Vector3 offsetLeft;
    public Vector3 offsetRight;

    private Vector3 leftDefaultLocalPos;
    private Vector3 rightDefaultLocalPos;

    void Start()
    {
        leftDefaultLocalPos = eyeLeft.localPosition;
        rightDefaultLocalPos = eyeRight.localPosition;
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);

        Vector3 pos = leftDefaultLocalPos;

        float xOffset = target.position.z * rangeX * t;

        pos.x += xOffset;
        pos.y -= t * yOffsetStrength;

        eyeLeft.localPosition = pos;

        pos = rightDefaultLocalPos;

        xOffset = target.position.z * rangeX * t;

        pos.x += xOffset;
        pos.y -= t * yOffsetStrength;

        eyeRight.localPosition = pos;
    }
}
