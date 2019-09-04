using UnityEngine;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] private Text bronzeScore;
    [SerializeField] private Text silverScore;
    [SerializeField] private Text goldScore;
    [SerializeField] private Text info;
    [SerializeField] private Text highScore;

    [SerializeField] private GameObject levelInfoPanel;

    private Level selectedLevel;

    void Start()
    {
        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.MENU_SONG);
    }

    public void LoadCurrentSelectedLevel()
    {
        LevelLoader.GetInstance().SetCurrentLevel(selectedLevel);

        SceneLoader loader = GetComponent<SceneLoader>();
        AudioManager.GetInstance().SmoothOutSound(AudioManager.GetInstance().currentMusic, 0.05f, 1f);
        loader.LoadSceneAsynchronously(SceneLoader.MAP_NAME);
    }

    public void OpenLevelInfo(Level level)
    {
        levelInfoPanel.SetActive(true);
        bronzeScore.text = " > " + level.bronzeScore.ToString("0");
        silverScore.text = " > " + level.silverScore.ToString("0");
        goldScore.text = " > " + level.goldScore.ToString("0");
        highScore.text = "Highscore\n" + SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>().GetLevelHighScore(level.id).ToString("0");
        info.text = level.levelInfo;
        selectedLevel = level;
    }

    public void CloseLevelInfo()
    {
        selectedLevel = null;
        levelInfoPanel.SetActive(false);
    }
}
