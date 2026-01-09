using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scores")]
    public int playerScore = 0;
    public int aiScore = 0;
    public int playerCombo = 0;

    [Header("Combo Settings")]
    public int comboStartThreshold = 5;
    public float combo10Multiplier = 1.5f;
    public float combo20Multiplier = 2f;
    public float combo30Multiplier = 3f;
    public string comboTextPrefix = "COMBO: ";

    [Header("Miss Penalty Settings")]
    public bool enableMissPenalty = true;
    public float missPenaltyAmount = 5f;

    [Header("UI")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI aiScoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI timerText;

    [Header("Hit Feedback UI")]
    public GameObject perfectUI;
    public GameObject goodUI;
    public GameObject missUI;
    public float feedbackDisplayTime = 0.5f;

    [Header("Cars")]
    public Transform playerCar;
    public Transform aiCar;

    [Header("Car Movement Settings")]
    public float maxScoreDiff = 200f;
    public float carMoveSpeed = 3f;
    public float minZOffset = -1f;
    public float maxZOffset = 1f;

    [Header("Race Settings")]
    public float raceTime = 60f;
    private float timer;
    public bool raceStarted = false;
    public bool raceEnded = false;
    private float feedbackTimer = 0f;

    [Header("AI Settings")]
    public float aiHitChance = 0.75f;
    public float aiScorePerHit = 10f;
    public float aiDecisionInterval = 0.4f;
    public float aiStartDelay = 3f;
    private float aiTimer = 0f;
    private float aiDelayTimer = 0f;
    private bool aiCanScore = false;

    [Header("Finish Line")]
    public GameObject finishLinePrefab;
    private GameObject finishLineInstance;
    public float finishLineStartZ = 50f;
    public float finishLineMoveSpeed = 15f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timer = raceTime;
        raceStarted = true;
        aiDelayTimer = aiStartDelay;
        UpdateScoreUI();
        UpdateComboUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!raceStarted || raceEnded) return;

        HandleTimer();
        UpdateCarPositions();
        HandleAIDelay();
        HandleAI();
        HandleFeedbackTimer();
    }

    void HandleTimer()
    {
        timer -= Time.deltaTime;
        UpdateTimerUI();

        if (timer <= 0f && !raceEnded)
        {
            timer = 0f;
            SpawnFinishLine();
        }
    }

    void SpawnFinishLine()
    {
        raceEnded = true;
        finishLineInstance = Instantiate(finishLinePrefab);
        finishLineInstance.transform.position = new Vector3(0, 0, finishLineStartZ);
        StartCoroutine(MoveFinishLine());
    }

    System.Collections.IEnumerator MoveFinishLine()
    {
        while (finishLineInstance.transform.position.z > -5f)
        {
            finishLineInstance.transform.position -= new Vector3(0, 0, finishLineMoveSpeed * Time.deltaTime);
            yield return null;
        }
        EndRace();
    }

    void EndRace()
    {
        raceEnded = true;
        string winner = "Tie";
        if (playerScore > aiScore) winner = "Player Wins!";
        else if (aiScore > playerScore) winner = "AI Wins!";

    }

    void HandleAIDelay()
    {
        if (!aiCanScore)
        {
            aiDelayTimer -= Time.deltaTime;
            if (aiDelayTimer <= 0f)
            {
                aiCanScore = true;
            }
        }
    }

    void HandleAI()
    {
        if (!aiCanScore) return;

        aiTimer -= Time.deltaTime;
        if (aiTimer <= 0f)
        {
            aiTimer = aiDecisionInterval;
            if (Random.value <= aiHitChance)
            {
                aiScore += (int)aiScorePerHit;
                UpdateScoreUI();
            }
        }
    }

    void UpdateCarPositions()
    {
        float diff = playerScore - aiScore;
        float normalizedDiff = Mathf.Clamp(diff / maxScoreDiff, -1f, 1f);

        float playerTargetZ = Mathf.Lerp(0f, maxZOffset, Mathf.Max(0f, normalizedDiff));
        playerTargetZ += Mathf.Lerp(0f, minZOffset, Mathf.Max(0f, -normalizedDiff));

        float aiTargetZ = Mathf.Lerp(0f, maxZOffset, Mathf.Max(0f, -normalizedDiff));
        aiTargetZ += Mathf.Lerp(0f, minZOffset, Mathf.Max(0f, normalizedDiff));

        Vector3 p = playerCar.localPosition;
        Vector3 a = aiCar.localPosition;

        p.z = Mathf.Lerp(p.z, playerTargetZ, Time.deltaTime * carMoveSpeed);
        a.z = Mathf.Lerp(a.z, aiTargetZ, Time.deltaTime * carMoveSpeed);

        playerCar.localPosition = p;
        aiCar.localPosition = a;
    }

    void UpdateScoreUI()
    {
        if (playerScoreText != null)
            playerScoreText.text = playerScore.ToString();

        if (aiScoreText != null)
            aiScoreText.text = aiScore.ToString();
    }

    void UpdateComboUI()
    {
        if (comboText != null)
        {
            if (playerCombo >= comboStartThreshold)
            {
                float multiplier = GetComboMultiplier();
                comboText.text = $"{comboTextPrefix}{playerCombo} (x{multiplier})";
            }
            else
            {
                comboText.text = "";
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void HandleFeedbackTimer()
    {
        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f)
            {
                if (perfectUI != null) perfectUI.SetActive(false);
                if (goodUI != null) goodUI.SetActive(false);
                if (missUI != null) missUI.SetActive(false);
            }
        }
    }

    public void ShowHitFeedback(string feedback)
    {
        if (perfectUI != null) perfectUI.SetActive(false);
        if (goodUI != null) goodUI.SetActive(false);
        if (missUI != null) missUI.SetActive(false);

        if (feedback == "Perfect" && perfectUI != null)
        {
            perfectUI.SetActive(true);
            feedbackTimer = feedbackDisplayTime;
        }
        else if (feedback == "Good" && goodUI != null)
        {
            goodUI.SetActive(true);
            feedbackTimer = feedbackDisplayTime;
        }
        else if (feedback == "Miss" && missUI != null)
        {
            missUI.SetActive(true);
            feedbackTimer = feedbackDisplayTime;
        }
    }

    // ✅ UPDATED METHOD
    public void AddPlayerScore(int amount, string hitQuality)
    {
        if (hitQuality == "Perfect")
        {
            playerCombo++;

            if (playerCombo >= comboStartThreshold)
            {
                float multiplier = GetComboMultiplier();
                amount = Mathf.RoundToInt(amount * multiplier);
            }

            playerScore += amount;
        }
        else if (hitQuality == "Good")
        {
            ResetCombo();
            playerScore += amount;
        }
        else if (hitQuality == "Miss")
        {
            ResetCombo();

            if (enableMissPenalty)
            {
                playerScore -= Mathf.RoundToInt(missPenaltyAmount);
                playerScore = Mathf.Max(0, playerScore); // clamp
            }
        }

        UpdateScoreUI();
        UpdateComboUI();
    }

    float GetComboMultiplier()
    {
        if (playerCombo >= 30) return combo30Multiplier;
        if (playerCombo >= 20) return combo20Multiplier;
        if (playerCombo >= 10) return combo10Multiplier;
        return 1f;
    }

    public void ResetCombo()
    {
        playerCombo = 0;
        UpdateComboUI();
    }
}
