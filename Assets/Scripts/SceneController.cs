using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] Image fade;

    void Start()
    {
        Events.LevelExited += FadeOut;
    }

    void OnDestroy()
    {
        Events.LevelExited -= FadeOut;
    }

    void FadeOut()
    {
        fade.DOFade(1, 2f).OnComplete(() => { SceneManager.LoadScene(1); });
    }
}
