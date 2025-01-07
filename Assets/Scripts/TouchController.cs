using UnityEngine;

public class TouchController : MonoBehaviour
{
    public static TouchController Instance;
    
    private Camera _camera;
    private bool _touchStart = false;
    private float _touchStartTime = 0f;
    private Vector3 _touchStartPosition;
    private ITouchable _touchObject;

    private void Awake()
    {
        Instance = this;
        
        _camera = GameObject.FindWithTag("Raycast Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;
        
        if (Input.touchCount > 1)
        {
            _touchStart = false;
            return;
        }
        
        if (CheckUI.IsPointerOverUIObject()) return;

        var t = Input.GetTouch(0);

        switch (t.phase)
        {
            case TouchPhase.Began:
                _touchStart = true;
                _touchStartTime = Time.time;
                _touchStartPosition = t.position;
                break;
            case TouchPhase.Ended:
                if (!_touchStart) break;
                if (Time.time - _touchStartTime > 0.5f) break;
                if (Vector3.Distance(t.position, _touchStartPosition) / Screen.width > 0.05f) break;
                Click(_touchStartPosition);
                _touchStart = false;
                break;
        }
    }
    
    private void Click(Vector3 pos)
    {
        RaycastHit hit = new RaycastHit();
        
        float radius = CameraController.Instance.GetCameraSize() * 0.05f;
        
        if (_camera.orthographic)
        {
            var origin = _camera.ScreenToWorldPoint(pos);
            Physics.SphereCast(origin, radius, _camera.transform.forward, out hit, 200f);
        }
        else
        {
            Ray ray = _camera.ScreenPointToRay(pos);
            Physics.SphereCast(ray, radius, out hit, 200f);
        }

        if (!hit.collider)
        {
            if (_touchObject as Object)
                _touchObject.OnUnClick();
            _touchObject = null;
            return;
        }

        ITouchable touchObject;
        if (hit.collider.TryGetComponent(out touchObject))
            ClickObject(touchObject);
        else
        {
            TouchableTarget touchTarget;
            if (hit.collider.TryGetComponent(out touchTarget))
                if (touchTarget.target.TryGetComponent(out touchObject))
                    ClickObject(touchObject);
        }
    }

    public void ClickObject(ITouchable touchObject)
    {
        if (touchObject == _touchObject)
        {
            touchObject.OnDoubleClick();
            return;
        }

        if (_touchObject as Object)
            _touchObject.OnUnClick();

        touchObject.OnClick();

        _touchObject = touchObject;
    }
}
