using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Obstacle))]
public class ObstacleC_E : Editor
{
    Obstacle script;
    SerializedObject so;
    SerializedProperty stringsProperty;

    private void OnEnable()
    {
        script = (Obstacle)target;
        so = new SerializedObject(script);
        stringsProperty = so.FindProperty("materialsPool");
    }
    public override void OnInspectorGUI()
    {
        script.type = (Obstacle.ObstacleType)EditorGUILayout.EnumPopup("Obstacle Type", script.type);
        EditorGUILayout.LabelField("Scale", EditorStyles.helpBox);
        script.minScale = EditorGUILayout.IntField("Min scale", script.minScale);
        script.maxScale = EditorGUILayout.IntField("Max scale", script.maxScale);
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
        switch (script.type)
        {
            case Obstacle.ObstacleType.PLANET:
                EditorGUILayout.LabelField("Planet fields", EditorStyles.boldLabel);
                break;

            case Obstacle.ObstacleType.STAR:
                EditorGUILayout.LabelField("Star fields", EditorStyles.boldLabel);
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