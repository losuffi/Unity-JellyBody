using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectTimer
{
    private static EffectTimer _instance;
    public static EffectTimer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EffectTimer();
            }
            return _instance;
        }
    }

    public abstract class Task
    {

    }
    private abstract class TimerTask : Task
    {
        public float OriginTime
        {
            get;
            protected set;
        }
        public float CompletePercent { get; protected set; }
        protected float calcZeroTime;
        protected float duration;
        protected RunnerType type;
        protected TimerTask()
        {

        }
        public abstract bool Tick();
    }
    public enum RunnerType
    {
        Once,
        Cycle,
    }
    private class TimerActionTask : TimerTask
    {
        public event System.Action feedback;
        public TimerActionTask(float duration, RunnerType type)
        {
            //if (duration < Time.deltaTime)
            //{
            //    Debug.LogError("Time set error:Please set duration unless deltaTime");
            //}
            OriginTime = Time.time;
            this.duration = duration;
            this.type = type;
            CompletePercent = 0;
            calcZeroTime = 0;
        }
        public override bool Tick()
        {
            calcZeroTime += Time.deltaTime;
            if (calcZeroTime >= duration)
            {
                if (type == RunnerType.Once)
                {
                    FeedBack();
                    return true;
                }
                else if (type == RunnerType.Cycle)
                {
                    FeedBack();
                    calcZeroTime = 0;
                    return false;
                }
            }
            return false;
        }
        private void FeedBack()
        {
            if (feedback != null)
            {
                feedback();
            }
        }
    }
    private class BoolActionTask : TimerTask
    {
        public event System.Action feedback;
        private System.Func<bool> expression;
        public BoolActionTask(float duration, System.Func<bool> expression)
        {
            OriginTime = Time.time;
            this.duration = duration;
            CompletePercent = 0;
            calcZeroTime = 0;
            this.expression = expression;
        }
        public override bool Tick()
        {
            calcZeroTime += Time.deltaTime;
            if (calcZeroTime >= duration)
            {
                if (!expression())
                {
                    return false;
                }
                FeedBack();
                return true;
            }
            return false;
        }
        private void FeedBack()
        {
            if (feedback != null)
            {
                feedback();
            }
        }
    }
    private List<TimerTask> taskCollection;
    private EffectTimerProxy proxy;
    private EffectTimer()
    {
        taskCollection = new List<TimerTask>();
        GameObject p = new GameObject();
        EffectTimerProxy proxy = p.AddComponent<EffectTimerProxy>();
        proxy.update += Update;
        MonoBehaviour.DontDestroyOnLoad(p);
    }
    private Task temp;
    private void Update()
    {
        for (int i = taskCollection.Count - 1; i >= 0; i--)
        {
            temp = taskCollection[i];
            if (taskCollection[i].Tick())
            {
                DestroyTask(temp);
            }
        }
    }
    public Task RunTimerTask(float duration, RunnerType type, System.Action feedback)
    {
        TimerActionTask task = new TimerActionTask(duration, type);
        task.feedback += feedback;
        taskCollection.Add(task);
        return task;
    }
    public Task RunTimerTask(float duration, System.Func<bool> expression, System.Action feedback)
    {
        BoolActionTask task = new BoolActionTask(duration, expression);
        task.feedback += feedback;
        taskCollection.Add(task);
        return task;
    }
    public void DestroyTask(Task t)
    {
        if (t == null || !taskCollection.Contains(t as TimerTask))
        {
            return;
        }
        taskCollection.Remove((t as TimerTask));
    }
}
public class EffectTimerProxy : MonoBehaviour
{
    public event System.Action update;
    void Update()
    {
        if (update != null)
        {
            update();
        }
    }
}
