using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{

    [SerializeField] private Text versionText;

    void Start()
    {
        versionText.text = "Version: " + Application.version;
    }
}
