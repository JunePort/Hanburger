// VictoryEffect.cs
using UnityEngine;

public class VictoryEffect : MonoBehaviour
{
    [Header("视觉")]
    public ParticleSystem fireworkEffect;
    [Header("听觉")]
    public AudioSource victoryAudio;
    public AudioClip winSound;

    private bool _isPlaying = false;

    // 在结算阶段调用（如 GameManager.OnGameWin()）
    public void PlayVictorySequence()
    {
        if (_isPlaying) return;
        _isPlaying = true;

        // 确保特效和音效**几乎同时触发**
        fireworkEffect?.Play();
        if (victoryAudio != null && winSound != null)
        {
            victoryAudio.clip = winSound;
            victoryAudio.Play();
        }

        // 可选：延迟重置状态（根据特效时长）
        Invoke(nameof(ResetState), fireworkEffect.main.duration + 0.5f);
    }

    private void ResetState()
    {
        _isPlaying = false;
    }
}