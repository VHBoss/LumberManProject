using System.Collections;
using UnityEngine;

public class TreeBalk : MonoBehaviour
{
    [SerializeField] TreeLog[] barks;
    [SerializeField] float forceX = 150;
    [SerializeField] float forceY = 20;
    [SerializeField] float breakPropability = 0.3f;
    [SerializeField] ParticleSystem poof;

    public void Split(float height, float burnAmount)
    {
        float countf = height / 0.2f;
        int count = Mathf.FloorToInt(countf);

        gameObject.SetActive(true);

        for (int i = 0; i < barks.Length; i++)
        {
            var item = barks[i];
            if (i < count)
            {
                item.Init(burnAmount, forceX, forceY);

                bool destroy = Random.Range(0f, 1f) < breakPropability;
                if (destroy)
                {
                    StartCoroutine(WaitAndDestroy(item.gameObject));
                }
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

        //Debug.Log(height + " | " + countf + " | " + count);
    }

    IEnumerator WaitAndDestroy(GameObject item)
    {
        yield return new WaitForSeconds(0.3f);

        if (item.GetComponent<BoxCollider>().enabled)//Если дерево ещё не подобрали
        {
            //poof.Play();
            GameObject.Instantiate(poof, item.transform.position, item.transform.rotation);
            //gameObject.SetActive(false);
            Destroy(item);
        }
    }
}
