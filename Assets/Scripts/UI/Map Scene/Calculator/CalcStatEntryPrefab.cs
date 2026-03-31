using TMPro;
using UnityEngine;

public class CalcStatEntryUI : MonoBehaviour
{
    public TMP_Text labelText;
    public TMP_InputField inputField;

    private int lastValidValue;

    public void Setup(string label, int value, System.Action<int> onChanged)
    {
        if (labelText != null)
            labelText.text = label;

        lastValidValue = value;

        if (inputField != null)
        {
            inputField.onEndEdit.RemoveAllListeners();
            inputField.SetTextWithoutNotify(value.ToString());

            inputField.onEndEdit.AddListener(text =>
            {
                if (int.TryParse(text, out int parsed))
                {
                    lastValidValue = parsed;
                    onChanged?.Invoke(parsed);
                }
                else
                {
                    // revert to last valid value
                    inputField.SetTextWithoutNotify(lastValidValue.ToString());
                }
            });
        }
    }
}