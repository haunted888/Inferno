using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceStatusUI : MonoBehaviour
{
    public BattleCharacter target;
    public TMP_Text nameText;
    public Slider hpSlider;
    public TMP_Text hpText;
    public Vector3 worldOffset = new Vector3(0, 2f, 0);

    private Camera cam;

    public void Initialize(BattleCharacter chr)
    {
        target = chr;
        cam = Camera.main;

        if (target != null && hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = target.MaxHealth;
            hpSlider.value = target.CurrentHealth;
        }

        UpdateTexts();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (hpSlider != null)
            hpSlider.value = target.CurrentHealth;

        UpdateTexts();

        if (cam != null)
        {
            // Position above target
            transform.position = target.transform.position + worldOffset;
            // Face camera (optional, for readability)
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }
    }

    private void UpdateTexts()
    {
        if (nameText != null && target != null)
            nameText.text = target.name;
        if (hpText != null && target != null)
            hpText.text = $"{target.CurrentHealth}/{target.MaxHealth}";
    }
}
