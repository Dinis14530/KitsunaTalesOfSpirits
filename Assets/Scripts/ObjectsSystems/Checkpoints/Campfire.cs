using TMPro;
using UnityEngine;

[System.Obsolete]
public class CampfireCheckpoint : MonoBehaviour, IInterectable
{
    private static CampfireCheckpoint activeCheckpoint = null;
    private Animator animator;
    private bool isActive = false;

    [Header("Audio")]
    public AudioSource fireAudioSource; // Fonte de som do fogo
    public AudioClip fireClip; // Som do fogo (loop)
    public float soundRadius = 5f; // Distancia maxima para ouvir

    private Transform playerTransform;

    [Header("UI")]
    public GameObject checkpointPanel;
    public TMP_Text checkpointText;
    public float panelDuration = 2f;

    private Coroutine checkpointPanelCoroutine;
    private PlayerHealth playerHealth;
    private PlayerSave playerSave;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetInactive(); // comeca apagado

        // Configura o AudioSource
        if (fireAudioSource == null)
            fireAudioSource = GetComponent<AudioSource>();

        if (fireAudioSource != null)
        {
            fireAudioSource.clip = fireClip;
            fireAudioSource.loop = true;
            fireAudioSource.playOnAwake = false;
        }

        // Procura o jogador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            playerSave = playerObj.GetComponent<PlayerSave>();
        }

        if (playerSave == null)
            playerSave = FindFirstObjectByType<PlayerSave>();
    }

    private void Start()
    {
        if (checkpointPanel != null)
            checkpointPanel.SetActive(false);

        // Restaura checkpoint do save
        if (playerSave != null && playerSave.currentCheckpoint == gameObject.name)
        {
            ActivateCheckpoint();
        }
    }

    private void Update()
    {
        if (isActive && fireAudioSource != null && playerTransform != null)
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);

            if (distance <= soundRadius)
            {
                if (!fireAudioSource.isPlaying)
                    fireAudioSource.Play();
            }
            else
            {
                if (fireAudioSource.isPlaying)
                    fireAudioSource.Stop();
            }
        }
    }

    public bool CanInteract()
    {
        return !isActive;
    }

    public void Interact()
    {
        if (isActive)
            return;

        if (playerHealth != null)
        {
            playerHealth.SetCheckpoint(transform.position);
            ActivateCheckpoint();
            ShowCheckpointPanel();

            // Auto-save
            if (playerSave != null)
            {
                playerSave.currentCheckpoint = gameObject.name;
                playerSave.SaveGameAsync();
            }

            Debug.Log("Checkpoint ativado e jogo guardado");
        }
    }

    private void ActivateCheckpoint()
    {
        if (activeCheckpoint != null)
            activeCheckpoint.SetInactive();

        isActive = true;
        activeCheckpoint = this;

        if (animator != null)
            animator.SetBool("IsLit", true);
    }

    private void SetInactive()
    {
        isActive = false;
        if (animator != null)
            animator.SetBool("IsLit", false);

        if (fireAudioSource != null && fireAudioSource.isPlaying)
            fireAudioSource.Stop();
    }

    private void ShowCheckpointPanel()
    {
        checkpointPanelCoroutine = TimedPanelHelper.Show(
            this,
            checkpointPanel,
            panelDuration,
            checkpointPanelCoroutine,
            checkpointText,
            "Checkpoint ativado!"
        );
    }

    private void OnDisable()
    {
        TimedPanelHelper.Cleanup(this, checkpointPanel, ref checkpointPanelCoroutine);
    }
}
