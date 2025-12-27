using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CampEditorScreenController : MonoBehaviour
{
    
    bool startHidden = true;
    [Header("Top bar")]
    public Transform characterSelectParent;        // container under EditorButtons/CharacterSelect
    public GameObject characterSelectButtonPrefab; // CharacterSelectButtonPrefab

    [Header("Stat screen")]
    public GameObject statScreenRoot;              // CampEditorScreen/StatScreen/Panel (or parent)
    public FullStatScreenUI statScreenUI;          // CampEditorScreen/StatScreen/Panel/FullStatScreenUI
    public ScrollRect statScreenScrollRect;        // CampEditorScreen/StatScreen/StatScreenScrollbar        // CampEditorScreen/StatScreen/CharacterName

    [Header("Skill screen")]
    public GameObject skillScreenRoot;             // CampEditorScreen/SkillScreen/Panel (or parent)
    public CampSkillListUI skillListUI;
    public ScrollRect skillScreenScrollRect;       // CampEditorScreen/SkillScreen/SkillScreenScrollbar

    [Header("Inventory screen")]
    public GameObject inventoryScreenRoot;
    public CampInventoryUI inventoryListUI;
    public ScrollRect inventoryScreenScrollRect;

    [Header("Talent screen")]
    public TalentTreeHost talentScreenRoot;            // CampEditorScreen/TalentScreen/Panel (or parent)

    [Header("Character Info")]
    public TMP_Text characterNameLabel;    
    public TMP_Text characterLevelLabel;
    public Slider healthBar;
    public Slider xpBar;
    public Image heldItemIcon;

    [Header("LevelUp Stats Screen")]
    public GameObject levelUpStatsScreenRoot;
    

    //Option buttons
    [Header("Option Buttons")]
    public Button statScreenButton;
    public Button skillScreenButton;
    public Button inventoryScreenButton;
    public Button talentScreenButton;
    public Button levelUpButton;

    public CanvasManager canvasManager;

    // current selection
    private MapPartyMemberDefinition current;

    private enum CurrentScreen {
        Stat,
        Skill,
        Inventory,
        Talent,
        LevelUp
    };

    private CurrentScreen currentScreen = CurrentScreen.Stat;

    void Awake()
    {
        if (startHidden) gameObject.SetActive(false);
        if (statScreenButton != null)
            statScreenButton.onClick.AddListener(() => 
            {
                OpenStatScreen();
                skillScreenRoot.SetActive(false);
                inventoryScreenRoot.SetActive(false);
                talentScreenRoot.gameObject.SetActive(false);
            });
        if (skillScreenButton != null)
            skillScreenButton.onClick.AddListener(() => 
            {
                OpenSkillScreen();
                statScreenRoot.SetActive(false);
                inventoryScreenRoot.SetActive(false);
                talentScreenRoot.gameObject.SetActive(false);
            });
        if( inventoryScreenButton != null)
            inventoryScreenButton.onClick.AddListener(() => 
            {
                OpenInventoryScreen();
                statScreenRoot.SetActive(false);
                skillScreenRoot.SetActive(false);
                talentScreenRoot.gameObject.SetActive(false);
            });
        if (talentScreenButton != null)
            talentScreenButton.onClick.AddListener(() => 
            {
                OpenTalentScreen();
                statScreenRoot.SetActive(false);
                skillScreenRoot.SetActive(false);
                inventoryScreenRoot.SetActive(false);
            });
        if(levelUpButton != null)
            levelUpButton.onClick.AddListener(() => 
            {
                if(current != null)
                {
                    current.tryToLevelUp();
                    Refresh();
                }
            });
    }

    public void Open()
    {
        gameObject.SetActive(true);
        BuildCharacterButtons();
        // default to first member if none selected
        if (current == null)
        {
            var camp = MapCombatTransfer.Instance != null ? MapCombatTransfer.Instance.camp : null; // :contentReference[oaicite:0]{index=0}
            if (camp != null && camp.Count > 0) SelectCharacter(camp[0]);
        }

        // bring up stat screen + position scrollbar at top (1)
        OpenStatScreen();
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        canvasManager.RefreshUI();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if(InputSystem.actions.FindAction("exit").WasPressedThisFrame())
        {
            if(currentScreen == CurrentScreen.Talent)
            {
                // close talent screen first
                currentScreen = CurrentScreen.Stat;
                if (talentScreenRoot != null) talentScreenRoot.gameObject.SetActive(false);
                OpenStatScreen();
            }
            else
            {
                Close();
                canvasManager.RefreshUI();
            }
        }
        
    }

    void BuildCharacterButtons()
    {
        // clear old
        for (int i = characterSelectParent.childCount - 1; i >= 0; i--)
            Destroy(characterSelectParent.GetChild(i).gameObject);

        var camp = MapCombatTransfer.Instance != null ? MapCombatTransfer.Instance.camp : null; // :contentReference[oaicite:1]{index=1}
        if (camp == null) return;

        foreach (var def in camp)
        {
            if (def == null) continue;
            var go = Instantiate(characterSelectButtonPrefab, characterSelectParent);
            var btn = go.GetComponent<Button>();
            var text = go.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                var name = !string.IsNullOrEmpty(def.displayName)
                    ? def.displayName
                    : (def.characterPrefab != null ? def.characterPrefab.name : "Unnamed");
                text.text = name;
            }
            btn.onClick.AddListener(() => SelectCharacter(def));
        }
    }

    void SelectCharacter(MapPartyMemberDefinition def)
    {
        current = def;
        if (characterNameLabel != null)
        {
            var name = (def != null && !string.IsNullOrEmpty(def.displayName))
                ? def.displayName
                : (def != null && def.characterPrefab != null ? def.characterPrefab.name : "Character");
            characterNameLabel.text = name;
        }
        // stat screen
        OpenStatScreen();
        // skill screen
        Refresh();
    }
    

    void OpenStatScreen()
    {
        currentScreen = CurrentScreen.Stat;
        if (statScreenRoot != null) statScreenRoot.SetActive(true);
        if (statScreenScrollRect != null) statScreenScrollRect.verticalNormalizedPosition = 1f;
        if (statScreenUI != null && current != null) statScreenUI.UpdateStats(current.stats);
    }
    void OpenSkillScreen()
    {
        currentScreen = CurrentScreen.Skill;
        if (skillScreenRoot != null) skillScreenRoot.SetActive(true);
        if (skillScreenScrollRect != null) skillScreenScrollRect.verticalNormalizedPosition = 1f;
        if (skillListUI != null && current != null) skillListUI.UpdateSkillList(current.skills);
    }
    void OpenInventoryScreen()
    {
        currentScreen = CurrentScreen.Inventory;
        if (inventoryScreenRoot != null) inventoryScreenRoot.SetActive(true);
        if (inventoryScreenScrollRect != null) inventoryScreenScrollRect.verticalNormalizedPosition = 1f;
        if (inventoryListUI != null && current != null) inventoryListUI.SetCharacter(current);
    }
    void OpenTalentScreen()
    {
        currentScreen = CurrentScreen.Talent;
        if (talentScreenRoot != null) talentScreenRoot.gameObject.SetActive(true);
        talentScreenRoot.ShowFor(current);
    }

    void Refresh(){
        if(current == null) return;
        
        RefreshCharacterPortrait();

        if(statScreenRoot != null && currentScreen != CurrentScreen.Stat)
            statScreenRoot.SetActive(false);
        if(skillScreenRoot != null && currentScreen != CurrentScreen.Skill) 
            skillScreenRoot.SetActive(false);
        if(inventoryScreenRoot != null && currentScreen != CurrentScreen.Inventory)
            inventoryScreenRoot.SetActive(false);
        if(talentScreenRoot != null && currentScreen != CurrentScreen.Talent) 
            talentScreenRoot.gameObject.SetActive(false);
        // Refresh current screen
        if(currentScreen == CurrentScreen.Stat)
        {
            OpenStatScreen();
        }
        else if(currentScreen == CurrentScreen.Skill)
        {
            OpenSkillScreen();
        }
        else if(currentScreen == CurrentScreen.Inventory)
        {
            OpenInventoryScreen();
        }
        else if(currentScreen == CurrentScreen.Talent)
        {
            OpenTalentScreen();
        }
    }

    public void RefreshCharacterPortrait()
    {
        //Set character name
        if (characterNameLabel != null)
        {
            var name = (current != null && !string.IsNullOrEmpty(current.displayName))
                ? current.displayName
                : (current != null && current.characterPrefab != null ? current.characterPrefab.name : "Character");
            characterNameLabel.text = name;
        }
        //Update health bar
        if (healthBar != null)
        {
            var currentHealth = current.health;
            var maxHealth = current.stats.maxHealth;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        //Update xp bar and level up button
        if (xpBar != null)
        {
            var currentXp = current.currentXp;
            var nextLevelXp = current.GetXpRequiredForNextLevel(current.level);
            xpBar.maxValue = nextLevelXp;
            xpBar.value = Mathf.Min(currentXp, nextLevelXp);

            
            //Show level up button if character can level up
            if(levelUpButton.gameObject != null)
            {
                levelUpButton.gameObject.SetActive(currentXp >= nextLevelXp);
            }
        }

        

        //Update character level
        if(characterLevelLabel != null)
        {
            characterLevelLabel.text = $"Lv. {current.level}";
        }

        //Update held item icon
        if (heldItemIcon != null)
        {
            var Transfer = MapCombatTransfer.Instance;
            ItemDefinition heldItemDef = Transfer.GetEquippedItem(current);
            if (heldItemDef != null && heldItemDef.icon != null)
            {
                heldItemIcon.sprite = heldItemDef.icon;
                heldItemIcon.enabled = true;
            }
            else
            {
                heldItemIcon.enabled = false;
            }
        }

        
    }
}
