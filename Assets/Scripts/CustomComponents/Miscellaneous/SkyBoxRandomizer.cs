using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRandomizer : MonoBehaviour
{
    [Header("SkyBoxPlanes")]
    [SerializeField] private GameObject front = null;
    [SerializeField] private GameObject back = null;
    [SerializeField] private GameObject up = null;
    [SerializeField] private GameObject down = null;
    [SerializeField] private GameObject left = null;
    [SerializeField] private GameObject right = null;
    [Header("TexturePool")]
    [SerializeField] private SkyBoxSample[] skyBoxPool = null;
    void Start()
    {
        int index = Random.Range(0, skyBoxPool.Length);
        front.GetComponent<MeshRenderer>().material.mainTexture = skyBoxPool[index].frontTexture;
        back.GetComponent<MeshRenderer>().material.mainTexture = skyBoxPool[index].backTexture;
        up.GetComponent<MeshRenderer>().material.mainTexture = skyBoxPool[index].upTexture;
        down.GetComponent<MeshRenderer>().material.mainTexture = skyBoxPool[index].downTexture;
        left.GetComponent<MeshRenderer>().material.mainTexture = skyBoxPool[index].leftTexture;
        right.GetComponent<MeshRenderer>().material.mainTexture = skyBoxPool[index].rightTexture;
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }
}
