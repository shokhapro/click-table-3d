using UnityEngine;

public class CubeSizer : MonoBehaviour
{
    public static CubeSizer Instance;
    
    [SerializeField] private Transform box;
    [SerializeField] private Material boxMatFB;
    [SerializeField] private Material boxMatLR;
    [SerializeField] private Material boxMatTB;
    [Space]
    [SerializeField] private Transform[] xLines;
    [SerializeField] private Transform[] yLines;
    [SerializeField] private Transform[] zLines;
    [SerializeField] private float lineLength = 10f;
    [SerializeField] private float lineWidth = 0.03f;
    [Space]
    [SerializeField] private Transform planeX;
    [SerializeField] private Transform planeY;
    [SerializeField] private Transform planeZ;

    private void Awake()
    {
        Instance = this;
        
        boxMatFB.mainTextureScale = new Vector2(1, 1);
        boxMatLR.mainTextureScale = new Vector2(1, 1);
        boxMatTB.mainTextureScale = new Vector2(1, 1);
    }

    public void SetSize(int x, int y, int z)
    {
        var x2 = x * 0.5f;
        box.localScale = new Vector3(x2, y, z);
        box.position = new Vector3(x2 * 0.5f, y * -0.5f, z * 0.5f);
        boxMatFB.mainTextureScale = new Vector2(x, y);
        boxMatLR.mainTextureScale = new Vector2(z, y);
        boxMatTB.mainTextureScale = new Vector2(x, z);

        foreach (var xl in xLines)
            xl.localScale = new Vector3(
                lineWidth / box.lossyScale.y,
                lineLength,
                lineWidth / box.lossyScale.z
                );
        foreach (var yl in yLines)
            yl.localScale = new Vector3(
                lineWidth / box.lossyScale.x,
                lineLength,
                lineWidth / box.lossyScale.z
            );
        foreach (var zl in zLines)
            zl.localScale = new Vector3(
                lineWidth / box.lossyScale.y,
                lineLength,
                lineWidth / box.lossyScale.x
            );
    }

    public void ShowPlanes(float x, float y, float z)
    {
        if (!x.Equals(0f))
        {
            planeX.transform.position = new Vector3(x, box.position.y, box.position.z);
            planeX.gameObject.SetActive(true);
        }
        else
            planeX.gameObject.SetActive(false);
        
        if (!y.Equals(0f))
        {
            planeY.transform.position = new Vector3(box.position.x, y, box.position.z);
            planeY.gameObject.SetActive(true);
        }
        else
            planeY.gameObject.SetActive(false);
        
        if (!z.Equals(0f))
        {
            planeZ.transform.position = new Vector3(box.position.x, box.position.y, z);
            planeZ.gameObject.SetActive(true);
        }
        else
            planeZ.gameObject.SetActive(false);
    }

    public void HidePlanes()
    {
        ShowPlanes(0f, 0f, 0f);
    }
}
