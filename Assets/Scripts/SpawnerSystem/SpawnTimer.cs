using System;
using System.Collections;
using UnityEngine;

public class SpawnTimer
{
    private enum SpawnRateParadigm { FIXED, RANDOM_RANGE, SCALE_FUNC }
    private SpawnRateParadigm paradigm;
    private Vector2 spawnRateRange;
    private MonoBehaviour context;
    private float currentTime;
    private bool routineActive;
    private float spawnRate;
    private float spawnTime;
    private bool isPaused = false;

    private event Action SpawnEvent;
    private Func<int, float> scaleFunc;

    public SpawnTimer(MonoBehaviour context, float fixedSpawnRate, bool startNow)
    {
        routineActive = false;
        this.context = context;
        paradigm = SpawnRateParadigm.FIXED;
        spawnRate = fixedSpawnRate;
        if (startNow)
        {
            StartSpawnRoutine();
        }
    }
    public SpawnTimer(MonoBehaviour context, Vector2 spawnRateRange, bool startNow)
    {
        routineActive = false;
        this.context = context;
        this.spawnRateRange = spawnRateRange;
        paradigm = SpawnRateParadigm.RANDOM_RANGE;
        spawnRate = UnityEngine.Random.Range(this.spawnRateRange.x, this.spawnRateRange.y);
        if (startNow)
        {
            StartSpawnRoutine();
        }
    }
    public SpawnTimer(MonoBehaviour context, Func<int, float> scaleFunc, bool startNow)
    {
        routineActive = false;
        this.context = context;
        this.scaleFunc = scaleFunc;
        paradigm = SpawnRateParadigm.SCALE_FUNC;
        spawnRate = scaleFunc(0);
        if (startNow)
        {
            StartSpawnRoutine();
        }
    }

    public void SubscribeFunction(Action functionToSub)
    {
        SpawnEvent += functionToSub;
    }

    public void UnsubscribeFunction(Action functionToUnsub)
    {
        SpawnEvent -= functionToUnsub;
    }

    public void StartSpawnRoutine()
    {
        routineActive = true;
        spawnTime = 1 / spawnRate;
        currentTime = Time.timeSinceLevelLoad;
        context.StartCoroutine(SpawnCoroutine());
    }

    public void KillSpawnRoutine()
    {
        routineActive = false;
    }

    public void PauseSpawnRoutine(bool state)
    {
        isPaused = state;
    }

    private IEnumerator SpawnCoroutine()
    {
        while (routineActive)
        {
            if (!isPaused && Time.timeSinceLevelLoad >= currentTime + spawnTime)
            {
                if(SpawnEvent != null)
                    SpawnEvent();

                switch(paradigm)
                {
                    case SpawnRateParadigm.FIXED:
                        break;
                    case SpawnRateParadigm.RANDOM_RANGE:
                        spawnRate = UnityEngine.Random.Range(spawnRateRange.x, spawnRateRange.y);
                        break;
                    case SpawnRateParadigm.SCALE_FUNC:
                        spawnRate = scaleFunc((int)Time.timeSinceLevelLoad);
                        break;
                }
                spawnTime = 1 / spawnRate;
                currentTime = Time.timeSinceLevelLoad;
            }
            yield return null;
        }
    }
}
