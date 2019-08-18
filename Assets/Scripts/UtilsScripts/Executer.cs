using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Executer : MonoBehaviour
{
    // Asynchronous -> Synchronous
    // It is common in Unity to deal with Asynchronous events. Because of the way Unity is built, some functions can only be called from the main thread.
    // To use this class, attach this script to a gameObject and this class will take care of making that object a persistent gameObject, that means it 
    // will persist between scenes. After attaching this script to the gameObject is possible to call the static method Executer.GetInstance() to get the 
    // instance of the executer. To enqueue a job simply call the method AddJob(Action job). It is possible to think about this class as a method to execute
    // an asynchronous function in the main thread.
    // I.E:
    // For example when implementing the google rewarded ad API, a function is called asynchronously when the player finish watching an ad. It could be useful to 
    // modify some UI in order to inform the player that he just earned a reward. The event, however, is asynchornous so Unity will throw an exception if someone 
    // tries to modify the UI in the async func. With this class, simply use the following code:
    
    // Executer.GetInstance().AddJob(F);
    // ...
    // void F() { sometext.text = "Earned Reward"; }
    
    // TIP:
    // It is possible to use a contracted form:
    // Executer.GetInstance().AddJob( () => { sometext.text = "Earned Reward"; });

    #region Singleton
    private static Executer instance;

    /// <summary>
    /// Get the singleton instance of the Executer
    /// </summary>
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

    /// <summary>
    /// Enqueue a job to be performed, the job must have the following declaration: void F()
    /// </summary>
    public void AddJob(Action job)
    {
        jobs.Enqueue(job);
    }
}
