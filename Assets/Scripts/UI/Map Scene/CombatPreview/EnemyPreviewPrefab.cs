using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPreviewPrefab : MonoBehaviour
{
    public TMP_Text enemyNameText;


    public RectTransform ContentContainer;

    public SkillNamePrefab skillNamePrefab;
    public Transform skillListContainer;

    public Transform passiveListContainer;
    public Transform statsContainer;

    public Button skillListToggleButton;
    public Button passiveListToggleButton;
    public Button statsToggleButton;

    LayoutElement layoutElement;

    private float baseHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        baseHeight = ContentContainer.rect.height;
    }
    void Start()
    {

        skillListToggleButton.onClick.AddListener(() => SetActiveDisplays(!skillListContainer.gameObject.activeSelf, false, false));
        passiveListToggleButton.onClick.AddListener(() => SetActiveDisplays(false, !passiveListContainer.gameObject.activeSelf, false));
        statsToggleButton.onClick.AddListener(() => SetActiveDisplays(false, false, !statsContainer.gameObject.activeSelf));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(MapEnemyDefinition enemyDef)
    {
        // Set up the enemy preview based on the provided enemy definition
        // You can implement your logic here to display the enemy's information
        enemyNameText.text = enemyDef.displayName;

        foreach (var skill in enemyDef.skills)
        {
            var skillPrefabInstance = Instantiate(skillNamePrefab, skillListContainer);
            skillPrefabInstance.SetSkillName(skill.skillName);
        }

        SetActiveDisplays(false, false, false); // Start with all displays hidden
    }

    public void SetActiveDisplays(bool showSkills, bool showPassives, bool showStats)
    {
        // 1) Toggle first
        skillListContainer.gameObject.SetActive(showSkills);
        passiveListContainer.gameObject.SetActive(showPassives);
        statsContainer.gameObject.SetActive(showStats);

        // 2) Rebuild layout so rect.height values are correct this frame
        Canvas.ForceUpdateCanvases();

        float extra =
            showSkills   ? skillListContainer.GetComponent<RectTransform>().rect.height :
            showPassives ? passiveListContainer.GetComponent<RectTransform>().rect.height :
            showStats    ? statsContainer.GetComponent<RectTransform>().rect.height :
            0f;

        // 3) Drive size through layout
        layoutElement.preferredHeight = baseHeight + extra;
        // 4) Force parent layout to update so siblings get pushed
        var parent = transform.parent as RectTransform;
        if (parent) LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
    }
}
