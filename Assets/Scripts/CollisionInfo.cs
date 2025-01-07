using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CollisionInfo : MonoBehaviour
{
    public Vector3 Position = Vector3.zero;
    public ITouchable Region = null;

    private RectTransform _rt;
    private Camera _c;
    private Image _image;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        
        _c = Camera.main;

        _image = GetComponent<Image>();
    }

    private void Update()
    {
        var vpos = _c.WorldToScreenPoint(Position);

        _rt.position = vpos;
    }

    public void OnClick()
    {
        CameraController.Instance.SetTargetPosition(Position);
        CameraController.Instance.SetCameraSize(4f);
        
        TouchController.Instance.ClickObject(Region);
    }
    
    public void SetVisible(bool value)
    {
        _image.enabled = value;
    }
}
