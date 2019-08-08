using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ButtonPress))]
public class ButtonPressC_E : Editor
{
    ButtonPress script;
    private void OnEnable()
    {
        script = (ButtonPress)target;
    }
    public override void OnInspectorGUI()
    {
        script.colorPressEffect = EditorGUILayout.Toggle("Pressed color effect", script.colorPressEffect);
        if(script.colorPressEffect)
        {
            script.pressedColor = EditorGUILayout.ColorField("Pressed color", script.pressedColor);
        }
        script.resizeOnPress = EditorGUILayout.Toggle("Resize on press", script.resizeOnPress);
        if (script.resizeOnPress)
        {
            script.pressedScaleMultiplier = EditorGUILayout.Vector2Field("Resize multiplier", script.pressedScaleMultiplier);
        }
        EditorGUILayout.LabelField("", EditorStyles.helpBox);
        script.playSound = EditorGUILayout.Toggle("Play sound", script.playSound);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
        }
    }
}
