using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIManager : MonoBehaviour
{
    [Header("Dialogue Screen (for party member rewards)")]
    public GameObject dialoguePanelRoot; // full-screen panel
    public Button dialogueClickCatcher;  // button covering entire dialogue panel
    public TMP_Text dialogueText;        // text inside dialogue panel

    [Header("Summary Screen (shows all rewards as text)")]
    public GameObject summaryPanelRoot;  // full-screen panel
    public Button summaryClickCatcher;   // button covering entire summary panel
    public TMP_Text summaryText;         // text inside summary panel

    private List<MapRewardDefinition> currentRewards = new List<MapRewardDefinition>();
    private bool hasPartyMemberRewards;

    public ClickManager clickManager;
    public CampUIManager campUI;

    void Start()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null)
        {
            HideAll();
            return;
        }

        // Get rewards and early out if none or if player did not win
        currentRewards = transfer.GetPendingRewards();
        if (!transfer.lastBattlePlayerWon || currentRewards == null || currentRewards.Count == 0)
        {
            HideAll();
            return;
        }

        hasPartyMemberRewards = currentRewards.Any(r => r != null && r.type == RewardCategory.PartyMember);


        if (clickManager != null) clickManager.enabled = false;
        if (campUI != null && campUI.campButton != null)
            campUI.campButton.interactable = false;

        // Wire clicks
        if (dialogueClickCatcher != null)
            dialogueClickCatcher.onClick.AddListener(OnDialogueClicked);

        if (summaryClickCatcher != null)
            summaryClickCatcher.onClick.AddListener(OnSummaryClicked);

        // If there are party member rewards, show dialogue first; else skip straight to summary
        if (hasPartyMemberRewards)
        {
            ShowDialogue();
        }
        else
        {
            ApplyRewards();  // nothing party-related for now, but keeps logic centralized
            ShowSummary();
        }
    }

    void ShowDialogue()
    {
        if (dialoguePanelRoot != null)
            dialoguePanelRoot.SetActive(true);

        if (summaryPanelRoot != null)
            summaryPanelRoot.SetActive(false);

        if (dialogueText != null)
        {
            var names = currentRewards
                .Where(r => r != null && r.type == RewardCategory.PartyMember && r.partyMember != null)
                .Select(r => string.IsNullOrEmpty(r.partyMember.displayName)
                    ? "a new ally"
                    : r.partyMember.displayName);

            string joined = string.Join(", ", names);
            if (string.IsNullOrEmpty(joined))
                joined = "You have gained a new ally.";

            dialogueText.text = $"You have gained: {joined}";
        }
    }

    void ShowSummary()
    {
        if (dialoguePanelRoot != null)
            dialoguePanelRoot.SetActive(false);

        if (summaryPanelRoot != null)
            summaryPanelRoot.SetActive(true);

        if (summaryText != null)
        {
            // For now, just party member names as text
            var lines = new List<string>();

            foreach (var r in currentRewards)
            {
                if (r == null) continue;

                if (r.type == RewardCategory.PartyMember && r.partyMember != null)
                {
                    string name = string.IsNullOrEmpty(r.partyMember.displayName)
                        ? "Unnamed ally"
                        : r.partyMember.displayName;
                    lines.Add($"Party Member: {name}");
                }
                else if (r.type == RewardCategory.Money)
                {
                    lines.Add($"Money: {r.moneyAmount}");
                }
                else if (r.type == RewardCategory.Item && r.item != null)
                {
                    string name = string.IsNullOrEmpty(r.item.displayName) ? "Item" : r.item.displayName;
                    lines.Add($"Item: {name} x{Mathf.Max(1, r.itemQuantity)}");
                }

            }

            if (lines.Count == 0)
                lines.Add("No rewards.");

            summaryText.text = string.Join("\n", lines);
        }
    }

    void OnDialogueClicked()
    {
        // When the dialogue is closed, actually grant the party member rewards
        ApplyRewards();
        ShowSummary();
    }

    void OnSummaryClicked()
    {
        // Done with rewards; clear and re-enable map interaction
        var transfer = MapCombatTransfer.Instance;
        if (transfer != null)
        {
            transfer.ClearPendingRewards();
        }

        HideAll();
        RestoreMapInteraction();
    }

    void ApplyRewards()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        foreach (var r in currentRewards)
        {
            if (r == null) continue;

            if (r.type == RewardCategory.PartyMember && r.partyMember != null)
            {
                transfer.AddPartyMember(r.partyMember, addToPartyIfSpace: true);
            }
            else if (r.type == RewardCategory.Item && r.item != null)
            {
                int qty = Mathf.Max(1, r.itemQuantity);
                transfer.AddItem(r.item, qty);
            }
            // Money: ignore for now
        }
    }

    void HideAll()
    {
        if (dialoguePanelRoot != null)
            dialoguePanelRoot.SetActive(false);
        if (summaryPanelRoot != null)
            summaryPanelRoot.SetActive(false);
    }

    void RestoreMapInteraction()
    {
        if (clickManager != null)
            clickManager.enabled = true;

        if (campUI != null && campUI.campButton != null)
            campUI.campButton.interactable = true;
    }
}
