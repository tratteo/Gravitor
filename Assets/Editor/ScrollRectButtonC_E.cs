using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(ScrollRectButton))]
public class ScrollRectButtonC_E : Editor
{
    ScrollRectButton script;
    private void OnEnable()
    {
        script = (ScrollRectButton)target;
    }
    public override void OnInspectorGUI()
    {
        script.mainTrigger = (EventTrigger)EditorGUILayout.ObjectField("Trigger", script.mainTrigger, typeof(EventTrigger), true);
        script.colorPressEffect = EditorGUILayout.Toggle("Pressed color effect", script.colorPressEffect);
        if (script.colorPressEffect)
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
