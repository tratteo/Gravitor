using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GameObstacle))]
public class GameObstacleC_E : Editor
{
    GameObstacle script;
    SerializedObject so;
    SerializedProperty stringsProperty;
    private void OnEnable()
    {
        script = (GameObstacle)target;
        so = new SerializedObject(script);
        stringsProperty = so.FindProperty("materialsPool");
    }
    public override void OnInspectorGUI()
    {
        script.type = (GameObstacle.ObstacleType)EditorGUILayout.EnumPopup("Obstacle Type", script.type);
        EditorGUI.BeginDisabledGroup(true);
        script.mass = EditorGUILayout.FloatField("Mass", script.mass);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.LabelField("Scale", EditorStyles.helpBox);
        script.minScale = EditorGUILayout.IntField("Min scale", script.minScale);
        script.maxScale = EditorGUILayout.IntField("Max scale", script.maxScale);
        EditorGUILayout.LabelField("Density", EditorStyles.helpBox);
        script.minDensity = EditorGUILayout.FloatField("Min density", script.minDensity);
        script.maxDensity = EditorGUILayout.FloatField("Max density", script.maxDensity);
        EditorGUILayout.LabelField("Preferences", EditorStyles.helpBox);
        script.rotate = EditorGUILayout.Toggle("Rotate", script.rotate);
        EditorGUILayout.LabelField("Material randomizer", EditorStyles.helpBox);
        script.randomizeMaterials = EditorGUILayout.Toggle("Enabled", script.randomizeMaterials);
        if (script.randomizeMaterials)
        {
            EditorGUILayout.PropertyField(stringsProperty, true);
            so.ApplyModifiedProperties();
        }
        EditorGUILayout.LabelField("References", EditorStyles.helpBox);
        script.deathEffect = (GameObject)EditorGUILayout.ObjectField("Death effect", script.deathEffect, typeof(GameObject), false);
        switch (script.type)
        {
            case Obstacle.ObstacleType.PLANET:
                EditorGUILayout.LabelField("Planet fields", EditorStyles.boldLabel);
                break;

            case Obstacle.ObstacleType.STAR:
                EditorGUILayout.LabelField("Star fields", EditorStyles.boldLabel);
                script.starFlare = (GameObject)EditorGUILayout.ObjectField("Star Flare", script.starFlare, typeof(GameObject), false);
                break;

            case Obstacle.ObstacleType.WHITE_DWARF:
                EditorGUILayout.LabelField("White Dwarf fields", EditorStyles.boldLabel);
                break;

            case Obstacle.ObstacleType.NEUTRON_STAR:
                EditorGUILayout.LabelField("Neutron Star fields", EditorStyles.boldLabel);
                break;
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
        }
    }
}