using UnityEngine;
using UnityEngine.UI;

public class GenericUIIcon : MonoBehaviour {
    public Image ImageIcon;
    public Image CooldownOverlay;
    

    public void SetCooldownRemainingPercent(float percentRemaining) {
        CooldownOverlay.fillAmount = Mathf.Clamp01(1- percentRemaining);
    }

}
