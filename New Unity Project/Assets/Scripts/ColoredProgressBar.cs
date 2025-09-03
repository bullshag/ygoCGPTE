using UnityEngine;
using UnityEngine.UI;

public class ColoredProgressBar : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private Image manaFill;

    public void SetValue(float hpPercent, float manaPercent)
    {
        if (hpFill != null)
        {
            hpFill.fillAmount = Mathf.Clamp01(hpPercent);
        }
        if (manaFill != null)
        {
            manaFill.fillAmount = Mathf.Clamp01(manaPercent);
        }
    }
}
