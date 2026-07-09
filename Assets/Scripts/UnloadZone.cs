using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UnloadZone : MonoBehaviour
{
    [SerializeField] UIElement ui;
    [SerializeField] float loadTime = 0.1f;
    [SerializeField] int maxCount = 30;
    [SerializeField] Transform target;
    [SerializeField] ParticleSystem poof;
    [SerializeField] Transform root;
    [SerializeField] int startCount;
    [SerializeField] GameObject coinPrefab;

    [Header("SFX")]
    [SerializeField] AudioType sfxDropCoins;

    private BalkCollector balkCollector;
    private Furnace furnace;
    private int balkCount;
    private List<GameObject> balks = new List<GameObject>();
    private bool isUplolading;
    private int visualCount;

    void Start()
    {
        visualCount = root.childCount;
        UpdateUI(startCount);
        UpdateVisualBarks(startCount);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("UnloadZone " + other.name + " | " + other.tag, other.gameObject);
        if (isUplolading || balkCount >= maxCount) return;

        if (other.CompareTag("Player"))
        {
            balkCollector = other.GetComponent<BalkCollector>();
            InvokeRepeating("StartProcess", 0, loadTime);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Cancel();
    }

    void StartProcess()
    {
        if (balkCount == maxCount)
        {
            GiveBonus();
            Cancel();
            return;
        }

        GameObject balk = balkCollector.GetBalk();
        if (balk != null)
        {
            balkCount++;
            ui.SetCount(balkCount, maxCount);
            balk.transform.SetParent(null);
            balk.SetActive(true);

            //float delta = 0.4f/maxCount;

            DOTween.Sequence()
                .Append(balk.transform.DOMove(target.position, 0.3f))
                .Join(balk.transform.DOScale(0.8f, 0.3f))
                .OnComplete(() =>
                {
                    balks.Add(balk);
                    balk.SetActive(false);
                    //TODO
                    //progress.localPosition = new Vector3(0, -0.4f + delta * balkCount, 0);
                    UpdateVisualBarks(balkCount);
                });
        }
        else
        {
            Cancel();
        }
    }

    [ContextMenu("GiveBonus")]
    void GiveBonus()
    {
        AudioManager.PlayAt(sfxDropCoins, transform.position);
        poof.Play();
        //Events.GetCoins?.Invoke(transform.position);
        GiveBonus(transform.position);
    }

    void UpdateUI(int count)
    {
        ui.SetCount(count, maxCount);
    }

    void Cancel()
    {
        balkCollector = null;
        CancelInvoke("StartProcess");
    }

    public void Unload(Furnace furnace)
    {
        this.furnace = furnace;
        isUplolading = true;
        InvokeRepeating("UnloadProcess", 0, loadTime);
    }

    public void CancelUnload()
    {
        CancelInvoke();
        ui.gameObject.SetActive(false);
    }

    void UnloadProcess()
    {
        if (balkCount <= 0)
        {
            furnace.Continue();
            CancelUnload();
            return;
        }

        balkCount--;
        ui.SetCount(balkCount, maxCount);

        GameObject balk = GetBalk();
        if (balk != null)
        {
            //float delta = 0.4f / maxCount;

            DOTween.Sequence()
                .Append(balk.transform.DOMove(furnace.transform.position, 0.3f))
                .Join(balk.transform.DOScale(0.8f, 0.3f))
                .OnComplete(() =>
                {
                    furnace.AddBalk(this);
                    Destroy(balk.gameObject);
                    //TODO
                    //progress.localPosition = new Vector3(0, -0.4f + delta * balkCount, 0);
                    UpdateVisualBarks(balkCount);
                });
        }
    }

    void UpdateVisualBarks(int count)
    {
        // Рассчитываем сколько бревен должно отображаться визуально
        int visualBarkCount = Mathf.RoundToInt((float)count / maxCount * visualCount);
        visualBarkCount = Mathf.Clamp(visualBarkCount, 0, visualCount);

        // Включаем/выключаем дочерние объекты
        for (int i = 0; i < root.childCount; i++)
        {
            root.GetChild(i).gameObject.SetActive(i < visualBarkCount);
        }
    }

    public GameObject GetBalk()
    {
        if (balks.Count > 0)
        {
            GameObject balk = balks[0];
            balks.RemoveAt(0);
            balk.SetActive(true);
            return balk;
        }
        return null;
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
            GameObject obj = Instantiate(coinPrefab, pos, Quaternion.identity);
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

}
