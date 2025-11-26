// FeedbackManager.cs
using UnityEngine;
using System;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;

    public CameraShake cameraShake;
    public VictoryEffect victoryEffect;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 封装通用反馈接口（确保所有操作走统一路径）
    public void PlayActionFeedback(string actionType)
    {
        switch (actionType)
        {
            case "PlayCard":
            case "ServeDish":
                cameraShake?.TriggerShake(duration: 0.3f, magnitude: 0.12f, speed: 10f);
                break;
            case "GameWin":
                victoryEffect?.PlayVictorySequence();
                break;
        }
    }
}