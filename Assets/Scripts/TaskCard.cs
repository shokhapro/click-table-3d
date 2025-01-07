using TMPro;
using UnityEngine;

public class TaskCard : Card
{
    [SerializeField] private TextMeshProUGUI text;
    
    public override void Init(int key)
    {
        text.text = "Task id:" + key + "\n"
                    + DataDrawer.Instance.Table.tasks[key].title + "\n"
                    + DataDrawer.Instance.Table.projects[DataDrawer.Instance.Table.tasks[key].projectIndex].value + " | "
                    + DataDrawer.Instance.Table.workers[DataDrawer.Instance.Table.tasks[key].workerIndex].value + "\n"
                    + DataDrawer.Instance.Table.tasks[key].startTime + " - " +
                    DataDrawer.Instance.Table.tasks[key].finishTime;
        //budet brat danniye iz DataDrawers sam po task id
        //i pokaz danniye nujnie
        //pokaz deskription
        
        //+napisat chto peresekayetsa s drugimi zadaniyami, warning
    }
}
