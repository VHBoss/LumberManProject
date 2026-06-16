using DG.Tweening;
using System.Collections.Generic;
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
    [SerializeField] Bounds bounds;
    [SerializeField] LayerMask raycastMask;
    [SerializeField] Vector3 extends = Vector3.one * 0.5f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 1f;

    Collider[] results;

    void Start()
    {
        results = new Collider[5];
        for (int i = 0; i < vegetables.Length; i++)
        {
            Create(vegetables[i]);
        }

        Events.GetCoins += GiveBonus;
    }

    void OnDestroy()
    {
        Events.GetCoins -= GiveBonus;
    }

    void Create(Vegetable vegetable)
    {
        //Vector3 extends = Vector3.one * density;
        for (int i = 0; i < vegetable.count; i++)
        {
            // 1. Создаем объект в случайной позиции
            float randomX = Random.value * bounds.size.x;
            float randomZ = Random.value * bounds.size.z;
            Vector3 randomPos = new Vector3(transform.position.x + randomX - bounds.extents.x, bounds.extents.y, transform.position.z + randomZ - bounds.extents.z);
            //if (Physics.Raycast(randomPos, -Vector3.up * bounds.extents.y, out RaycastHit hit))
            //{
            //    randomPos.y = hit.point.y;
            //}
            //else
            //{
            //    randomPos.y = 0;
            //}

            Debug.DrawRay(randomPos, -Vector3.up, Color.yellow, 10);

            if (Physics.OverlapBoxNonAlloc(randomPos, extends, results, Quaternion.identity, raycastMask) > 0) continue;

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

    void GiveBonus(Vector3 pos)
    {
        int random = Random.Range(2, 8);
        List<int> availableDirections = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            availableDirections.Add(i);
        }

        for (int i = 0; i < random; i++)
        {
            // Если направления закончились, выходим
            if (availableDirections.Count == 0) break;

            // Выбираем случайное направление из доступных
            int dirIndex = Random.Range(0, availableDirections.Count);
            int direction = availableDirections[dirIndex];
            availableDirections.RemoveAt(dirIndex);

            // Создаем объект
            GameObject obj = Instantiate(vegetables[3].prefab, pos, Quaternion.identity);
            obj.GetComponent<Collider>().enabled = false;

            // Запускаем анимацию полета по дуге
            ThrowToGround(obj, direction, pos);
        }
    }

    void ThrowToGround(GameObject obj, int direction, Vector3 startPos)
    {
        // 8 направлений: 4 по сторонам куба, 4 по углам
        Vector3[] directions = new Vector3[]
        {
        new Vector3(1, 0, 0),   // Право (0°)
        new Vector3(0, 0, 1),   // Вперед (90°)
        new Vector3(-1, 0, 0),  // Лево (180°)
        new Vector3(0, 0, -1),  // Назад (270°)
        new Vector3(1, 0, 1),   // Право-вперед (45°)
        new Vector3(-1, 0, 1),  // Лево-вперед (135°)
        new Vector3(-1, 0, -1), // Лево-назад (225°)
        new Vector3(1, 0, -1)   // Право-назад (315°)
        };

        // Вычисляем целевую позицию на земле
        Vector3 targetPos = startPos + directions[direction].normalized;
        targetPos.y = 0; // Устанавливаем на уровень земли (Y = 0)

        // Параметры полета
        float duration = 0.8f;
        float height = 0.5f;

        // Сохраняем начальную позицию для расчета дуги
        Vector3 arcStartPos = startPos;

        // Используем DOVirtual для кастомной траектории
        float t = 0;
        DOTween.To(() => t, x => t = x, 1, duration)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                // Линейная интерполяция между точками
                Vector3 currentPos = Vector3.Lerp(arcStartPos, targetPos, t);

                // Параболическая высота (дуга)
                float parabolaHeight = 4 * height * t * (1 - t);
                currentPos.y = startPos.y + parabolaHeight;

                obj.transform.position = currentPos;
            })
            .OnComplete(() =>
            {
                obj.GetComponent<Collider>().enabled = true;

                // Фиксируем конечную позицию на земле
                //obj.transform.position = targetPos;

                // Опционально: добавить эффект приземления (партиклы, звук)
                // OnLandingEffect(obj.transform.position);
            });
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, bounds.size);
    }
}