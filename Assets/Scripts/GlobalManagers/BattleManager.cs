using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Player HP")]
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

    [Header("Respawn")]
    public GameObject playerPrefab;
    public GameObject eggPrefab;
    public Vector2 eggSpawnPos;
    public Vector2 targetLaunchPos;
    public float launchDuration;
    public float launchTorque;
    public float respawnJumpForce;
    
    [Header("Hp UI")]
    public GameObject deathPanelGO;
    Image[] healthImages;           //left most health has index 0
    Animator[] healthAnims;
    RectTransform hpPanel;

    [Header("Robot Respawn")]
    public Vector2 robotSpawnPos;
    public float robotFallTime;

    [Header("Nut")]
    public GameObject nutPrefab;
    public int nutCache;
    public TextMeshProUGUI nutText;

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
            EndBattle();
        }
        else
        {
            SpawnEgg(3);
        }
    }

    void EndBattle()
    {
        Debug.Log("Player is dead");

        deathPanelGO.SetActive(true);
        nutText.text = nutCache.ToString();

        Debug.Log($"Ending battle. Player earned {nutCache} nuts!");
        GameManager.Instance.inventoryManager.nuts += nutCache;
    }

    public void SpawnEgg(float delay)
    {
        StartCoroutine(Spawn());

        IEnumerator Spawn()
        {
            yield return new WaitForSeconds(delay);
            Rigidbody2D eggrb = Instantiate(eggPrefab, eggSpawnPos, Quaternion.identity).GetComponent<Rigidbody2D>();
            Vector2 vel;
            Vector2 dis = targetLaunchPos - eggSpawnPos;
            vel.x = dis.x / launchDuration;
            vel.y = dis.y / launchDuration - 0.5f * eggrb.gravityScale * Physics2D.gravity.y * launchDuration;
            eggrb.velocity = vel;
            eggrb.AddTorque(launchTorque);

            //Debug.Log($"Spawning egg with velocity: {vel}");
        }
    }

    public void RespawnPlayer(Vector2 spawnPos)
    {
        GameObject playerGO = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerGO.GetComponent<Rigidbody2D>().AddForce(respawnJumpForce * Vector2.up, ForceMode2D.Impulse);
        isDead = false;
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

    public void SpawnRobot()
    {
        RobotController robot = RobotBuilder.Instance.GenerateRobot();

        //animate robot spawning
        StartCoroutine(LandRobot(robotFallTime));

        IEnumerator LandRobot(float fallTime)
        {
            Vector2 startPos = robotSpawnPos;
            Vector2 endPos = robot.transform.position;
            robot.transform.position = startPos;
            float t = 0;

            while ((Vector2)robot.transform.position != endPos)
            {
                robot.transform.position = Vector2.Lerp(startPos, endPos, 1 - Mathf.Cos(Mathf.PI / 2 * (t / fallTime)));
                t += Time.deltaTime;
                t = Mathf.Clamp(t, 0, fallTime);
                yield return null;
            }

            //camera shake
            CameraController.GenerateImpulse(-Vector2.up, 10, 10, 0, 0.6f, 0.9f);
        }
    }

    public void DropNuts(Vector2 spawnPos, int nutsDropped)
    {
        float maxNutForce = 10;

        if (nutPrefab)
        {
            for (int i = 0; i < nutsDropped; i++)
            {
                GameObject nut = Instantiate(nutPrefab, spawnPos, Quaternion.identity);
                Rigidbody2D nutrb = nut.GetComponent<Rigidbody2D>();
                nutrb.AddForce(new Vector2(Random.Range(-maxNutForce, maxNutForce), Random.Range(0, maxNutForce)), ForceMode2D.Impulse);
                nut.transform.eulerAngles = new Vector3(0, Random.Range(-180, 180), 0);
            }
        }
        else
        {
            Debug.LogWarning("Nut prefab not assigned");
        }
    }

    //used by retry button
    public void ReloadScene()
    {
        GameManager.Instance.ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    //used by return button
    public void ReturnToMainMenu()
    {
        GameManager.Instance.ChangeScene(0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(robotSpawnPos, 1);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eggSpawnPos, 1);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetLaunchPos, 1);
    }
}
