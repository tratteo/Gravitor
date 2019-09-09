using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

public class GoogleAdsManager : MonoBehaviour
{
    public enum RewardedAdType { EXTRA_ATTEMPT }
    public const string ATTEMPT_ID = "Attempt";
    public const string BONUSGP_ID = "GravityPoints";


    private static GoogleAdsManager instance = null;
    public static GoogleAdsManager GetInstance() { return instance; }

    public string appID = "ca-app-pub-2239617021238574~2592756280";
    public bool useTestAdsID = true;

    private RewardedAd extraAttempt = null;
    private string extraAttemptID;

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
        }
        else
        {
            extraAttemptID = "ca-app-pub-2239617021238574/7600245152";
        }
        //StartCoroutine(LoadAds_C());
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
        }
    }

    public void LoadAd(RewardedAdType type)
    {
        AdRequest request;
        switch (type)
        {
            case RewardedAdType.EXTRA_ATTEMPT:
                extraAttempt = new RewardedAd(extraAttemptID);
                request = new AdRequest.Builder().Build();
                extraAttempt.LoadAd(request);
                extraAttempt.OnUserEarnedReward += HandleUserEarnedReward;
                break;
        }
    }

    private void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        Debug.Log(type);
        RewardClaimed(type);
    }
}
