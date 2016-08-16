using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class ActivitySystem : MonoBehaviour {

    public delegate bool CanRunTask_Delegate();

    protected Entity entity;

    protected void Awake() {
        entity = GetComponent<Entity>();
        AddTasks();
        AddTaskRestrictions();
        AddActivities();
        AddActivityRestrictions();
    }

	protected void Start () {
        AddOtherTaskRestrictions();
        AddOtherActivityRestrictions();
    }

    protected abstract void AddTaskRestrictions();
    protected abstract void AddTasks();
    protected abstract void AddOtherTaskRestrictions();

    protected abstract void AddActivityRestrictions();
    protected abstract void AddActivities();
    protected abstract void AddOtherActivityRestrictions();

    //Task stuff
    public delegate void Task();

    public void AddTask(Task task) {
        AddTask(task, false);
    }

    public void AddTask(Task task, bool lateUpdate) {
        DictionaryValue<Task> newEntry = new DictionaryValue<Task>();
        newEntry.OnFailDelegates = null;
        newEntry.restrictions = null;
        newEntry.OnSuccessDelegates = null;

        if (lateUpdate)
            lateUpdateTasks.Add(task, newEntry);
        else
            updateTasks.Add(task, newEntry);
    }

    public void Update() {
        RunTasks(updateTasks);
    }

    public void FixedUpdate() {
        RunTasks(lateUpdateTasks);
    }

    /// <summary>
    /// Values held in the dictionary per task
    /// </summary>
    class DictionaryValue<T> {
        public CanRunTask_Delegate restrictions;
        public T OnFailDelegates;
        public T OnSuccessDelegates;
    }

    Dictionary<Task, DictionaryValue<Task>> updateTasks = new Dictionary<Task, DictionaryValue<Task>>(); 
    Dictionary<Task, DictionaryValue<Task>> lateUpdateTasks = new Dictionary<Task, DictionaryValue<Task>>();

    void RunTasks(Dictionary<Task, DictionaryValue<Task>> tasks) {
        if (tasks.Count == 0)
            return;

        var enumerator = tasks.GetEnumerator();
        using (enumerator) { //Can't do foreach because Unity memory leak unless its been fixed
            while (enumerator.MoveNext()) {
                KeyValuePair<Task, DictionaryValue<Task>> pair = enumerator.Current;

                bool b = true;
                if (pair.Value.restrictions != null)
                    b = CheckCanStart(pair.Value.restrictions.GetInvocationList()); //Check if all restrictions are met
                if (b) { //If they are met, call the function
                    pair.Key();
                    if (pair.Value.OnSuccessDelegates != null)
                        pair.Value.OnSuccessDelegates();
                } else if (pair.Value.OnFailDelegates != null) //Else call the OnFail delegates
                    pair.Value.OnFailDelegates(); 
            }
        }
    }

    public void Add_CanRunTask_Function(Task t, CanRunTask_Delegate d, bool debug = false) {
        DictionaryValue<Task> e;
        updateTasks.TryGetValue(t, out e);
        if (e == null)
            lateUpdateTasks.TryGetValue(t, out e);
        if (debug)
            Debug.Log(e);
        if (e != null) {
            if (debug)
                Debug.Log("Found task");
            e.restrictions += d;
        }
    }

    public void Add_OnFailTask_Function(Task t, Task f) {
        DictionaryValue<Task> e;
        updateTasks.TryGetValue(t, out e);
        if (e == null)
            lateUpdateTasks.TryGetValue(t, out e);
        if (e != null)
            e.OnFailDelegates += f;
    }

    public void Add_OnSuccessTask_Function(Task t, Task f) {
        DictionaryValue<Task> e;
        updateTasks.TryGetValue(t, out e);
        if (e == null)
            lateUpdateTasks.TryGetValue(t, out e);
        if (e != null)
            e.OnSuccessDelegates += f;
    }

    /// <summary>
    /// Runs a list of delegates and checks if any return false
    /// </summary>
    protected bool CheckCanStart(Delegate[] list) {
        if (list == null || list.Length == 0)
            return false;

        for (int i = 0; i < list.Length; i++) {
            Delegate t = list[i];

            bool b = (bool)t.DynamicInvoke();
            if (!b) {
                //Debug.Log(t.Method.Name + " " + t.Target);
                return false;
            }
        } 
        return true; 
    }

    //Activity stuff
    public delegate void Activity();

    public void AddActivity(Activity activity) {
        DictionaryValue<Activity> newEntry = new DictionaryValue<Activity>();
        newEntry.OnFailDelegates = null;
        newEntry.restrictions = null;
        newEntry.OnSuccessDelegates = null;

        activities.Add(activity, newEntry);
    }

    Dictionary<Activity, DictionaryValue<Activity>> activities = new Dictionary<Activity, DictionaryValue<Activity>>();

    public void TryStartActivity(Activity activity) {
        DictionaryValue<Activity> t;
        activities.TryGetValue(activity, out t);

        bool b = true;
        if (t.restrictions != null)
            b = CheckCanStart(t.restrictions.GetInvocationList());

        if (b) { //If we can start, run the activity
            activity();
            if (t.OnSuccessDelegates != null)
                t.OnSuccessDelegates();
        } else if (t.OnFailDelegates != null) //Else call the OnFail delegates
            t.OnFailDelegates();
    }

    public void Add_CanRunActivity_Function(Activity t, CanRunTask_Delegate d) {
        DictionaryValue<Activity> e;
        activities.TryGetValue(t, out e);
        if (e != null)
            e.restrictions += d;
    }

    public void Add_OnFailActivity_Function(Activity t, Activity f) {
        DictionaryValue<Activity> e;
        activities.TryGetValue(t, out e);
        if (e != null)
            e.OnFailDelegates += f;
    }

    public void Add_OnSuccessActivity_Function(Activity t, Activity f) {
        DictionaryValue<Activity> e;
        activities.TryGetValue(t, out e);
        if (e != null)
            e.OnSuccessDelegates += f;
    }
}
