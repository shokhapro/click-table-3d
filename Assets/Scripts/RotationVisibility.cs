using System;
using UnityEngine;

public class RotationVisibility : MonoBehaviour
{
    private enum RotAxis {x, y, z}
    
    [SerializeField] private RotAxis cameraAxis = RotAxis.y;
    [SerializeField] private RotAxis rotationAxis = RotAxis.y;
    
    [Serializable]
    public class RotLimit
    {
        public float toAngle = 90f;
        public float setAngle = 90f;
    }
    
    [SerializeField] private RotLimit[] cameraLimits = new RotLimit[1];

    private Transform _t;
    private Vector3 _rot;

    private void Awake()
    {
        _t = transform;
        
        _rot = _t.eulerAngles;
    }

    private void Update()
    {
        var cr = CameraController.Instance.GetRotation();
        var ca = GetAxis(cr, cameraAxis);

        foreach (var l in cameraLimits)
            if (ca <= l.toAngle)
            {
                SetAxis(ref _rot, l.setAngle, rotationAxis);
                _t.eulerAngles = _rot;
                return;
            }
    }

    private float GetAxis(Vector3 rot, RotAxis axis)
    {
        var val = 0f;
        
        switch (axis)
        {
            case RotAxis.x:
                val = rot.x;
                break;
            case RotAxis.y:
                val = rot.y;
                break;
            case RotAxis.z:
                val = rot.z;
                break;
        }

        return val;
    }
    
    private void SetAxis(ref Vector3 rot, float val, RotAxis axis)
    {
        switch (axis)
        {
            case RotAxis.x:
                rot.x = val;
                break;
            case RotAxis.y:
                rot.y = val;
                break;
            case RotAxis.z:
                rot.z = val;
                break;
        }
    }
}
