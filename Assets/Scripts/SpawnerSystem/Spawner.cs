using System;
using UnityEngine;

///<summary>
/// Setup:
/// <para>1. Create a Spawner passing the context (MonoBehaviour) and your spawn ranges</para> 
/// <para>2. Create a timer within the spawner calling CreateSpawnTimer</para> 
/// <para>3. Subscribe delegate method to spawn event calling SubscribeToSpawnTimerEvent</para> 
/// <para>4. Create when needed a timed spawn exception calling CreateSpawnException and either start it on creation or later calling StartTimedException</para> 
/// <para>5. Get the random spawn position in the area of the Spawner calling GetSpawnPosition, the Spawner will take care of an eventual spawn area exception</para> 
/// <para>For further info please take a look at the read_me file inside package folder</para>
/// </summary> 
public class Spawner
{
    private SpawnTimer spawnTimer = null;
    private SpawnException spawnException = null;
    public readonly Vector2 areaHorizontalRange, areaVerticalRange, areaDepthRange;

    private MonoBehaviour context;

    public Spawner(MonoBehaviour context, Vector2 horizontalRange, Vector2 verticalRange, Vector2 depthRange)
    {
        this.context = context;
        this.areaHorizontalRange = horizontalRange;
        this.areaVerticalRange = verticalRange;
        this.areaDepthRange = depthRange;

        if (areaHorizontalRange.x - areaHorizontalRange.y == 0
            || areaVerticalRange.x - areaVerticalRange.y == 0
            || areaDepthRange.x - areaDepthRange.y == 0)
        {
            throw new System.Exception("Spawner area is not valid, check that it is indeed a volume\n(can't be 0 of thickness in a certain direction: horizontal.x != horizontal.y for every range, vertial and depth");
        }
    }

    public SpawnTimer CreateSpawnTimer(int fixedSpawnRate, bool startNow)
    {
        spawnTimer = new SpawnTimer(context, fixedSpawnRate, startNow);
        return spawnTimer;
    }
    public SpawnTimer CreateSpawnTimer(Vector2 spawnRateRange, bool startNow)
    {
        spawnTimer = new SpawnTimer(context, spawnRateRange, startNow);
        return spawnTimer;
    }
    public SpawnTimer CreateSpawnTimer(Func<int, float> scaleOverTimeFunc, bool startNow)
    {
        spawnTimer = new SpawnTimer(context, scaleOverTimeFunc, startNow);
        return spawnTimer;
    }


    public void StartSpawnTimer()
    {
        spawnTimer.StartSpawnRoutine();
    }
    public void KillSpawnTimer()
    {
        spawnTimer.KillSpawnRoutine();
    }
    public void PauseSpawnTimer(bool state)
    {
        spawnTimer.PauseSpawnRoutine(state);
    }
    public void SubscribeToSpawnEvent(Action functionToSub)
    {
        spawnTimer.SubscribeFunction(functionToSub);
    }
    public void UnsubscribeToSpawnEvent(Action functionToUnsub)
    {
        spawnTimer.UnsubscribeFunction(functionToUnsub);
    }
    
    public SpawnException CreateSpawnException(Vector3 centre, float width, float height, float depth)
    {
        spawnException = new SpawnException(context, this, centre, width, height, depth);
        return spawnException;
    }
    public SpawnException CreateSpawnException(Vector3 centre, float width, float height, float depth, float duration, bool startNow)
    {
        spawnException = new SpawnException(context, this, centre, width, height, depth, duration, startNow);
        return spawnException;
    }
    public void StartTimedException()
    {
        spawnException.StartException();
    }
    public void StopException()
    {
        spawnException.StopException();
    }
    public bool IsExceptionActive()
    {
        return spawnException.IsActive();
    }
    public Vector3 GetSpawnPosition()
    {
        if (spawnException != null && spawnException.IsActive())
        {
            return spawnException.GetNextPosition();
        }
        else
        {
            return new Vector3(UnityEngine.Random.Range(areaHorizontalRange.x, areaHorizontalRange.y), UnityEngine.Random.Range(areaVerticalRange.x, areaVerticalRange.y), UnityEngine.Random.Range(areaDepthRange.x, areaDepthRange.y));
        }
    }
}
