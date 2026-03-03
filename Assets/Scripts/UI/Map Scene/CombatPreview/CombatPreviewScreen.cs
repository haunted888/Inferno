using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatPreviewScreen : MonoBehaviour
{
    public Transform enemyPreviewContainer;
    public EnemyPreviewPrefab enemyPreviewPrefab;
    public List<MapEnemyDefinition> storedEnemies;
    public CalculatorScreen calculatorScreen;
    public Button closeButton;
    public Button calculatorButton;
    // Awake is called when the script instance is being loaded
    void Awake()
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
        storedEnemies = new List<MapEnemyDefinition>(enemies);
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
        var Transfer = MapCombatTransfer.Instance;
        if (Transfer == null)
        {
            return;
        }
        calculatorScreen.Open(Transfer.party, storedEnemies);
    }
}