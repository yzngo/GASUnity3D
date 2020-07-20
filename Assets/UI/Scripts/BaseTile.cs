using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class BaseTile : MonoBehaviour 
{
    [SerializeField] private Image icon = default;
    [SerializeField] private Image timeMask = default;
    
    public void SetRemainingPercent(float percentRemaining) 
    {
        timeMask.fillAmount = Mathf.Clamp01(percentRemaining);
    }

    public void SetSprite(Sprite sprite, Color color)
    {
        icon.sprite = sprite;
        icon.color = color;
    }
}
