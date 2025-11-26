using UnityEngine;
using UnityEngine.UI;

public class StarterChoiceUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelRoot;      // full-screen panel blocking interaction
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;

    [Header("Starter Options")]
    public MapPartyMemberDefinition starter1;
    public MapPartyMemberDefinition starter2;
    public MapPartyMemberDefinition starter3;

    // Optional: references to things we want to disable during the choice
    public ClickManager clickManager;
    public CampUIManager campUI;

    void Start()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null)
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            return;
        }

        // If we already picked a starter before, do nothing.
        if (transfer.starterChoiceCompleted)
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            return;
        }

        // Find systems to disable while the question is up

        if (clickManager != null) clickManager.enabled = false;
        if (campUI != null && campUI.campButton != null)
            campUI.campButton.interactable = false;

        if (panelRoot != null) panelRoot.SetActive(true);

        option1Button.onClick.AddListener(() => OnStarterChosen(starter1));
        option2Button.onClick.AddListener(() => OnStarterChosen(starter2));
        option3Button.onClick.AddListener(() => OnStarterChosen(starter3));
    }

    void OnStarterChosen(MapPartyMemberDefinition chosen)
    {
        if (chosen == null) return;
        var transfer = MapCombatTransfer.Instance;
        if (transfer != null)
        {
            // Start party with only this chosen character.
            transfer.SetupParty(new[] { chosen });
            transfer.starterChoiceCompleted = true;
        }

        // Re-enable map interaction
        if (clickManager != null) clickManager.enabled = true;
        if (campUI != null && campUI.campButton != null)
            campUI.campButton.interactable = true;

        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
