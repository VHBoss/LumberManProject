using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private PlayerController m_Player;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("AXE: " + other.name, other);
        if (other.CompareTag("Tree"))
        {
            var tree = other.GetComponent<TreeObject>();
            tree.Chop(transform, m_Player);
        }
    }
}
