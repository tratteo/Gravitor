using GoogleMobileAds.Api;
using System;
using UnityEngine;
using System.Collections;

public class GoogleAdsManager : MonoBehaviour
{
    public enum RewardedAdType { EXTRA_ATTEMPT, TIMED_REWARD }
    public const string ATTEMPT_ID = "Attempt";
    public const string TIMED_REWARD_ID = "TimedReward";


    private static GoogleAdsManager instance = null;
    public static GoogleAdsManager GetInstance() { return instance; }

    public string appID = "ca-app-pub-2239617021238574~2592756280";
    public bool useTestAdsID = true;

    private RewardedAd extraAttempt = null;
    private string extraAttemptID;
    private bool extraAttemptLoading = false;

    private RewardedAd timedReward = null;
    private string timedRewardID;
    private bool timedRewardLoading = false;

    private Action<string> RewardClaimed;
    public void SubscribeToRewardClaimed(Action<string> funcToSub) { RewardClaimed += funcToSub; }
    public void UnsubscribeToRewardClaimed(Action<string> funcToUnsub) { RewardClaimed -= funcToUnsub; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);


        MobileAds.Initialize(appID);
        if (useTestAdsID)
        {
            extraAttemptID = "ca-app-pub-3940256099942544/5224354917";
            timedRewardID = "ca-app-pub-3940256099942544/5224354917";
        }
        else
        {
            extraAttemptID = "ca-app-pub-2239617021238574/7600245152";
            //TODO set
            timedRewardID = "ca-app-pub-2239617021238574/2587152628";
        }

        StartCoroutine(LoadAd_C());
    }

    public bool IsRewardedAdLoaded(RewardedAdType type)
    {
        switch (type)
        {
            case RewardedAdType.EXTRA_ATTEMPT:
                if (extraAttempt == null)
                {
                    return false;
                }
                return extraAttempt.IsLoaded();

            case RewardedAdType.TIMED_REWARD:
                if (timedReward == null)
                {
                    return false;
                }
                return timedReward.IsLoaded();
        }
        return false;
    }

    public void ShowRewardedAd(RewardedAdType type)
    {
        switch (type)
        {
            case RewardedAdType.EXTRA_ATTEMPT:
                if (extraAttempt == null)
                {
                    break;
                }

                if (extraAttempt.IsLoaded())
                {
                    extraAttempt.Show();
                }
                break;

            case RewardedAdType.TIMED_REWARD:
                if (timedReward == null)
                {
                    break;
                }

                if (timedReward.IsLoaded())
                {
                    timedReward.Show();
                }
                break;
        }
    }

    private void LoadAd(RewardedAdType type)
    {
        AdRequest request;
        switch (type)
        {
            case RewardedAdType.EXTRA_ATTEMPT:
                extraAttempt = new RewardedAd(extraAttemptID);
                request = new AdRequest.Builder().AddTestDevice("59F1657632AC57BAC87BEE49D98DFD57").Build();
                extraAttempt.LoadAd(request);
                extraAttempt.OnUserEarnedReward += HandleUserEarnedReward;
                break;
            case RewardedAdType.TIMED_REWARD:
                timedReward = new RewardedAd(timedRewardID);
                request = new AdRequest.Builder().AddTestDevice("59F1657632AC57BAC87BEE49D98DFD57").Build();
                timedReward.LoadAd(request);
                timedReward.OnUserEarnedReward += HandleUserEarnedReward;
                break;
        }
    }

    public IEnumerator LoadAd_C()
    {
        while(true)
        {
            if(extraAttempt == null || !extraAttempt.IsLoaded())
            {
                if (!extraAttemptLoading)
                {
                    Debug.Log("Loading Extra attempt");
                    LoadAd(RewardedAdType.EXTRA_ATTEMPT);
                    extraAttemptLoading = true;
                }
            }
            else
            {
                extraAttemptLoading = false;
            }

            if(timedReward == null || !timedReward.IsLoaded())
            {
                if (!timedRewardLoading)
                {
                    Debug.Log("Loading Timed reward");
                    LoadAd(RewardedAdType.TIMED_REWARD);
                    timedRewardLoading = true;
                }
            }
            else
            {
                timedRewardLoading = false;
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        RewardClaimed(type);
    }
}
