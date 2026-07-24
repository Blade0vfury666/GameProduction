using System.Collections;
using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bugPrefab;
    public BoxCollider2D spawnZone;

    [Header("Economy Settings")]
    [Tooltip("Global multiplier for both Cash and XP. 1 = Base, 2 = Double, 0.5 = Half.")]
    public float rewardMultiplier = 1.0f;

    [Header("Warning System")]
    public GameObject warningUI;
    [Tooltip("Add a CanvasGroup to your warning UI and drag it here")]
    public CanvasGroup warningCanvasGroup;
    [Tooltip("Attach an AudioSource with your short looping siren, set 'Loop' to TRUE in Inspector")]
    public AudioSource warningAudio;

    [Header("Overlap Prevention")]
    [Tooltip("Set this to your 'Bug' layer")]
    public LayerMask bugLayer;

    [Header("State (Read Only)")]
    public bool isBursting = false;
    public float waitTimer = 0f;
    public int bugsInPool = 0;
    public int concurrentBugsAlive = 0;

    private int currentCap;
    private float minReward, maxReward;
    private int minHealth, maxHealth;
    
    private bool isWarning = false;
    private Coroutine warningCoroutine;

    void OnEnable() 
    {
        if (PlayerManager.instance != null)
        {
            SetNextInterval();
        }
    }

    void Start()
    {
        SetNextInterval();
    }

    void Update()
    {
        if (PlayerManager.instance == null) return;

        int pLevel = PlayerManager.instance.playerLevel;

        if (pLevel < 2) return; 

        if (!isBursting)
        {
            waitTimer -= Time.deltaTime;

            // Trigger the warning exactly 2 seconds before the burst
            if (waitTimer <= 2f && !isWarning)
            {
                StartWarning();
            }

            if (waitTimer <= 0f)
            {
                TriggerBurst(pLevel);
            }
        }
        else
        {
            if (concurrentBugsAlive < currentCap && bugsInPool > 0)
            {
                SpawnBug();
            }
            else if (bugsInPool <= 0 && concurrentBugsAlive <= 0)
            {
                isBursting = false;
                SetNextInterval();
            }
        }
    }

    private void StartWarning()
    {
        isWarning = true;

        if (warningUI != null) warningUI.SetActive(true);
        if (warningAudio != null) warningAudio.Play();

        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        float totalTimer = 0f;
        float cycleLength = 0.7f; // How long one full pulse takes

        // The UI stays up for exactly 5 seconds
        while (totalTimer < 5f)
        {
            totalTimer += Time.deltaTime;

            // The sound stops exactly after 5 seconds
            if (totalTimer >= 5f && warningAudio != null && warningAudio.isPlaying)
            {
                warningAudio.Stop();
            }

            // Pulsate Math: Visible longer, dips to invisible very quickly
            if (warningCanvasGroup != null)
            {
                float timeInCycle = totalTimer % cycleLength;
                
                if (timeInCycle < 0.5f) 
                {
                    // Stay fully visible for 0.5 seconds
                    warningCanvasGroup.alpha = 1f;
                } 
                else if (timeInCycle < 0.6f) 
                {
                    // Fade out incredibly fast over 0.1 seconds
                    warningCanvasGroup.alpha = Mathf.Lerp(1f, 0f, (timeInCycle - 0.5f) / 0.1f);
                } 
                else 
                {
                    // Fade back in incredibly fast over 0.1 seconds
                    warningCanvasGroup.alpha = Mathf.Lerp(0f, 1f, (timeInCycle - 0.6f) / 0.1f);
                }
            }

            yield return null;
        }

        // Cleanup after 5 seconds
        if (warningUI != null) warningUI.SetActive(false);
    }

    private void TriggerBurst(int level)
    {
        isBursting = true;
        isWarning = false;

        // THREAT LEVEL 1 (Levels 2-5)
        if (level >= 2 && level <= 5)
        {
            bugsInPool = Random.Range(10, 16); 
            currentCap = 4;
            minReward = 100; maxReward = 200; 
            minHealth = 1; maxHealth = 3; // 1 to 3 clicks
        }
        // THREAT LEVEL 2 (Levels 6-10)
        else if (level >= 6 && level <= 10)
        {
            bugsInPool = Random.Range(15, 21);
            currentCap = 5;
            minReward = 150; maxReward = 300;
            minHealth = 2; maxHealth = 4; // 2 to 4 clicks
        }
        // THREAT LEVEL 3 (Levels 11-24)
        else if (level >= 11 && level <= 24)
        {
            bugsInPool = Random.Range(20, 31);
            currentCap = 6;
            minReward = 350; maxReward = 750;
            minHealth = 2; maxHealth = 5; // 2 to 5 clicks
        }
        // THREAT LEVEL 4 (Levels 25-34)
        else if (level >= 25 && level <= 34)
        {
            bugsInPool = Random.Range(25, 41);
            currentCap = 8;
            minReward = 800; maxReward = 1500;
            minHealth = 2; maxHealth = 3; // 2 to 3 clicks
        }
        // THREAT LEVEL 5 (Levels 35-49)
        else if (level >= 35 && level <= 49)
        {
            bugsInPool = Random.Range(30, 51);
            currentCap = 10;
            minReward = 1600; maxReward = 3000;
            minHealth = 1; maxHealth = 2; // 1 to 2 clicks
        }
        // THREAT LEVEL 6 (Levels 50+)
        else 
        {
            bugsInPool = Random.Range(40, 61);
            currentCap = 12;
            minReward = 10000; maxReward = 30000; 
            minHealth = 1; maxHealth = 1; // 1 click
        }
    }

    private void SetNextInterval()
    {
        if (PlayerManager.instance == null) return;

        int level = PlayerManager.instance.playerLevel;
        
        if (level < 2)        waitTimer = 2f;
        else if (level <= 5)  waitTimer = Random.Range(30f, 50f);
        else if (level <= 10) waitTimer = Random.Range(30f, 90f);
        else if (level <= 24) waitTimer = Random.Range(60f, 150f);
        else if (level <= 34) waitTimer = Random.Range(90f, 210f);
        else if (level <= 49) waitTimer = Random.Range(120f, 270f);
        else                  waitTimer = Random.Range(180f, 360f);
    }

    private void SpawnBug()
    {
        int randomHP = Random.Range(minHealth, maxHealth + 1);
        
        // Applies the global reward multiplier to the random base reward
        float randomReward = Random.Range(minReward, maxReward) * rewardMultiplier;

        float t = Mathf.InverseLerp(1f, 15f, randomHP);
        float scaleMultiplier = Mathf.Lerp(1.0f, 2.5f, t);

        float prefabBaseRadius = 0.5f; 
        Collider2D prefabCol = bugPrefab.GetComponent<Collider2D>();
        
        if (prefabCol != null)
        {
            if (prefabCol is BoxCollider2D box)
            {
                prefabBaseRadius = Mathf.Max(box.size.x * bugPrefab.transform.localScale.x, box.size.y * bugPrefab.transform.localScale.y) / 2f;
            }
            else if (prefabCol is CircleCollider2D circle)
            {
                prefabBaseRadius = circle.radius * Mathf.Max(bugPrefab.transform.localScale.x, bugPrefab.transform.localScale.y);
            }
        }

        float requiredClearance = prefabBaseRadius * scaleMultiplier;
        Bounds bounds = spawnZone.bounds;
        Vector3 validSpawnPos = Vector3.zero;
        bool foundValidSpot = false;

        for (int i = 0; i < 30; i++)
        {
            float randomX = Random.Range(bounds.min.x + requiredClearance, bounds.max.x - requiredClearance);
            float randomY = Random.Range(bounds.min.y + requiredClearance, bounds.max.y - requiredClearance);
            Vector2 testPos = new Vector2(randomX, randomY);

            Collider2D hit = Physics2D.OverlapCircle(testPos, requiredClearance, bugLayer);
            
            if (hit == null) 
            {
                validSpawnPos = new Vector3(testPos.x, testPos.y, -0.1f);
                foundValidSpot = true;
                break; 
            }
        }

        if (!foundValidSpot)
        {
            return; 
        }

        GameObject newBugObj = Instantiate(bugPrefab, validSpawnPos, Quaternion.identity, transform);
        Bug newBugScript = newBugObj.GetComponent<Bug>();
        
        newBugScript.Init(randomHP, randomReward, this);

        bugsInPool--;
        concurrentBugsAlive++;
    }

    public void OnBugRemoved()
    {
        concurrentBugsAlive--;
    }

    void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        isBursting = false;
        isWarning = false;
        bugsInPool = 0;
        concurrentBugsAlive = 0;

        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        if (warningUI != null) warningUI.SetActive(false);
        if (warningAudio != null) warningAudio.Stop();
    }
}