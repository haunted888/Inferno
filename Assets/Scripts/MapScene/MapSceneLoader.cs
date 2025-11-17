using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSceneLoader : MonoBehaviour
{
    public static MapSceneLoader Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("Scenes/Limbo Battle");
    }
}
