using UnityEngine;

public class TextDrawer : MonoBehaviour
{
    public static TextDrawer Instance;
    
    [SerializeField] private TitleObject projectTextPrefab;
    [SerializeField] private Transform projectTextParent;
    [Space]
    [SerializeField] private TitleObject monthText;
    [SerializeField] private TitleObject dayTextPrefab;
    [SerializeField] private TitleObject activeDayTextPrefab;
    [SerializeField] private Transform dayTextParent;
    [Space]
    [SerializeField] private TitleObject workerTextPrefab;
    [SerializeField] private Transform workerTextParent;

    private void Awake()
    {
        Instance = this;
    }

    public void SetProjectTexts(string[] texts)
    {
        for (var ci = 0; ci < projectTextParent.childCount; ci++)
            Destroy(projectTextParent.GetChild(ci).gameObject);

        foreach (var t in texts)
        {
            var to = Instantiate(projectTextPrefab, projectTextParent);
            to.Set(t.Trim());
        }
    }
    
    public void SetMonthText(string text)
    {
        monthText.Set(text.Trim());
    }
    
    public void SetDaysTexts(string[] texts, int activeDayId = -1)
    {
        for (var ci = 0; ci < dayTextParent.childCount; ci++)
            Destroy(dayTextParent.GetChild(ci).gameObject);

        for (var ti = 0; ti < texts.Length; ti++)
        {
            var prefab = dayTextPrefab;
            if (ti == activeDayId)
                prefab = activeDayTextPrefab;
            
            var to = Instantiate(prefab, dayTextParent);
            to.Set(texts[ti].Trim());
        }
    }
    
    public void SetWorkersTexts(string[] texts)
    {
        for (var ci = 0; ci < workerTextParent.childCount; ci++)
            Destroy(workerTextParent.GetChild(ci).gameObject);
        
        foreach (var t in texts)
        {
            var to = Instantiate(workerTextPrefab, workerTextParent);
            to.Set(t.Trim());
        }
    }
}
