using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatPreviewScreen : MonoBehaviour
{
    public Transform enemyPreviewContainer;
    public EnemyPreviewPrefab enemyPreviewPrefab;
    public MapEnemyDefinition[] storedEnemies; // For testing purposes, can be removed later
    public CalculatorScreen calculatorScreen;
    public Button closeButton;
    public Button calculatorButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        ClickManager.OnNodeRightClicked.AddListener(HandleNodeRightClicked);
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        calculatorButton.onClick.AddListener(openCalculator);
        gameObject.SetActive(false); // Start hidden
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleNodeRightClicked(MapEnemyDefinition[] enemies)
    {
        storedEnemies = enemies; // For testing, can be removed later
        gameObject.SetActive(true);
        // Clear existing previews
        foreach (Transform child in enemyPreviewContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new previews for each enemy
        foreach (var enemyDef in enemies)
        {
            var previewInstance = Instantiate(enemyPreviewPrefab, enemyPreviewContainer);
            previewInstance.Setup(enemyDef);
        }
    }

    private void openCalculator()
    {
        calculatorScreen.gameObject.SetActive(true);
    }
}