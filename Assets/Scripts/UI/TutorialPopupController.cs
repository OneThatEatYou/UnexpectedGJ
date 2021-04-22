using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopupController : MonoBehaviour
{
    public GameObject[] slides;

    int curSlide = 0;

    public void NextSlide()
    {
        if (curSlide < slides.Length - 1)
        {
            slides[curSlide].SetActive(false);
            curSlide++;
            slides[curSlide].SetActive(true);
        }
        else
        {
            Destroy(gameObject);
            GameManager.Instance.tutorialManager.IsShowingPopUp = false;
        }
    }
}
