using UnityEngine;
using UnityEngine.UI;

public class BaseTile : MonoBehaviour 
{
    public Image ImageIcon;
    public Image CooldownOverlay;
    

    public void SetRemainingPercent(float percentRemaining) {
        CooldownOverlay.fillAmount = Mathf.Clamp01(percentRemaining);
    }

}
