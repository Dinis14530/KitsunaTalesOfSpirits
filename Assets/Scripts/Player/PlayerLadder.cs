using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerLadder : MonoBehaviour
{
    [Header("Ladder")]
    [SerializeField] private Tilemap ladderTilemap;
    [SerializeField] private TileBase[] ladderTiles;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float inputThreshold = 0.1f;

    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody2D rb;
    private PlayerController playerController;
    private readonly HashSet<TileBase> validLadderTiles = new HashSet<TileBase>();
    private float originalGravityScale;
    private bool isClimbing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        originalGravityScale = rb.gravityScale;

        BuildLadderLookup();
    }

    private void OnValidate()
    {
        BuildLadderLookup();
    }

    private void Update()
    {
        if (moveAction == null || moveAction.action == null || ladderTilemap == null)
            return;

        if (playerController != null && (playerController.isDashing || !playerController.canMove || playerController.isInDialogue))
        {
            StopClimbing();
            return;
        }

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float yInput = input.y;

        bool onLadder = IsOnLadderTile(transform.position) || IsOnLadderTile(transform.position + Vector3.down * 0.4f);

        if (onLadder && Mathf.Abs(yInput) > inputThreshold)
        {
            StartClimbing();
        }
        else if (!onLadder)
        {
            StopClimbing();
        }
    }

    private void FixedUpdate()
    {
        if (!isClimbing || moveAction == null || moveAction.action == null)
            return;

        float yInput = moveAction.action.ReadValue<Vector2>().y;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yInput * climbSpeed);

        if (playerController != null)
            playerController.isGrounded = true;
    }

    private bool IsOnLadderTile(Vector3 worldPosition)
    {
        if (ladderTilemap == null)
            return false;

        Vector3Int cell = ladderTilemap.WorldToCell(worldPosition);
        TileBase tile = ladderTilemap.GetTile(cell);

        if (tile == null)
            return false;

        if (validLadderTiles.Count == 0)
            return true;

        return validLadderTiles.Contains(tile);
    }

    private void StartClimbing()
    {
        if (isClimbing)
            return;

        isClimbing = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    private void StopClimbing()
    {
        if (!isClimbing)
            return;

        isClimbing = false;
        rb.gravityScale = originalGravityScale;
    }

    private void BuildLadderLookup()
    {
        validLadderTiles.Clear();

        if (ladderTiles == null)
            return;

        for (int i = 0; i < ladderTiles.Length; i++)
        {
            if (ladderTiles[i] != null)
                validLadderTiles.Add(ladderTiles[i]);
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }
}
