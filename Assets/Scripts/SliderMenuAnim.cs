using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMenuAnim : MonoBehaviour
{
    public GameObject panelMenu;
    public Image sliderButton;
    public Sprite leftSprite;
    public Sprite rightSprite;

    public void ShowHideMenu()
    {
        if (panelMenu != null)
        {
            Animator animator = panelMenu.GetComponent<Animator>();

            if (animator != null)
            {
                bool isOpen = animator.GetBool("show");
                animator.SetBool("show", !isOpen);
                if (isOpen)
                {
                    sliderButton.sprite = rightSprite;

                } 
                else
                {
                    sliderButton.sprite = leftSprite;
                }
            }
        }
    }
}
