using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillsRotationScript : MonoBehaviour
{

    private int _currentIndex = 0;
    public Button[] SkillsButtons;
    public EnableEffectOnSkillButton SkillButtonEffects;
    public Button LeftArrow, RightArrow;

    public void Start()
    {
        if (SkillsButtons != null)
        {
            SkillsButtons[_currentIndex].gameObject.SetActive(true);
        }
    }

    public void DesactivateSkillButtonEffects()
    {
        SkillButtonEffects.Circle1.fillAmount = 0;
        SkillButtonEffects.Circle2.fillAmount = 0;
        SkillButtonEffects.gameObject.SetActive(false);
        LeftArrow.gameObject.SetActive(true);
        RightArrow.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (MyCharacterController.Instance.TransformationManager != null
            && !MyCharacterController.Instance.TransformationManager.IsSuperSaiyan
            && !MyCharacterController.Instance.TransformationManager.Transforming
            && MyCharacterController.Instance.Ki >= MyCharacterController.Instance.TransformationManager.KiRequired)
        {
            for (int i = 0; i < SkillsButtons.Length; i++)
            {
                var btn = SkillsButtons[i];
                if (btn != null && btn.name == "TransformationButton")
                {
                    _currentIndex = i;
                    SkillsButtons[_currentIndex].gameObject.SetActive(true);
                    if (SkillButtonEffects != null && !SkillButtonEffects.gameObject.activeSelf)
                    {
                        SkillButtonEffects.gameObject.SetActive(true);
                    }
                    LeftArrow.gameObject.SetActive(false);
                    RightArrow.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    public void SelectPreviousSkill()
    {
        SkillsButtons[_currentIndex].gameObject.SetActive(false);
        if (_currentIndex - 1 < 0)
        {
            _currentIndex = SkillsButtons.Length - 1;
        }
        else
        {
            --_currentIndex;
        }
        SkillsButtons[_currentIndex].gameObject.SetActive(true);
    }

    public void SelectNextSkill()
    {
        SkillsButtons[_currentIndex].gameObject.SetActive(false);
        if (_currentIndex + 1 > SkillsButtons.Length - 1)
        {
            _currentIndex = 0;
        }
        else
        {
            ++_currentIndex;
        }
        SkillsButtons[_currentIndex].gameObject.SetActive(true);
    }

}
