using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager
{
    public class TutorialPopUp
    {
        public int id;
        public GameObject popUpPrefab;
        public bool seen;

        public TutorialPopUp(int _id, GameObject _prefab)
        {
            id = _id;
            popUpPrefab = _prefab;
            seen = false;
        }
    }

    bool isShowingPopUp = false;
    public bool IsShowingPopUp
    {
        get { return isShowingPopUp; }
        set
        {
            if (value == false)
            {
                if (popUpIndexQueue.Count != 0)
                {
                    //there is still popups to show
                    InstantiatePopUp(popUpIndexQueue.Dequeue());
                    isShowingPopUp = true;
                }
                else
                {
                    //no popups to show
                    isShowingPopUp = false;
                    Time.timeScale = 1;
                }
            }
            else
            {
                isShowingPopUp = value;
            }
        }
    }

    List<TutorialPopUp> tutorialPopUps = new List<TutorialPopUp>();
    Queue<int> popUpIndexQueue = new Queue<int>();

    public void OnInit()
    {
        LoadPopUp(0, "ControlsPopUp");
        LoadPopUp(1, "ItemUsePopUp");
        LoadPopUp(2, "RespawnPopUp");
    }

    //finds the popup and add it to queue. instantiate pop up if isShowingPopUp is false
    public void QueuePopUp(int id)
    {
        popUpIndexQueue.Enqueue(FindPopUp(id));

        if (!IsShowingPopUp)
        {
            InstantiatePopUp(popUpIndexQueue.Dequeue());

            IsShowingPopUp = true;
            Time.timeScale = 0;
        }
    }

    int FindPopUp(int id)
    {
        //find popup with matching id
        for (int i = 0; i < tutorialPopUps.Count; i++)
        {
            if (id == tutorialPopUps[i].id)
            {
                return i;
            }
        }

        Debug.LogWarning($"PopUp with id {id} is not found");

        return 0;
    }

    public void InstantiatePopUp(int index)
    {
        TutorialPopUp popUp = tutorialPopUps[index];

        if (popUp != null && !popUp.seen)
        {
            Debug.Log("Spawning pop up");

            GameObject.Instantiate(popUp.popUpPrefab, GameManager.Instance.fadeImage.transform);
            tutorialPopUps[index].seen = true;
        }
    }

    void LoadPopUp(int id, string filePath)
    {
        TutorialPopUp newPopUp = new TutorialPopUp(id, Resources.Load<GameObject>(filePath));

        if (newPopUp.popUpPrefab == null)
        {
            Debug.LogError($"PopUp not found at {filePath}");
        }
        else
        {
            tutorialPopUps.Add(newPopUp);
        }
    }
}
