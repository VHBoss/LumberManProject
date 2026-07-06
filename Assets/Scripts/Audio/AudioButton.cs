using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioButton : Button
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        //AudioManager.Instance.Click();
    }
}
