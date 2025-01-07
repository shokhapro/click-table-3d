using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class DataDrawer : MonoBehaviour
{
    public static DataDrawer Instance;

    [Serializable]
    public class Table3D
    {
        [Serializable]
        public class Task
        {
            public string id = "";
            public int projectIndex = 0;
            public int workerIndex = 0;
            public DateTime startTime = DateTime.MinValue;
            public DateTime finishTime = DateTime.MinValue;
            public bool isActive = false;
            public string title = "";
            public Color color = Color.green;

            public Task(string Id, int ProjectIndex, int WorkerIndex, DateTime StartTime, DateTime FinishTime, bool IsActive, string Title, Color Clr)
            {
                id = Id;
                projectIndex = ProjectIndex;
                workerIndex = WorkerIndex;
                startTime = StartTime;
                finishTime = FinishTime;
                isActive = IsActive;
                title = Title;
                color = Clr;
            }
        }
        
        [Serializable]
        public class IdString
        {
            public string id = "";
            public string value = "";

            public IdString(string Id, string Value)
            {
                id = Id;
                value = Value;
            }

            public static string[] ToStringArray(IdString[] IdStrings)
            {
                string[] array = new string[IdStrings.Length];
                for (var i = 0; i < IdStrings.Length; i++)
                    array[i] = IdStrings[i].value;
            
                return array;
            }
        }

        public IdString[] workers = new IdString[0];
        public IdString[] projects = new IdString[0];
        public Task[] tasks = new Task[0];
    }
    
    [SerializeField] private string jsonUrl = "";
    [Space]
    [SerializeField] private Mesh regionMesh;
    [SerializeField] private RegionTask regionPrefab;
    [Space]
    [SerializeField] private UnityEvent onShowData;

    private Transform _t;
    private string _json = "";
    [HideInInspector] public Table3D Table = new Table3D();
    private Coroutine _updateDataCoroutine = null;
    
    private int _monthShift = 0;
    private bool _onlyActive = true;

    private void Awake()
    {
        Instance = this;
        
        _t = transform;
    }
    
    private void Start()
    {
        ShowData();
        
        UpdateData();
    }

    public void UpdateData()
    {     
        IEnumerator Updating()
        {
            LoadingAnimation.Instance.Set(true);
            
            yield return DownloadJson();
            
            if (_json == "")
            {
                _updateDataCoroutine = null;
            
                LoadingAnimation.Instance.Set(false);
                
                yield break;
            }

            List<Table3D.IdString> workers = new List<Table3D.IdString>();
            List<Table3D.IdString> projects = new List<Table3D.IdString>();
            List<Table3D.Task> tasks = new List<Table3D.Task>();

            ClickJson data = JsonConvert.DeserializeObject<ClickJson>(_json);

            var groups = data.result.data;
            foreach (var group in groups)
            {
                if (group.TASKS == null) continue;

                projects.Add(new Table3D.IdString(group.ID, group.NAME));
                var projectIndex = projects.Count - 1;

                var gtasks = group.TASKS;
                foreach (var gtask in gtasks)
                {
                    var workerIndex = -1; 
                    
                    var userid = gtask.RESPONSIBLE_ID;
                    var users = group.USERS;
                    
                    /*
                    foreach (var user in users)
                    {
                        if (user.USER_ID != userid) continue;
                        
                        for (var w = 0; w < workers.Count; w++)
                            if (workers[w].id == userid)
                                workerIndex = w;

                        if (workerIndex == -1)
                        {
                            workers.Add(new IdString(userid, user.NAME));
                            workerIndex = workers.Count - 1;
                        }
                        break;
                    }
                    */

                    ClickJson.USER user = null;
                    foreach (var u in users) if (u.USER_ID == userid) user = u;
                    if (user == null)
                    {
                        user = new ClickJson.USER();
                        user.USER_ID = userid;
                        user.NAME = "??? [USER_ID_"+userid+"_NOT_IN_GROUP]";
                        user.WORK_POSITION = "??? [USER_ID_"+userid+"_NOT_IN_GROUP]";
                    }

                    for (var w = 0; w < workers.Count; w++)
                        if (workers[w].id == user.USER_ID)
                            workerIndex = w;

                    if (workerIndex == -1)
                    {
                        workers.Add(new Table3D.IdString(user.USER_ID, user.NAME));
                        workerIndex = workers.Count - 1;
                    }


                    DateTime startTime = DateTime.MaxValue;
                    var startDate = gtask.START_DATE_PLAN != null ? gtask.START_DATE_PLAN : gtask.CREATED_DATE;
                    if (startDate != null)
                        startTime = DateTime.ParseExact(startDate, "dd.MM.yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.CurrentCulture);
                    DateTime finishTime = DateTime.MinValue;
                    var endDate = gtask.END_DATE_PLAN != null ? gtask.END_DATE_PLAN : gtask.DEADLINE;
                    if (endDate != null)
                        finishTime = DateTime.ParseExact(endDate, "dd.MM.yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.CurrentCulture);

                    var color = new Color(0, 1, 0);
                    //ColorUtility.TryParseHtmlString(_table.tasks[ti].hexColor, out color);
                    //color..a = 1f;
                    //color = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
                    var hue = (int.Parse(gtask.ID) % 32f) / 32f;
                    color = Color.HSVToRGB(hue, 1f, 1f);
                    //System.Random rand = new System.Random(int.Parse(gtask.ID));

                    tasks.Add(new Table3D.Task(
                        gtask.ID,
                        projectIndex,
                        workerIndex,
                        startTime,
                        finishTime,
                        gtask.IS_ACTIVE,
                        gtask.TITLE,
                        color
                        ));
                }
            }
            
            Table = new Table3D();
            Table.projects = projects.ToArray();
            Table.workers = workers.ToArray();
            Table.tasks = tasks.ToArray();

            ShowData();
            if (Table.tasks.Length == 0) Debug.Log("No tasks to show");

            _updateDataCoroutine = null;
            
            LoadingAnimation.Instance.Set(false);
        }

        if (_updateDataCoroutine != null) return;
        _updateDataCoroutine = StartCoroutine(Updating());
    }
    
    private IEnumerator DownloadJson()
    {
        _json = "";
        
        using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
        {
            IEnumerator SendingRequest() { yield return request.SendWebRequest(); }
            Coroutine reqc = StartCoroutine(SendingRequest());
            yield return new WaitForSeconds(10);
            if (!request.isDone)
            {
                StopCoroutine(reqc);
                Debug.LogError("Error: Server is not responding");
                yield break;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error + "\nCheck your connection / Try to restart the app");
                yield break;
            }

            _json = request.downloadHandler.text;

            Debug.Log("Success");
        }
    }

    private void ShowData()
    {
        for (var ci = 0; ci < _t.childCount; ci++)
            Destroy(_t.GetChild(ci).gameObject);
        
        onShowData.Invoke();

        var dateTimeNow = DateTime.Now.AddMonths(_monthShift);

        var timeMin = new DateTime(dateTimeNow.Year, dateTimeNow.Month, 1);
        var timeMax = new DateTime(dateTimeNow.Year, dateTimeNow.AddMonths(1).Month, 1);
        var timeScale = DateTime.DaysInMonth(dateTimeNow.Year, dateTimeNow.Month) * 0.5f;

        for (var ti = 0; ti < Table.tasks.Length; ti++)
        {
            if (_onlyActive && !Table.tasks[ti].isActive) continue;
            
            if (Table.tasks[ti].startTime < timeMin && 
                 (Table.tasks[ti].finishTime == DateTime.MinValue || Table.tasks[ti].finishTime < timeMin) ||
                Table.tasks[ti].startTime > timeMax)
                continue;

            var m = new Mesh();
            m.vertices = regionMesh.vertices;
            m.triangles = regionMesh.triangles;
            m.normals = regionMesh.normals;
            var colors = new Color[m.vertexCount];
            for (var ci = 0; ci < colors.Length; ci++)
                colors[ci] = Table.tasks[ti].color;
            m.colors = colors;

            var r = Instantiate(regionPrefab, _t);
            r.GetComponent<MeshFilter>().mesh = m;
            var bounds = r.GetComponent<MeshRenderer>().bounds;
            var cldr = r.GetComponent<BoxCollider>();
            cldr.center = bounds.center;
            cldr.size = bounds.size;
            r.TaskIndex = ti;

            var rpos = 1f * (Table.tasks[ti].startTime.Ticks - timeMin.Ticks) / (timeMax.Ticks - timeMin.Ticks) * timeScale;
            var rlen = Table.tasks[ti].finishTime == DateTime.MinValue ? 0.25f : 1f * (Table.tasks[ti].finishTime.Ticks - Table.tasks[ti].startTime.Ticks) / (timeMax.Ticks - timeMin.Ticks) * timeScale;
            if (rpos < -1f) { rlen += rpos + 1f; rpos = -1f; }
            if (rpos + rlen > timeScale + 1f) rlen = timeScale + 1f - rpos;
            r.transform.localPosition = new Vector3(rpos, -Table.tasks[ti].projectIndex - 0.5f, Table.tasks[ti].workerIndex + 0.5f);
            r.transform.localScale = new Vector3(rlen, 1f, 1f);
        }
        
        TextDrawer.Instance.SetProjectTexts(Table3D.IdString.ToStringArray(Table.projects));
        TextDrawer.Instance.SetWorkersTexts(Table3D.IdString.ToStringArray(Table.workers));

        var month = dateTimeNow.ToString("MMMM", CultureInfo.InvariantCulture)+ " " + dateTimeNow.Year.ToString();
        TextDrawer.Instance.SetMonthText(month);
        
        var thisMonthDayCount = DateTime.DaysInMonth(dateTimeNow.Year, dateTimeNow.Month);

        string GetThisMonthWeekDay(int monthDay)
        {
            var thisMonthDay = new DateTime(dateTimeNow.Year, dateTimeNow.Month, monthDay);
            return thisMonthDay.DayOfWeek.ToString();
        }

        string[] days = new string[thisMonthDayCount];
        for (var di = 0; di < days.Length; di++)
        {
            var weekday = GetThisMonthWeekDay(di + 1).Substring(0, 3).ToUpper();
            days[di] = (di + 1) + "  <color=#"+(weekday == "SUN" ? "ccc" : "555")+">" + weekday + "</color>";
        }
        TextDrawer.Instance.SetDaysTexts(days, dateTimeNow.Month == DateTime.Now.Month ? DateTime.Now.Day - 1 : -1);

        CubeSizer.Instance.SetSize(
            thisMonthDayCount,
            Table.projects.Length < 7 ? 7 : Table.projects.Length,
            Table.workers.Length < 1 ? 1 : Table.workers.Length
        );
        
        this.DelayedAction(0.2f, CameraController.Instance.UpdatePosition);
        
        this.DelayedAction(1f, CollisionDrawer.Instance.UpdateDraw);
    }

    public void MonthShift(int add)
    {
        if (_updateDataCoroutine != null) return;
        
        _monthShift += add;
        
        ShowData();
    }

    public void OnlyActive(bool value)
    {
        _onlyActive = value;
        
        if (_updateDataCoroutine != null) return;

        ShowData();
    }
}
