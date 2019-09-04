﻿using UnityEngine.UI; 
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(HUDManager))]
public class HUDManagerC_E : Editor
{
    HUDManager script;
    private void OnEnable()
    {
        script = (HUDManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("HUD", EditorStyles.helpBox);
        script.showHUDField = EditorGUILayout.Toggle("Show HUD", script.showHUDField);
        if(script.showHUDField)
        {
            script.scoreText = (Text)EditorGUILayout.ObjectField("Score text", script.scoreText, typeof(Text), true);
            script.timeRelativeText = (Text)EditorGUILayout.ObjectField("Time relative text", script.timeRelativeText, typeof(Text), true);
            script.distanceText = (Text)EditorGUILayout.ObjectField("Distance text", script.distanceText, typeof(Text), true);
            script.speedText = (Text)EditorGUILayout.ObjectField("Speed text", script.speedText, typeof(Text), true);
            script.timeProperText = (Text)EditorGUILayout.ObjectField("Time proper text", script.timeProperText, typeof(Text), true);
            script.fpsText = (Text)EditorGUILayout.ObjectField("FPS text", script.fpsText, typeof(Text), true);
            script.gameOverScoreText = (Text)EditorGUILayout.ObjectField("GameOverScore text", script.gameOverScoreText, typeof(Text), true);
            script.gameOverGravityPointsText = (Text)EditorGUILayout.ObjectField("GameOver GP text", script.gameOverGravityPointsText, typeof(Text), true);
            script.gameOverInfoText = (Text)EditorGUILayout.ObjectField("GameOverInfo text", script.gameOverInfoText, typeof(Text), true);
            script.healthText = (Text)EditorGUILayout.ObjectField("Health text", script.healthText, typeof(Text), true);
            script.loadingAdText = (Text)EditorGUILayout.ObjectField("LoadingAd text", script.loadingAdText, typeof(Text), true);
            script.healthBar = (Image)EditorGUILayout.ObjectField("Health bar", script.healthBar, typeof(Image), true);
            script.shieldBtn = (GameObject)EditorGUILayout.ObjectField("Shield button", script.shieldBtn, typeof(GameObject), true);
            script.shieldChargeIcon = (Image)EditorGUILayout.ObjectField("Shield use bar", script.shieldChargeIcon, typeof(Image), true);
            script.shieldsText = (Text)EditorGUILayout.ObjectField("Shields text", script.shieldsText, typeof(Text), true);
            script.antigravityBtn = (GameObject)EditorGUILayout.ObjectField("Antigravity button", script.antigravityBtn, typeof(GameObject), true);
            script.quantumTunnelBtn = (GameObject)EditorGUILayout.ObjectField("Quantumtunnel button", script.quantumTunnelBtn, typeof(GameObject), true);
            script.solarflareBtn = (GameObject)EditorGUILayout.ObjectField("Solarflare button", script.solarflareBtn, typeof(GameObject), true);
            script.gammaRayBurstBtn = (GameObject)EditorGUILayout.ObjectField("GammaRay button", script.gammaRayBurstBtn, typeof(GameObject), true);
            script.adButton = (GameObject)EditorGUILayout.ObjectField("AD button", script.adButton, typeof(GameObject), true);
        }

        EditorGUILayout.LabelField("Panels", EditorStyles.helpBox);
        script.showPanelsField = EditorGUILayout.Toggle("Show Panels", script.showPanelsField);
        if(script.showPanelsField)
        {
            script.controlPanel = (GameObject)EditorGUILayout.ObjectField("Controls", script.controlPanel, typeof(GameObject), true);
            script.HUDPanel = (GameObject)EditorGUILayout.ObjectField("HUD", script.HUDPanel, typeof(GameObject), true);
            script.timerPanel = (GameObject)EditorGUILayout.ObjectField("Timer", script.timerPanel, typeof(GameObject), true);
            script.gameOverPanel = (GameObject)EditorGUILayout.ObjectField("GameOver", script.gameOverPanel, typeof(GameObject), true);
            script.levelCompletedPanel = (GameObject)EditorGUILayout.ObjectField("LevelCompleted", script.levelCompletedPanel, typeof(GameObject), true);
            script.levelObjectivePanel = (GameObject)EditorGUILayout.ObjectField("Level objective", script.levelObjectivePanel, typeof(GameObject), true);
            script.pausePanel = (GameObject)EditorGUILayout.ObjectField("Pause", script.pausePanel, typeof(GameObject), true);
            script.tutorialPanel = (GameObject)EditorGUILayout.ObjectField("Tutorial", script.tutorialPanel, typeof(GameObject), true);
            script.highScorePanel = (GameObject)EditorGUILayout.ObjectField("HighScore", script.highScorePanel, typeof(GameObject), true);
            script.quantumTunnelPanel = (GameObject)EditorGUILayout.ObjectField("Quantumtunnel", script.quantumTunnelPanel, typeof(GameObject), true);
            script.highGravityFieldPanel = (GameObject)EditorGUILayout.ObjectField("HighGravityField", script.highGravityFieldPanel, typeof(GameObject), true);
            script.statsPanel = (GameObject)EditorGUILayout.ObjectField("Stats", script.statsPanel, typeof(GameObject), true);
        }

        EditorGUILayout.LabelField("Toasts", EditorStyles.helpBox);
        script.showToastsField = EditorGUILayout.Toggle("Show Toasts", script.showToastsField);
        if(script.showToastsField)
        {
            script.inGameToast = (ToastScript)EditorGUILayout.ObjectField("Game", script.inGameToast, typeof(ToastScript), true);
            script.achievementToast = (ToastScript)EditorGUILayout.ObjectField("Achievements", script.achievementToast, typeof(ToastScript), true);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
        }
    }
}
