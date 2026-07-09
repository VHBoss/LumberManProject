using UnityEngine;

[System.Serializable]
public class Vegetable
{
    public GameObject prefab;
    public int count;
    public bool randomizeColor;
}

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] Vegetable[] vegetables;
    [SerializeField] Vegetable coins;
    [SerializeField] Bounds bounds;
    [SerializeField] LayerMask raycastMask;
    [SerializeField] Vector3 extends = Vector3.one * 0.5f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 1f;

    Collider[] results;

    void Start()
    {
        results = new Collider[1];

        for (int i = 0; i < vegetables.Length; i++)
        {
            var vegetable = vegetables[i];
            for (int j = 0; j < vegetable.count; j++)
            {
                CreateVegetable(vegetable);
            }
        }

        CreateVegetable(coins);
    }

    void CreateVegetable(Vegetable vegetable)
    {
        // 1. Создаем объект в случайной позиции
        float randomX = Random.value * bounds.size.x;
        float randomZ = Random.value * bounds.size.z;
        Vector3 randomPos = new Vector3(transform.position.x + randomX - bounds.extents.x, bounds.extents.y, transform.position.z + randomZ - bounds.extents.z);

        Debug.DrawRay(randomPos, -Vector3.up, Color.yellow, 10);

        if (Physics.OverlapBoxNonAlloc(randomPos, extends, results, Quaternion.identity, raycastMask) > 0) return;

        randomPos.y = 0;
        GameObject tree = Instantiate(vegetable.prefab, randomPos, Quaternion.identity);

        float randomScale = Random.Range(minScale, maxScale);
        Vector3 scaleVec = new Vector3(randomScale, randomScale, randomScale);
        tree.transform.localScale = scaleVec;

        RotateRandom(tree);

        if (vegetable.randomizeColor)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();

            Color randomColor = new Color(
                Random.Range(0.7f, 1.0f), // R - Красный
                Random.Range(0.8f, 1.0f), // G - Зеленый (делаем его ярче)
                Random.Range(0.5f, 0.8f), // B - Синий
                1f
            );

            props.SetColor("_Color", randomColor); // Стандартный URP Lit Shader

            // Получаем рендерер и применяем блок
            Renderer rend = tree.transform.GetChild(1).GetComponent<Renderer>();
            rend.SetPropertyBlock(props);
        }

        tree.transform.SetParent(transform);
    }

    void RotateRandom(GameObject obj)
    {
        float angle = Random.Range(-20, 20);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 dir = Vector3.RotateTowards(Vector3.up, Vector3.forward, rad, 0);
        obj.transform.Rotate(dir * 20, Space.Self);
        float randomAngle = Random.Range(0, 360);
        var euler = obj.transform.localEulerAngles;
        euler.y = randomAngle;
        obj.transform.localEulerAngles = euler;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, bounds.size);
    }
}