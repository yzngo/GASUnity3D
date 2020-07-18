using TMPro;

public class GameplayTagStatusBarButton : GenericUIIcon {
    public TextMeshProUGUI TextMeshPro;

    public void SetStacks(int stacks) => 
        TextMeshPro.text = stacks > 1 ? stacks.ToString() : "";
}