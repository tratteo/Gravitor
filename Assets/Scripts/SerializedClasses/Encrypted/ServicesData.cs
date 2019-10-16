using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServicesData : EncryptedData
{
    public System.DateTime lastAccess;
    public System.DateTime lastRewardClaimed;

    public ServicesData()
    {
        lastAccess = System.DateTime.Now;
        lastRewardClaimed = System.DateTime.Now;
    }

    public void InitializeMissingData()
    {
        lastAccess = lastAccess.Year == 0001 ? System.DateTime.Now : lastAccess;
        lastRewardClaimed = lastRewardClaimed.Year == 0001 ? System.DateTime.Now : lastRewardClaimed;
        base.InitializeDeviceId();
    }
}
