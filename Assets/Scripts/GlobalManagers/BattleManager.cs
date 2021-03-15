using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
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
    public Sprite fullHealthSprite;
    public Sprite emptyHealthSprite;
    public GameObject heartPrefab;
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
    public AudioClip spawnLandSFX;
    
    [Header("Hp UI")]
    public RectTransform hpPanel;
    Image[] healthImages;           //left most health has index 0
    Animator[] healthAnims;

    [Header("Robot Respawn")]
    public Vector2 robotSpawnPos;
    public float robotFallTime;

    [Header("Nut")]
    public GameObject nutPrefab;
    public int nutCache;
    public TextMeshProUGUI nutText;

    [Header("Death UI")]
    public GameObject deathPanelGO;
    public Image imageOverlay;
    public Color fadeColor;

    [Header("Death UI animation")]
    public float dropdownDelay;
    public float dropdownDur;

    public delegate void BMCallback();
    public event BMCallback onRegisterHealthPanel;

    [Space]
    public bool autoSpawnFirst = false;

    bool isDead = false;

    private void Start()
    {
        RegisterHealthPanel();
        CurrentHealth = maxHealth;

        if (autoSpawnFirst)
        {
            StartCoroutine(SpawnFirstRobot(5));
        }
    }

    IEnumerator SpawnFirstRobot(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnRobot();
    }

    void RegisterHealthPanel()
    {
        if (!heartPrefab)
        {
            Debug.LogWarning("No heart prefab found! Please check the hp panel.");
            return;
        }

        if (onRegisterHealthPanel != null)
        {
            onRegisterHealthPanel.Invoke();
        }

        healthImages = new Image[maxHealth];
        healthAnims = new Animator[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject obj;
            Image img;
            Animator anim;

            obj = Instantiate(heartPrefab, hpPanel);
            img = obj.GetComponent<Image>();
            anim = obj.GetComponent<Animator>();

            healthImages[i] = img;
            healthAnims[i] = anim;
        }
    }

    void UpdateHealth()
    {
        float beatRate = (1f - ((float)CurrentHealth / maxHealth)) * maxBeatRate;

        //set fullHealth
        for (int i = 0; i < CurrentHealth; i++)
        {
            healthImages[i].sprite = fullHealthSprite;
            healthAnims[i].SetFloat(healthBeatParam, 1f + beatRate);
        }

        //set emptyHealth
        for (int i = CurrentHealth; i < maxHealth; i++)
        {
            healthImages[i].sprite = emptyHealthSprite;
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

        StartCoroutine(ShowDeathPanel(dropdownDelay));
        //fade out background
        imageOverlay.DOColor(fadeColor, 5);
        //fade out battle sfx
        GameManager.Instance.audioManager.mainMixer.DOSetFloat("Vol_Battle", -80, 3).SetEase(Ease.InExpo);

        Debug.Log($"Ending battle. Player earned {nutCache} nuts!");
        GameManager.Instance.inventoryManager.nuts += nutCache;
    }

    //drops down death panel and set the nut earned
    IEnumerator ShowDeathPanel(float delay)
    {
        yield return new WaitForSeconds(delay);

        deathPanelGO.SetActive(true);
        deathPanelGO.GetComponent<RectTransform>().DOMoveY(GameManager.ScreenSizePixel.y * 1.5f, dropdownDur).From().SetRelative().SetEase(Ease.OutBounce);
        nutText.text = nutCache.ToString();
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

            CameraController.GenerateImpulse(-Vector2.up, 10, 10, 0, 0.6f, 0.9f);
            AudioManager.PlayAudioAtPosition(spawnLandSFX, transform.position, AudioManager.sfxMixerGroup);
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
