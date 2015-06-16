using UnityEngine;
using UnityEngine.UI;

public class EnableEffectOnSkillButton : MonoBehaviour
{

    public float Delta;
    public Image Circle1, Circle2;

    void Start()
    {
        Circle1.fillAmount = 0;
        Circle2.fillAmount = Delta;
    }

    void Update()
    {
        Circle1.fillAmount += Time.deltaTime;
        Circle2.fillAmount += Time.deltaTime;
        if (Circle2.fillAmount >= 1 || Circle1.fillAmount >= 1)
        {
            Circle1.fillAmount = 0;
            Circle2.fillAmount = Delta;
        }
    }
}
