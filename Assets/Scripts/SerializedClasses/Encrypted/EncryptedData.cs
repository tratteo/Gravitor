using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncryptedData
{
    public string deviceId;

    protected void InitializeDeviceId()
    {
        deviceId = deviceId == null ? SystemInfo.deviceUniqueIdentifier : deviceId;
    }
}
