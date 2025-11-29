using UnityEngine;

public class CardDrawEffect : MonoBehaviour
{
    public ParticleSystem drawVFX;

    public void TriggerEffect(int[] cardIds)
    {
        if (drawVFX != null)
        {
            drawVFX.Play();
        }

        // 可扩展：根据 cardIds 播放不同稀有度特效
    }
}