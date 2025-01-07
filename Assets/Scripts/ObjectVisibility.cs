using UnityEngine;

public class ObjectVisibility : MonoBehaviour
{
    [SerializeField] private GameObject activeObject;
    [SerializeField] private Vector2[] cameraXLimit = { new Vector2(-45f, 45f) };
    [SerializeField] private Vector2[] cameraYLimit = { new Vector2(-45f, 45f) };

    private void Update()
    {
        if (!activeObject) return;

        var rot = CameraController.Instance.GetRotation();

        var vx = false;
        foreach (var l in cameraXLimit)
            if (rot.x > l[0] && rot.x < l[1])
            {
                vx = true;
                break;
            }
        var vy = false;
        foreach (var l in cameraYLimit)
            if (rot.y > l[0] && rot.y < l[1])
            {
                vy = true;
                break;
            }
        var v = vx && vy;
        
        activeObject.SetActive(v);
    }
}
