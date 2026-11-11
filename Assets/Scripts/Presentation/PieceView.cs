using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PieceView : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float dropSpeed = 4f;

    public ChessPiece Piece { get; private set; }
    public int BoardX { get; private set; }
    public int BoardY { get; private set; }
    public bool IsMoving { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    private Coroutine _moveCoroutine;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y, ChessPiece piece, BoardManager boardManager)
    {
        Piece = piece;
        SetBoardPosition(x, y, boardManager);
    }

    public void InitializeDrop(int x, int y, ChessPiece piece, BoardManager boardManager, float delay, float dropHeight)
    {
        Piece = piece;
        DropIn(x, y, boardManager, delay, dropHeight);
    }

    public void SetBoardPosition(int x, int y, BoardManager boardManager)
    {
        BoardX = x;
        BoardY = y;
        transform.position = boardManager.GetWorldPosition(x, y);
    }

    public void DropIn(int x, int y, BoardManager boardManager, float delay, float dropHeight)
    {
        BoardX = x;
        BoardY = y;
        Vector3 target = boardManager.GetWorldPosition(x, y);
        transform.position = target + Vector3.up * dropHeight;
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(DropCoroutine(target, delay));
    }

    private IEnumerator DropCoroutine(Vector3 target, float delay)
    {
        IsMoving = true;
        if (delay > 0f) yield return new WaitForSeconds(delay);
        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, dropSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        IsMoving = false;
    }

    public void AnimateTo(int x, int y, BoardManager boardManager, Action onComplete = null)
    {
        BoardX = x;
        BoardY = y;
        Vector3 target = boardManager.GetWorldPosition(x, y);
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveCoroutine(target, onComplete));
    }

    private IEnumerator MoveCoroutine(Vector3 target, Action onComplete)
    {
        IsMoving = true;
        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        IsMoving = false;
        onComplete?.Invoke();
    }

    public void ShowHitEffect(int damage, GameObject popupPrefab)
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRed());
        if (popupPrefab != null)
            DamagePopup.Create(popupPrefab, transform, damage);
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer.color = new Color32(0xD2, 0x01, 0x14, 0xFF);
        yield return new WaitForSeconds(0.25f);
        SpriteRenderer.color = Color.white;
        _flashCoroutine = null;
    }
}
