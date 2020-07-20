using TMPro;

public class EffectTile : BaseTile 
{
    public TextMeshProUGUI stackText;

    public void SetStacks(int stacks) => 
        stackText.text = stacks > 1 ? stacks.ToString() : "";
}