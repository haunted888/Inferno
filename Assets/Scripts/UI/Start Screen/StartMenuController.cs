using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public Button newGameButton;

    void Start()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
    }

    void OnNewGameClicked()
    {
        if (MapCombatTransfer.Instance != null)
        {
            MapCombatTransfer.Instance.StartNewGame();
        }

        SceneManager.LoadScene("Scenes/Map Scene");
    }
}
