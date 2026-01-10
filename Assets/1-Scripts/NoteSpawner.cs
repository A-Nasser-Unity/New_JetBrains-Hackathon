using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note Prefabs")]
    public GameObject greenNotePrefab;
    public GameObject blueNotePrefab;
    public GameObject yellowNotePrefab;
    public GameObject redNotePrefab;

    [Header("Lane Positions (X axis)")]
    public float greenLaneX = 2.5f;
    public float yellowLaneX = 1f;
    public float blueLaneX = -1f;
    public float redLaneX = -2.5f;

    [Header("Spawn Settings")]
    public float spawnZ = 50f;
    public float spawnInterval = 1f;
    public float noteSpeed = 15f;

    [Header("Difficulty – Spawn Interval")]
    public float spawnIntervalDecreaseRate = 0.02f;
    public float minSpawnInterval = 0.3f;

    [Header("Difficulty – Note Speed")]
    public float noteSpeedIncreaseRate = 0.5f;
    public float maxNoteSpeed = 35f;

    [Header("Difficulty – Multi-Spawn")]
    public bool enableDoubleSpawn = false;
    public float doubleSpawnStartTime = 30f;
    [Range(0f, 1f)]
    public float doubleSpawnChance = 0.5f;

    [Header("Pause/Rest Settings")]
    public bool enablePauseFeature = false;
    [Tooltip("Spawn notes for this many seconds before pausing")]
    public float pauseAfterTime = 30f;
    [Tooltip("Pause duration to give player rest time")]
    public float pauseDuration = 5f;

    private float spawnTimer;
    private float activeSpawnTimer = 0f;
    private float pauseTimer = 0f;
    private bool isPaused = false;

    void Update()
    {
        if (!GameManager.instance.raceStarted || GameManager.instance.raceEnded)
            return;

        // Handle pause feature
        if (enablePauseFeature)
        {
            if (isPaused)
            {
                pauseTimer += Time.deltaTime;
                if (pauseTimer >= pauseDuration)
                {
                    // End pause, resume spawning
                    isPaused = false;
                    pauseTimer = 0f;
                    activeSpawnTimer = 0f;
                }
                return; // Don't spawn while paused
            }
            else
            {
                activeSpawnTimer += Time.deltaTime;
                if (activeSpawnTimer >= pauseAfterTime)
                {
                    // Start pause
                    isPaused = true;
                    pauseTimer = 0f;
                    return;
                }
            }
        }

        // Decrease spawn interval over time
        if (spawnInterval > minSpawnInterval)
            spawnInterval -= spawnIntervalDecreaseRate * Time.deltaTime;

        // Increase note speed over time
        if (noteSpeed < maxNoteSpeed)
            noteSpeed += noteSpeedIncreaseRate * Time.deltaTime;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;

            bool doDoubleSpawn = enableDoubleSpawn && Time.timeSinceLevelLoad >= doubleSpawnStartTime && Random.value < doubleSpawnChance;

            // Spawn notes
            SpawnRandomNote();
            if (doDoubleSpawn)
                SpawnRandomNote(); // second note
        }
    }

    void SpawnRandomNote()
    {
        int randomNote = Random.Range(0, 4);
        GameObject notePrefab = null;
        float laneX = 0f;

        switch (randomNote)
        {
            case 0:
                notePrefab = greenNotePrefab;
                laneX = greenLaneX;
                break;
            case 1:
                notePrefab = blueNotePrefab;
                laneX = blueLaneX;
                break;
            case 2:
                notePrefab = yellowNotePrefab;
                laneX = yellowLaneX;
                break;
            case 3:
                notePrefab = redNotePrefab;
                laneX = redLaneX;
                break;
        }

        if (notePrefab != null)
        {
            GameObject note = Instantiate(notePrefab);
            note.transform.position = new Vector3(laneX, -0.3f, spawnZ);

            Note noteScript = note.GetComponent<Note>();
            if (noteScript != null)
            {
                noteScript.moveSpeed = noteSpeed;
            }
        }
    }
}