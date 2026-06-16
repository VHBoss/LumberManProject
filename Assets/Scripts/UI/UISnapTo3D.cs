using UnityEngine;

public class UISnapTo3D : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 offset = Vector3.zero;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        gameObject.SetActive(target != null);
    }

    void Update()
    {
        if (target == null) return;

        Vector3 screenPoint = cam.WorldToScreenPoint(target.position + offset);
        transform.position = screenPoint;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        //if(target == null) transform.position = new Vector3 (9999, 0, 0);

        gameObject.SetActive(target != null);
    }
}
