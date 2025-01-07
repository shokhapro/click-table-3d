using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [SerializeField] private Vector2 rotationDegree = new Vector2(180f, 180f);
    [SerializeField] private Vector2 rotationLimitX = new Vector2(-90f, 90f);
    [SerializeField] private Vector2 rotationLimitY = new Vector2(-90f, 90f);
    [SerializeField] private float rotationSmooth = 0.8f;
    [Space]
    [SerializeField] private Camera cam;
    [SerializeField] private Camera raycastCam;
    [SerializeField] private float cameraSizeMin = 2f;
    [SerializeField] private float zoomSmooth = 0.8f;
    [Space]
    [SerializeField] private BoxCollider boundsCollider;
    [SerializeField] private float moveFactor = 1f;
    [SerializeField] private float moveSmooth = 0.8f;
    [Space]
    [SerializeField] private float matrixSmooth = 0.8f;
    
    private Transform _t;
    private Vector3 _pos;
    private Vector3 _eangles;
    private Vector3 _rot;
    private float _camSize;
    private Vector3 _camPos;
    private bool _isTouching1 = false;
    private bool _isTouching2 = false;
    private Vector2 _lastTouch1Pos;
    private Vector2 _lastTouch2Pos;
    private bool _isAnyTouch = false;

    private void Awake()
    {
        Instance = this;
        
        _t = transform;
        
        _pos = _t.transform.position;

        _eangles = _t.eulerAngles;

        _rot = _t.eulerAngles;

        _camSize = cam.orthographicSize;

        _camPos = cam.transform.localPosition;
        
        LoadCameraPrefs();
    }

    private void Update()
    {
        UpdateTouch();
        
        UpdateRotation();

        UpdateAutoPin();

        _eangles = Vector3.Lerp(_rot, _eangles, rotationSmooth);
        _t.eulerAngles = _eangles;

        UpdateZoom();

        UpdateCameraProjection();

        cam.orthographicSize = Mathf.Lerp(_camSize, cam.orthographicSize, zoomSmooth);
        raycastCam.orthographicSize = cam.orthographicSize;
        _camPos.z = Mathf.Lerp(-_camSize * 2.5f, _camPos.z, zoomSmooth);

        cam.transform.localPosition = Vector3.Lerp(_camPos, cam.transform.localPosition, moveSmooth);
        raycastCam.transform.localPosition = cam.transform.localPosition;
        
        UpdateMove();
        
        _t.transform.position = Vector3.Lerp(_pos, _t.transform.position, moveSmooth);
    }

    private void LateUpdate()
    {
        LateUpdateTouch();

        if (Input.touchCount > 0 != _isAnyTouch)
        {
            _isAnyTouch = Input.touchCount > 0;
            
            if (!_isAnyTouch) SaveCameraPrefs();
        }
    }

    private void UpdateTouch()
    {
        void Finger1()
        {
            if (Input.touchCount < 1)
            {
                _isTouching1 = false;
                return;
            }

            var t1 = Input.GetTouch(0);
            if (t1.phase == TouchPhase.Began)
            {
                if (CheckUI.IsPointerOverUIObject("ui")) return;
                _isTouching1 = true;
                _lastTouch1Pos = t1.position;
            }
            if (t1.phase == TouchPhase.Ended || t1.phase == TouchPhase.Canceled) _isTouching1 = false;
        }

        void Finger2()
        {
            if (Input.touchCount < 2)
            {
                _isTouching2 = false;
                return;
            }
        
            var t2 = Input.GetTouch(1);
            if (t2.phase == TouchPhase.Began)
            {
                if (CheckUI.IsPointerOverUIObject("ui")) return;
                _isTouching2 = true;
                _lastTouch2Pos = t2.position;
            }
            if (t2.phase == TouchPhase.Ended || t2.phase == TouchPhase.Canceled) _isTouching2 = false;
        }

        Finger1();
        Finger2();
    }

    private void LateUpdateTouch()
    {
        if (Input.touchCount > 0)
        {
            var t1 = Input.GetTouch(0);
            
            _lastTouch1Pos = t1.position;
        }
        
        if (Input.touchCount > 1)
        {
            var t2 = Input.GetTouch(1);
            
            _lastTouch2Pos = t2.position;
        }
    }

    private void UpdateRotation()
    {
        if (!_isTouching1 || _isTouching2) return;

        var t1 = Input.GetTouch(0);

        if (t1.phase != TouchPhase.Moved) return;
    
        var dtpos = t1.deltaPosition;
        var dmovex = 1f * dtpos.x / Screen.width;
        var dmovey = 1f * dtpos.y / Screen.width;
        _rot.y += dmovex * rotationDegree.x;
        _rot.y = Mathf.Clamp(_rot.y, rotationLimitY[0], rotationLimitY[1]);
        _rot.x -= dmovey * rotationDegree.y;
        _rot.x = Mathf.Clamp(_rot.x, rotationLimitX[0], rotationLimitX[1]);
    }

    private void UpdateAutoPin()
    {
        if (_isTouching1) return;
        
        //if (_camSize < cameraSizeMin * 3f) return;

        var r = _rot;
        if (r.x > 85f) r.x = 90f;
        if (r.x > -5f && r.x < 5f) r.x = 0f;
        if (r.x % 90f != 0f) return;
        var ryi = r.y % 90f;
        var ryn = r.y - ryi;
        if (ryi > 45f) ryi = 90f;
        if (ryi > -45f && ryi < 45f) ryi = 0f;
        if (ryi < -45f) ryi = -90f;
        r.y = ryn + ryi;
        _rot = r;
    }
    
    private void UpdateCameraProjection()
    {
        Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time) {
            Matrix4x4 ret = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                ret[i] = Mathf.Lerp(from[i], to[i], time);
            return ret;
        }
        
        var fov = 45f;
        var near = .3f;
        var far = 1000f;
        
        var aspect = (float)Screen.width / Screen.height;
        var ortho = Matrix4x4.Ortho(-_camSize * aspect, _camSize * aspect, -_camSize, _camSize, near, far);
        var pers = Matrix4x4.Perspective(fov, aspect, near, far);

        var tyval = (_t.eulerAngles.y % 45f) / 45f;
        var tyrev = Mathf.FloorToInt(_t.eulerAngles.y / 45f) % 2 == 1;
        var ty = tyrev ? 1 - tyval : tyval;
        var txval = (_t.eulerAngles.x % 45f) / 45f;
        var txrev = Mathf.FloorToInt(_t.eulerAngles.x / 45f) % 2 == 1;
        var tx = txrev ? 1 - txval : txval;
        var t = ty * 0.5f + tx * 0.5f;

        var pm = MatrixLerp(ortho, pers, t);
        cam.projectionMatrix = MatrixLerp(pm, cam.projectionMatrix, matrixSmooth);
        
        raycastCam.projectionMatrix = MatrixLerp(ortho, pers, t < 0.04f ? 0f : t);
        raycastCam.orthographic = t < 0.04f;
    }

    private void UpdateZoom()
    {
        if (!_isTouching1 || !_isTouching2) return;

        var t1 = Input.GetTouch(0);
        var t2 = Input.GetTouch(1);
        
        if (t1.phase != TouchPhase.Moved) return;
        if (t2.phase != TouchPhase.Moved) return;
        
        var dis1 = Vector2.Distance(_lastTouch1Pos, _lastTouch2Pos);
        var dis2 = Vector2.Distance(t1.position, t2.position);
        var dzoom = dis2 / dis1;
        _camSize -= _camSize * (dzoom - 1f);
        _camSize = Mathf.Clamp(_camSize, cameraSizeMin, GetCameraSizeMax());
    }

    private float GetCameraSizeMax()
    {
        return (boundsCollider.bounds.max - boundsCollider.bounds.min).magnitude * 1.2f;
    }
 
    private void UpdateMove()
    {
        if (!_isTouching1 || !_isTouching2) return;
        
        var t1 = Input.GetTouch(0);
        var t2 = Input.GetTouch(1);
        
        if (t1.phase != TouchPhase.Moved) return;
        if (t2.phase != TouchPhase.Moved) return;

        var cen1 = (_lastTouch1Pos + _lastTouch2Pos) * 0.5f;
        var cen2 = (t1.position + t2.position) * 0.5f;
        var dmove = cen2 - cen1;

        var rot2 = _rot;
        var xdir = Quaternion.Euler(rot2) * Vector3.right;
        var ydir = Quaternion.Euler(rot2) * Vector3.up;

        var pos = _pos;
        pos -= xdir * dmove.x * moveFactor;
        pos -= ydir * dmove.y * moveFactor;
        
        _pos = PositionClampInBounds(pos);
    }

    private Vector3 PositionClampInBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, boundsCollider.bounds.min.x, boundsCollider.bounds.max.x),
            Mathf.Clamp(position.y, boundsCollider.bounds.min.y, boundsCollider.bounds.max.y),
            Mathf.Clamp(position.z, boundsCollider.bounds.min.z, boundsCollider.bounds.max.z)
        );
    }


    private class CameraPrefs
    {
        public Vector3 pos;
        public Vector3 rot;
        public float size;
    }

    private void SaveCameraPrefs()
    {
        var prefs = new CameraPrefs();
        prefs.pos = _pos;
        prefs.rot = _rot;
        prefs.size = _camSize;
        var json = prefs.Serialize().json;
        
        PlayerPrefs.SetString("camprefs", json);
    }

    private void LoadCameraPrefs()
    {
        if (!PlayerPrefs.HasKey("camprefs")) return;
        
        var json = PlayerPrefs.GetString("camprefs");
        var prefs = Newtonsoft.Json.JsonConvert.DeserializeObject<CameraPrefs>(json);
        if (prefs == null) return;
        
        _pos = prefs.pos;
        _rot = prefs.rot;
        _camSize = prefs.size;

        _t.transform.position = _pos;
        _eangles = _rot;
        _t.eulerAngles = _eangles;
        cam.orthographicSize = _camSize;
    }
    
    
    public Vector2 GetRotation()
    {
        return _rot;
    }

    public void UpdatePosition()
    {
        _pos = PositionClampInBounds(_pos);
    }

    public void SetTargetPosition(Vector3 pos)
    {
        _pos = pos;
    }

    public void SetCameraSize(float size)
    {
        _camSize = Mathf.Clamp(size, cameraSizeMin, GetCameraSizeMax());
    }
    
    public float GetCameraSize()
    {
        return _camSize;
    }
}
