using UnityEngine;

[System.Serializable]
public class CurrencyData : EncryptedData
{
    public int gravityPoints;
    public int gravitons;

    public CurrencyData()
    {
        gravityPoints = 0;
        gravitons = 0;
    }

    public void InitializeMissingData()
    {
        base.InitializeDeviceId();
    }
}
