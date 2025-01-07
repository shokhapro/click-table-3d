using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDrawer : MonoBehaviour
{
    public static CollisionDrawer Instance;
    
    [SerializeField] private CollisionInfo collInfoPrefab;

    private RectTransform _rt;
    private List<CollisionInfo> _collInfos = new List<CollisionInfo>();
    private List<bool> _isVisible = new List<bool>(1){true};
    
    private void Awake()
    {
        Instance = this;

        _rt = GetComponent<RectTransform>();
    }
    
    public void UpdateDraw()
    {
        Clean();
        
        Bounds[] boundsArray = new Bounds[RegionTask.All.Count];
        for (var ri = 0; ri < RegionTask.All.Count; ri++)
            boundsArray[ri] = RegionTask.All[ri].GetComponent<MeshRenderer>().bounds;
        
        for (var b1i = 0; b1i < boundsArray.Length; b1i++)
        {
            if (b1i == boundsArray.Length - 1) break;
                
            for (var b2i = b1i + 1; b2i < boundsArray.Length; b2i++)
            {
                var b1 = boundsArray[b1i];
                var b2 = boundsArray[b2i];
                
                if (Math.Abs(b1.center.z - b2.center.z) > 0.001f) continue;
                
                if (b1.min.x > b2.min.x && b1.min.x < b2.max.x || b2.min.x > b1.min.x && b2.min.x < b1.max.x)
                {
                    var collInfo = Instantiate(collInfoPrefab, _rt);
                    collInfo.Position = b1.center;
                    collInfo.Region = RegionTask.All[b1i].GetComponent<RegionTask>();
                    
                    _collInfos.Add(collInfo);

                    break;
                }
            }
        }

        VisibleUpdate();
    }

    public void Clean()
    {
        foreach (var collInfo in _collInfos)
            Destroy(collInfo.gameObject);
        _collInfos.Clear();
    }

    public RegionTask[] GetCollidingRegionsFor(RegionTask region)
    {
        List<RegionTask> collRegions = new List<RegionTask>();
        
        Bounds b1 = region.GetComponent<MeshRenderer>().bounds;

        for (var ri = 0; ri < RegionTask.All.Count; ri++)
        {
            if (RegionTask.All[ri] == region) continue;

            var b2 = RegionTask.All[ri].GetComponent<MeshRenderer>().bounds;
            
            if (Math.Abs(b1.center.z - b2.center.z) > 0.001f) continue;
            
            if (b1.min.x > b2.min.x && b1.min.x < b2.max.x || b2.min.x > b1.min.x && b2.min.x < b1.max.x)
            {
                collRegions.Add(RegionTask.All[ri]);
            }
        }

        return collRegions.ToArray();
    }

    private void VisibleUpdate()
    {
        var visible = true;

        foreach (var v in _isVisible)
            if (!v) { visible = false; break; } 
        
        foreach (var collInfo in _collInfos)
            collInfo.SetVisible(visible);
    }

    public void SetVisible(bool value, int boolId)
    {
        while (_isVisible.Count <= boolId)
            _isVisible.Add(true);
        
        _isVisible[boolId] = value;
        
        VisibleUpdate();
    }
    
    public void SetVisible(bool value)
    {
        SetVisible(value, 0);
    }
}
