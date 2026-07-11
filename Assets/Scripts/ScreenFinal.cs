using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFinal : MonoBehaviour
{
    [SerializeField] Image fade;
    [SerializeField] Button buttonReplay;

    void Start()
    {
        fade.color = Color.black;
        CanvasGroup img = buttonReplay.GetComponent<CanvasGroup>();
        buttonReplay.onClick.AddListener(Replay);
        DOTween.Sequence()
            .Append(fade.DOFade(0, 2f))
            .Append(img.DOFade(1, 1f)).OnComplete(() => img.interactable = true);
    }

    void Replay()
    {
        buttonReplay.interactable = false;
        fade.DOFade(1, 1f).OnComplete(() => SceneManager.LoadScene(0));        
    }
}
