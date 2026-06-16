using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 offset;

    private Vector3 pos;

    void LateUpdate()
    {
        if(m_Target == null) return;

        pos = offset;
        pos.x = m_Target.position.x + offset.x;
        transform.position = pos;
    }
}
