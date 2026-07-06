using UnityEngine;

public class BurnSystem : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float offset;

    private MeshRenderer mr;
    private readonly int RadiusProperty = Shader.PropertyToID("_Radius");

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        float distance = offset - target.position.x;
        mr.material.SetFloat(RadiusProperty, distance);
    }
}
