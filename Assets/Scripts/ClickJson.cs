using System;
using System.Collections.Generic;

[Serializable]
public class ClickJson
{
    [Serializable]
    public class Datum
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public List<USER> USERS { get; set; }
        public List<TASK> TASKS { get; set; }
    }

    [Serializable]
    public class Result
    {
        public bool success { get; set; }
        public List<Datum> data { get; set; }
    }

    [Serializable]
    public class TASK
    {
        public string ID { get; set; }
        public string TITLE { get; set; }
        public string RESPONSIBLE_ID { get; set; }
        public string CREATED_DATE { get; set; }
        public string DEADLINE { get; set; }
        public string START_DATE_PLAN { get; set; }
        public string END_DATE_PLAN { get; set; }
        public bool IS_ACTIVE { get; set; }
    }

    [Serializable]
    public class Time
    {
        public double start { get; set; }
        public double finish { get; set; }
        public double duration { get; set; }
        public double processing { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_finish { get; set; }
    }

    [Serializable]
    public class USER
    {
        public string USER_ID { get; set; }
        public string NAME { get; set; }
        public string WORK_POSITION { get; set; }
    }

    public Result result { get; set; }
    public Time time { get; set; }
}
