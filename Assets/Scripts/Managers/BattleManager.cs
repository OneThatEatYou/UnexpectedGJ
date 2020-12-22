using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    #region Singleton
    private static BattleManager instance;
    public static BattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                //check if instance is in scene
                instance = FindObjectOfType<BattleManager>();

                //if (instance == null)
                //{
                //    //spawn instance
                //    GameObject obj = new GameObject("BattleManager");
                //    instance = obj.AddComponent<BattleManager>();
                //    DontDestroyOnLoad(obj);
                //}
            }
            return instance;
        }
    }
    #endregion

    public GameObject playerPrefab;
    public int maxHealth;
    int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }

        set
        {
            currentHealth = value;
            UpdateHealth();
        }
    }
    public string hpPanelTag = "HpPanel";
    public string deathPanelTag = "DeathPanel";
    public Sprite fullHealth;
    public Sprite emptyHealth;
    [Space]
    public string healthBeatParam = "BeatRate";
    public float maxBeatRate = 1f;

    Image[] healthImages;           //left most health has index 0
    Animator[] healthAnims;
    RectTransform hpPanel;
    public GameObject deathPanelGO;

    bool isDead = false;

    private void Awake()
    {
        RegisterHealthPanel();
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    void RegisterHealthPanel()
    {
        hpPanel = GameObject.FindGameObjectWithTag(hpPanelTag).GetComponent<RectTransform>();

        healthImages = new Image[maxHealth];
        healthAnims = new Animator[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            Image img = hpPanel.GetChild(i).GetComponent<Image>();

            if (!img)
            { Debug.LogWarning("Health image not found"); }

            healthImages[i] = img;

            Animator anim = hpPanel.GetChild(i).GetComponent<Animator>();
            if (!anim)
            { Debug.LogWarning("Health animator not found"); }
            healthAnims[i] = anim;
        }
    }

    void UpdateHealth()
    {
        float beatRate = (1f - ((float)CurrentHealth / maxHealth)) * maxBeatRate;

        //set fullHealth
        for (int i = 0; i < CurrentHealth; i++)
        {
            healthImages[i].sprite = fullHealth;
            healthAnims[i].SetFloat(healthBeatParam, 1f + beatRate);
        }

        //set emptyHealth
        for (int i = CurrentHealth; i < maxHealth; i++)
        {
            healthImages[i].sprite = emptyHealth;
            healthAnims[i].enabled = false;
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead)
        { return; }

        isDead = true;
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnPlayer();
        }
    }

    void GameOver()
    {
        Debug.Log("Player is dead");

        deathPanelGO.SetActive(true);

        GameManager.Instance.canRestart = true;
    }

    //public reference needs to be removed after debugging
    public void RespawnPlayer()
    {
        Debug.Log("Respawning");

        StartCoroutine(Respawn());

        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(3);
            Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
            isDead = false;
        }
    }

    public void PlaySlowMo(float slowIn, float slowStay, float slowOut, float slowTimeScale)
    {
        if (Time.timeScale != 1)
        { return; }

        StartCoroutine(StartSlowMo());

        IEnumerator StartSlowMo()
        {
            float t = 0;
            float tScale = Time.timeScale;

            while (t < slowIn)
            {
                //decrease time scale
                float p = Mathf.Sin(Mathf.PI / 2 * t / slowIn);
                Time.timeScale = Mathf.Lerp(tScale, slowTimeScale, p);
                t += Time.unscaledDeltaTime;
                t = Mathf.Clamp(t, 0, slowIn);

                yield return null;
            }
            Time.timeScale = slowTimeScale;

            yield return new WaitForSecondsRealtime(slowStay);

            t = 0;
            while (t < slowOut)
            {
                //increase time scale
                float p = Mathf.Sin(Mathf.PI / 2 * t / slowOut);
                Time.timeScale = Mathf.Lerp(slowTimeScale, 1, p);
                t += Time.unscaledDeltaTime;
                t = Mathf.Clamp(t, 0, slowOut);

                yield return null;
            }
            Time.timeScale = 1;
        }
    }

    /*
    public void SpawnRobot()
    {
        RobotController robot = RobotBuilder.Instance.GenerateRobot();

        Vector2 pos;
        foreach (Head head in robot.heads)
        {
            head.
        }
    }
    */
}
