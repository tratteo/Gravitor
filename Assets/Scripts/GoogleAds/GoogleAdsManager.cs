using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

public class GoogleAdsManager : MonoBehaviour
{
    public enum RewardedAdType { EXTRA_ATTEMPT, BONUS_GP }

    private static GoogleAdsManager instance = null;
    public static GoogleAdsManager GetInstance() { return instance; }

    public string appID = "ca-app-pub-2239617021238574~2592756280";
    public bool useTestAdsID = true;

    private RewardedAd extraAttempt = null;
    private string extraAttemptID;
    private RewardedAd bonusGP = null;
    private string bonusGPID;

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
            bonusGPID = "ca-app-pub-3940256099942544/5224354917";
        }
        else
        {
            extraAttemptID = "ca-app-pub-2239617021238574/7600245152";
            bonusGPID = "ca-app-pub-2239617021238574/6299605927";
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

            case RewardedAdType.BONUS_GP:
                if (bonusGP == null)
                {
                    return false;
                }
                return bonusGP.IsLoaded();
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

            case RewardedAdType.BONUS_GP:
                if (bonusGP == null)
                {
                    break;
                }

                if (bonusGP.IsLoaded())
                {
                    bonusGP.Show();
                }
                break;
        }
    }


    //private IEnumerator LoadAds_C()
    //{
    //    LoadAd(RewardedAdType.EXTRA_ATTEMPT);
    //    LoadAd(RewardedAdType.BONUS_GP);

    //    yield return new WaitForSeconds(5f);
    //    while (true)
    //    {
    //        if (!extraAttempt.IsLoaded())
    //        {
    //            LoadAd(RewardedAdType.EXTRA_ATTEMPT);
    //        }
    //        if (!bonusGP.IsLoaded())
    //        {
    //            LoadAd(RewardedAdType.BONUS_GP);
    //        }
    //        yield return new WaitForSeconds(10f);
    //    }
    //}

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

            case RewardedAdType.BONUS_GP:
                bonusGP = new RewardedAd(bonusGPID);
                request = new AdRequest.Builder().Build();
                bonusGP.LoadAd(request);
                bonusGP.OnUserEarnedReward += HandleUserEarnedReward;
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
