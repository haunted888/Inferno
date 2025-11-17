using System;
using UnityEngine;
using UnityEngine.UI;

public enum BattleCommandType
{
    Skills,
    Items,
    Skip,
    Back
}


public class BattleCommandUI : MonoBehaviour
{
    public Button skillsButton;
    public Button itemsButton;
    public Button skipButton;
    public Button backButton;

    private Action<BattleCommandType> onCommandChosen;

    public void ShowForCharacter(BattleCharacter character, bool canGoBack, Action<BattleCommandType> callback)
    {
        gameObject.SetActive(true);
        onCommandChosen = callback;

        backButton.gameObject.SetActive(canGoBack);

        skillsButton.onClick.RemoveAllListeners();
        itemsButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();

        skillsButton.onClick.AddListener(() => OnCommand(BattleCommandType.Skills));
        itemsButton.onClick.AddListener(() => OnCommand(BattleCommandType.Items));
        skipButton.onClick.AddListener(() => OnCommand(BattleCommandType.Skip));
        backButton.onClick.AddListener(() => OnCommand(BattleCommandType.Back));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        onCommandChosen = null;
    }

    private void OnCommand(BattleCommandType type)
    {
        onCommandChosen?.Invoke(type);
    }
}
