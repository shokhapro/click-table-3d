using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SelectionInfo : MonoBehaviour
{
    public static SelectionInfo Instance;

    [SerializeField] private Image image;

    private Vector3 _position = Vector3.zero;
    private RectTransform _rt;
    private Camera _c;

    private void Awake()
    {
        Instance = this;
        
        _rt = GetComponent<RectTransform>();
        
        _c = Camera.main;

        HideSelect();
    }

    private void Update()
    {
        var vpos = _c.WorldToScreenPoint(_position);

        _rt.position = vpos;
    }

    public void ShowSelect(Vector3 position)
    {
        _position = position;
        
        image.enabled = true;
    }
    
    public void HideSelect()
    {
        image.enabled = false;
    }
}