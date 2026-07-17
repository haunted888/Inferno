using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapNodeVisual : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Sprite sprite;

    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall -= DelayedApplyVisual;
        EditorApplication.delayCall += DelayedApplyVisual;
#endif
    }

#if UNITY_EDITOR
    private void DelayedApplyVisual()
    {
        if (this == null) return;
        ApplyVisual();
    }
#endif

    private void Start()
    {
        ApplyVisual();
    }

    public void ApplyVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = sprite;

    }
}