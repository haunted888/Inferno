using TMPro;
using UnityEngine;

public class SkillNamePrefab : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text skillNameText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSkillName(string text)
    {
        skillNameText.text = text;
    }
}
