using System.Collections;
using UnityEngine;

public class Bug : MonoBehaviour
{
    private int currentHealth;
    private float rewardAmount;
    private BugSpawner spawner;
    private float lifetimeTimer = 0f;
    private bool isDead = false;

    private Vector3 originalScale;

    public void Init(int health, float reward, BugSpawner bugSpawner)
    {
        currentHealth = health;
        rewardAmount = reward;
        spawner = bugSpawner;

        Vector3 currentPos = transform.position;
        currentPos.z = -0.1f;
        transform.position = currentPos;

        Vector3 baseScale = transform.localScale;

        float t = Mathf.InverseLerp(1f, 15f, health);
        float scaleMultiplier = Mathf.Lerp(1.0f, 2.5f, t);
        
        transform.localScale = new Vector3(
            baseScale.x * scaleMultiplier, 
            baseScale.y * scaleMultiplier, 
            baseScale.z
        );
        
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isDead) return;

        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= 5f)
        {
            Die(false); 
        }
    }

    void OnMouseDown()
    {
        if (isDead) return;

        currentHealth--;

        if (currentHealth > 0)
        {
            // Plays hurt sound on click when surviving
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("BugHurt"); 
            }

            StopAllCoroutines();
            StartCoroutine(JiggleRoutine());
        }
        else
        {
            Die(true);
        }
    }

    private void Die(bool squashedByPlayer)
    {
        isDead = true;

        if (squashedByPlayer)
        {
            PlayerManager.instance.playerCash += rewardAmount;
            PlayerManager.instance.playerXP += rewardAmount;
            
            // Added the Squish sound back here so the Spawner doesn't need to handle it
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("Squish");
            }
        }

        // Reverted to zero arguments to clear the CS1501 error
        spawner.OnBugRemoved();
        Destroy(gameObject);
    }

    private IEnumerator JiggleRoutine()
    {
        float timer = 0f;
        float duration = 0.15f;
        float magnitude = 0.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            
            transform.localScale = originalScale * 0.8f;
            transform.localPosition += new Vector3(xOffset, yOffset, 0f);

            yield return null;
            
            transform.localPosition -= new Vector3(xOffset, yOffset, 0f);
        }

        transform.localScale = originalScale;
    }
}