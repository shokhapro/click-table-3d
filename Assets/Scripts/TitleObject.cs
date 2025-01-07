using TMPro;
using UnityEngine;

public class TitleObject : MonoBehaviour, ITouchable
{
    public enum Axis3 {x, y, z}
    
    public Axis3 axis = Axis3.x;
    public Vector2 field = new Vector2(0f, 1f);
    [Space]
    [SerializeField] private TextMeshPro[] text;
    [SerializeField] private MeshRenderer highlightRenderer;
    [SerializeField] private Material highlightMaterial;
    
    private Material _normalMaterial;

    private void Awake()
    {
        if (highlightRenderer)
            _normalMaterial = highlightRenderer.material;
    }

    public void Set(string value)
    {
        foreach (var t in text)
            if (t) t.text = value;
    }
    
    public void Highlight(bool value)
    {
        if (!highlightRenderer) return;
        
        highlightRenderer.material = value ? highlightMaterial : _normalMaterial;
    }

    public void OnClick()
    {
        Highlight(true);

        foreach (var region in RegionTask.All)
        {
            var fade = true;

            var p = transform.position;
            var rp = region.transform.position;
            if (axis == Axis3.x)
            {
                if (rp.x <= p.x + field.y && rp.x + region.transform.localScale.x >= p.x + field.x)
                    fade = false;
            }
            else
            {
                var dis = p - rp;
                var dis1 = axis == Axis3.y ? dis.y : dis.z;
                if (dis1 >= field.x && dis1 <= field.y) fade = false;
            }

            region.SetFade(fade);
        }
        
        if (axis == Axis3.x) CubeSizer.Instance.ShowPlanes(transform.position.x + (field.x + field.y) * 0.5f, 0f, 0f);
        else if (axis == Axis3.y) CubeSizer.Instance.ShowPlanes(0f, transform.position.y - (field.x + field.y) * 0.5f, 0f);
        else if (axis == Axis3.z) CubeSizer.Instance.ShowPlanes(0f, 0f, transform.position.z - (field.x + field.y) * 0.5f);
    }
    
    public void OnDoubleClick()
    {
        //
    }

    public void OnUnClick()
    {
        Highlight(false);
        
        foreach (var region in RegionTask.All)
            region.SetFade(false);
        
        //CubeSizer.Instance.HidePlanes();
    }
}
