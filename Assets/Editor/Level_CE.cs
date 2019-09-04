using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Level))]
public class Level_CE : Editor
{
    Level script;
    SerializedObject so;
    SerializedProperty stringsProperty;
    private void OnEnable()
    {
        script = (Level)target;
        so = new SerializedObject(script);
        stringsProperty = so.FindProperty("poolManager");
    }
    public override void OnInspectorGUI()
    {
        script.id = EditorGUILayout.IntField("ID", script.id);
        EditorGUILayout.Space(20);
        script.category = (Level.LevelCategory)EditorGUILayout.EnumPopup("Level category", script.category);
        switch(script.category)
        {
            case Level.LevelCategory.TIME:
                script.targetTime = EditorGUILayout.FloatField("Target time", script.targetTime);
                break;
            case Level.LevelCategory.OBSTACLES_DESTROY:
                script.targetObstaclesDestoryed = EditorGUILayout.IntField("Target obstacles destroyed", script.targetObstaclesDestoryed);
                break;
            case Level.LevelCategory.TIME_DILATED:
                script.targetTimeDilated = EditorGUILayout.FloatField("Target time dilated", script.targetTimeDilated);
                break;
            case Level.LevelCategory.DISTANCE:
                script.targetDistance = EditorGUILayout.FloatField("Target distance", script.targetDistance);
                break;
        }
        EditorGUILayout.Space(10);
        script.levelObjective = EditorGUILayout.TextField("Level objective", script.levelObjective);
        EditorGUILayout.LabelField("Level info", EditorStyles.helpBox);
        script.levelInfo = EditorGUILayout.TextArea(script.levelInfo, GUILayout.Height(30));
        script.bronzeScore = EditorGUILayout.IntField("Bronze score", script.bronzeScore);
        script.silverScore = EditorGUILayout.IntField("Silver score", script.silverScore);
        script.goldScore = EditorGUILayout.IntField("Gold score", script.goldScore);
        EditorGUILayout.LabelField("Extras", EditorStyles.helpBox);
        script.extrasSpawnRateRange = EditorGUILayout.Vector2Field("Extras spawn rate range", script.extrasSpawnRateRange);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Obstacles", EditorStyles.helpBox);
        script.overrideSpawnArea = EditorGUILayout.Toggle("Override Spawn area", script.overrideSpawnArea);
        if(script.overrideSpawnArea)
        {
            script.randXSpawn = EditorGUILayout.Vector2Field("Rand X Spawn", script.randXSpawn);
            script.randYSpawn = EditorGUILayout.Vector2Field("Rand Y Spawn", script.randYSpawn);
            script.randZSpawn = EditorGUILayout.Vector2Field("Rand Z Spawn", script.randZSpawn);
        }
        EditorGUILayout.Space(5);
        script.isSpawnRateConst = EditorGUILayout.Toggle("Constant spawn rate", script.isSpawnRateConst);
        if (script.isSpawnRateConst)
        {
            script.constSpawnRate = EditorGUILayout.FloatField("Spawn rate", script.constSpawnRate);
        }
        else
        {
            EditorGUILayout.LabelField("f(x)=-(a) / (bx + g) + d", EditorStyles.helpBox);
            script.a = EditorGUILayout.FloatField("Alpha", script.a);
            script.b = EditorGUILayout.FloatField("Beta", script.b);
            script.g = EditorGUILayout.FloatField("Gamma", script.g);
            script.d = EditorGUILayout.FloatField("Delta", script.d);
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
        }
    }
}
