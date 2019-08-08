using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Executer : MonoBehaviour
{
    #region Singleton
    private static Executer instance;
    public static Executer GetInstance() { return instance; }
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private Queue<Action> jobs = new Queue<Action>();

    private void Update()
    {
        while(jobs.Count > 0)
        {
            jobs.Dequeue().Invoke();
        }
    }

    public void AddJob(Action job)
    {
        jobs.Enqueue(job);
    }
}
