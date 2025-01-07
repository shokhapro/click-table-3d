using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TextBackground : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Vector2 spacing = Vector2.zero;
    [SerializeField] private Vector2 offset = Vector2.zero;

    private Transform _t;

    private void Awake()
    {
        _t = transform;
    }

    private void Update()
    {
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (text == null) return;
        
        _t.localScale = new Vector3(text.preferredWidth + spacing.x, text.preferredHeight + spacing.y, 1f);
        _t.localPosition = new Vector3(
            -(text.rectTransform.pivot.x - 0.5f) * text.preferredWidth + offset.x,
            -(text.rectTransform.pivot.y - 0.5f) * text.preferredHeight + offset.y,
            0.01f);
    }
}
