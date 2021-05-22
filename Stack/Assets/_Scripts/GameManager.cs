using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform spawnedObjectsParent;
    public Transform front, back, left, right;
    public Transform currentPlatform;

    public Vector3 leftSpawnT = new Vector3(-12.5f, -2);
    public Vector3 rightSpawnT = new Vector3(0, -2, 12.5f);

    bool newPlatformSide;
    [HideInInspector]
    public bool gameStart, gameOver, pulseEffect, newBest;

    [HideInInspector]
    public int currentScore, bestScore;
    int pulseCount;

    [SerializeField] GameObject platform;
    [SerializeField] GameObject pulse;
    GameObject current;

    #region MONO
    void Awake()
    {
        instance = this;
        bestScore = PlayerPrefs.GetInt("best score");
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && gameStart)
            current.GetComponent<Platform>().Touched();
#endif

#if UNITY_ANDROID
        if(Input.GetMouseButtonDown(0) && gameStart)
            current.GetComponent<Platform>().Touched();
#endif

        if (!pulseEffect)
            pulseCount = 0;
    }
    #endregion

    #region STATE
    public IEnumerator PulseEffect()
    {
        if (pulseCount < 4)
            pulseCount++;

        Vector3 spawnPos = new Vector3(currentPlatform.position.x, currentPlatform.position.y + 1.55f, currentPlatform.position.z);

        for (int i = 0; i < pulseCount; i++)
        {
            if(i == 0)
            {
                GameObject currentPulse = Instantiate(pulse, spawnPos, pulse.transform.rotation);

                GameObject sprite = currentPulse.transform.GetChild(0).gameObject;
                GameObject mask = currentPulse.transform.GetChild(1).gameObject;

                sprite.transform.localScale = new Vector3(currentPlatform.localScale.x + 0.65f, currentPlatform.localScale.z + 0.65f, 1);
                mask.transform.localScale = new Vector3(currentPlatform.localScale.x, currentPlatform.localScale.z, 1);

                sprite.transform.GetComponent<SpriteRenderer>().DOFade(0, 1.25f);

                Destroy(currentPulse, 1.25f);

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                if(currentPlatform.localScale.x > 1.5f && currentPlatform.localScale.z > 1.5f && AudioManager.instance.currentCombo < 5)
                {
                    GameObject currentPulse = Instantiate(pulse, spawnPos, pulse.transform.rotation);

                    GameObject sprite = currentPulse.transform.GetChild(0).gameObject;
                    GameObject mask = currentPulse.transform.GetChild(1).gameObject;

                    sprite.transform.localScale = new Vector3(currentPlatform.localScale.x + 0.65f, currentPlatform.localScale.z + 0.65f, 1);
                    mask.transform.localScale = new Vector3(currentPlatform.localScale.x, currentPlatform.localScale.z, 1);

                    currentPulse.transform.DOScale(2f, 1.35f);
                    sprite.transform.GetComponent<SpriteRenderer>().DOFade(0, 1f);

                    Destroy(currentPulse, 1.35f);

                    yield return new WaitForSeconds(0.135f);
                }
            }
        }
    }

    public void NewPlatform(Transform currentPlatform)
    {
        this.currentPlatform = currentPlatform;

        newPlatformSide = !newPlatformSide;

        if(newPlatformSide)
        {
            rightSpawnT = new Vector3(this.currentPlatform.position.x, rightSpawnT.y, rightSpawnT.z);
            GameObject newPlatform = Instantiate(platform, rightSpawnT, Quaternion.identity);
            current = newPlatform;
            newPlatform.GetComponent<Platform>().direction = true;
            newPlatform.transform.parent = spawnedObjectsParent;
        }
        else
        {
            leftSpawnT = new Vector3(leftSpawnT.x, leftSpawnT.y, this.currentPlatform.position.z);
            GameObject newPlatform = Instantiate(platform, leftSpawnT, Quaternion.identity);
            current = newPlatform;
            newPlatform.GetComponent<Platform>().direction = false;
            newPlatform.transform.parent = spawnedObjectsParent;
        }

        leftSpawnT += Vector3.up;
        rightSpawnT += Vector3.up;

        StartCoroutine(CameraFollow.instance.Follow());

        if (!gameStart)
            gameStart = true;
        else
        {
            currentScore++;
            //if new best
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                PlayerPrefs.SetInt("best score", bestScore);
                if (!newBest)
                    newBest = true;
            }
            UIManager.instance.AddScore();
        }

        ColorManager.instance.changePaletteCount++;
    }

    public void GameOver(Rigidbody rb) 
    {
        rb.isKinematic = false;
        gameStart = false;
        gameOver = true;

        AudioManager.instance.Cut();
        StartCoroutine(UIManager.instance.GameOver());
    }

    public void LoadLevel() => SceneManager.LoadScene(0);
    #endregion
}
