using System.Collections.Generic;
using UnityEngine;

public class RegionTask : MonoBehaviour, ITouchable
{
    public static List<RegionTask> All = new List<RegionTask>();
    //selected task?
    
    public int TaskIndex = -1;
    //task infos

    [SerializeField] private Material brightMaterial;
    [SerializeField] private Material fadeMaterial;

    private BoxCollider _collider;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        All.Add(this);
    }
    
    private void OnDisable()
    {
        All.Remove(this);
    }
    
    public void OnClick()
    {
        var centerTop = new Vector3(_collider.bounds.center.x, _collider.bounds.max.y, _collider.bounds.center.z);
        SelectionInfo.Instance.ShowSelect(centerTop);
        
        foreach (var region in All)
            region.SetFade(true);
        SetFade(false);
        var collRegions = CollisionDrawer.Instance.GetCollidingRegionsFor(this);
        foreach (var cr in collRegions)
            cr.SetFade(false);
        
        //CubeSizer.Instance.ShowPlanes(_collider.bounds.center.x, 0f, 0f);

        CollisionDrawer.Instance.SetVisible(false, 1);

        Cardbar.Instance.ShowTaskCard(TaskIndex);
        //+v otdelnoy kartochke pokazat s kakimi taskami peresekayetsa
    }
    
    public void OnDoubleClick()
    {
        CameraController.Instance.SetTargetPosition(_collider.bounds.center);
        var viewSize = (_collider.bounds.max - _collider.bounds.min).magnitude * 1.2f;
        if (viewSize < 5f) viewSize = 5f;
        CameraController.Instance.SetCameraSize(viewSize);
        
        CubeSizer.Instance.ShowPlanes(_collider.bounds.center.x, 0f, 0f);
    }

    public void OnUnClick()
    {
        SelectionInfo.Instance.HideSelect();
        
        foreach (var region in RegionTask.All)
            region.SetFade(false);
        
        //CubeSizer.Instance.HidePlanes();

        CollisionDrawer.Instance.SetVisible(true, 1);

        Cardbar.Instance.Clear();
    }

    public void SetFade(bool value)
    {
        _renderer.material = value ? fadeMaterial : brightMaterial;
    }
}
