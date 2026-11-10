using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputController : MonoBehaviour
{
    public event Action PlayerMoveCompleted;
    [Header("Shotgun")]
    [SerializeField, Min(1)] private int pelletCount = 5;
    [SerializeField, Min(1)] private int pelletDamage = 1;
    [SerializeField, Range(0, 180)] private float spreadDegrees = 30f;
    [SerializeField, Min(0.1f)] private float shotgunRange = 5f;
    [Header("Ammo UI")]
    [SerializeField] private ShotgunAmmoView ammoView;
    [Header("Piece Info")]
    [SerializeField] private PieceInfoView pieceInfoView;
    private ShotgunAttack _shotgun;
    public ShotgunAmmo Ammo { get; private set; }
    private bool _isAiming;
    private LineRenderer _aimLine;
    private Material _aimMaterial;
    private Board _board;
    private BoardManager _boardManager;
    private BoardView _boardView;
    private Camera _worldCamera;
    private PieceView _selectedPiece;
    private bool _isInitialized;
    private bool _inputEnabled;

    public void Initialize(
        Board board,
        BoardManager boardManager,
        BoardView boardView,
        Camera worldCamera)
    {
        _board = board;
        _boardManager = boardManager;
        _boardView = boardView;
        _worldCamera = worldCamera;

        _isInitialized =
            _board != null &&
            _boardManager != null &&
            _boardView != null &&
            _worldCamera != null;

        _inputEnabled = _isInitialized;
        _shotgun = new ShotgunAttack(pelletCount, pelletDamage, spreadDegrees, shotgunRange);
        Ammo = new ShotgunAmmo();
        if (ammoView == null) ammoView = GetComponent<ShotgunAmmoView>();
        if (ammoView == null) ammoView = gameObject.AddComponent<ShotgunAmmoView>();
        ammoView.Initialize(Ammo);
        CancelAim();

        if (!_isInitialized)
        {
            Debug.LogError(
                "PlayerInputController dependencies are not assigned.");
        }
    }

    public void SetInputEnabled(bool inputEnabled)
    {
        _inputEnabled = inputEnabled;
        if (!inputEnabled) CancelAim();
    }

    private void Update()
    {
        if (!_isInitialized ||
            !_inputEnabled ||
            Mouse.current == null)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelAim();
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (!Ammo.IsLoaded)
            {
                CancelAim();
            }
            else
            {
                if (_selectedPiece != null) ReturnSelectedPiece();
                _isAiming = !_isAiming;
                if (!_isAiming) CancelAim();
            }
        }

        if (_isAiming)
        {
            UpdateAim();
            if (Mouse.current.leftButton.wasPressedThisFrame) TryFire();
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySelectPiece();
        }

        if (_selectedPiece != null &&
            Mouse.current.leftButton.isPressed)
        {
            DragSelectedPiece();
        }

        if (_selectedPiece != null &&
            Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TryReleasePiece();
        }

        UpdatePieceInfo();
    }

    private void UpdatePieceInfo()
    {
        if (pieceInfoView == null) return;

        Vector3 worldPosition = GetMouseWorldPosition();
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            PieceView pieceView = hit.GetComponentInParent<PieceView>();
            if (pieceView == null || pieceView.Piece is BlackKing) continue;

            pieceInfoView.Show(pieceView.Piece);
            return;
        }

        pieceInfoView.Hide();
    }

    private void CancelAim()
    {
        _isAiming = false;
        if (_aimLine != null) _aimLine.enabled = false;
    }

    private bool TryGetAim(out int x, out int y, out Vector2 direction)
    {
        direction = Vector2.zero;
        if (!_board.TryFindPiece<BlackKing>(out x, out y)) return false;
        direction = _boardManager.WorldToBoard(GetMouseWorldPosition()) - new Vector2(x, y);
        return direction.sqrMagnitude > 0.0001f;
    }

    private void UpdateAim()
    {
        if (!TryGetAim(out int x, out int y, out Vector2 direction))
        {
            if (_aimLine != null) _aimLine.enabled = false;
            return;
        }
        if (_aimLine == null)
        {
            var aimObject = new GameObject("Shotgun aim cone");
            aimObject.transform.SetParent(transform, false);
            _aimLine = aimObject.AddComponent<LineRenderer>();
            _aimMaterial = new Material(Shader.Find("Sprites/Default"));
            _aimLine.sharedMaterial = _aimMaterial;
            _aimLine.useWorldSpace = true;
            _aimLine.startWidth = _aimLine.endWidth = 0.035f;
            _aimLine.startColor = _aimLine.endColor = new Color32(192, 40, 36, 204);
            _aimLine.sortingOrder = 100;
            _aimLine.loop = true;
        }
        const int arcSegments = 32;
        _aimLine.positionCount = arcSegments + 2;
        _aimLine.SetPosition(0, _boardManager.BoardToWorld(x, y));
        float angle = Mathf.Atan2(direction.y, direction.x);
        for (int i = 0; i <= arcSegments; i++)
        {
            float rayAngle = angle + (i / (float)arcSegments - 0.5f) * _shotgun.SpreadDegrees * Mathf.Deg2Rad;
            _aimLine.SetPosition(i + 1, _boardManager.BoardToWorld(
                x + Mathf.Cos(rayAngle) * _shotgun.Range,
                y + Mathf.Sin(rayAngle) * _shotgun.Range));
        }
        _aimLine.enabled = true;
    }

    private void TryFire()
    {
        if (!TryGetAim(out int x, out int y, out Vector2 direction)) return;
        if (!Ammo.TryFire())
        {
            Debug.Log("Патронник пуст. Сделайте ход для перезарядки.");
            return;
        }
        var damage = _shotgun.Fire(_board, x, y, direction.x, direction.y);
        _boardView.ApplyShot(damage);
        bool whiteKingKilled = false;
        foreach (var hit in damage)
        {
            Debug.Log($"Shotgun: {hit.Key.GetType().Name} took {hit.Value} damage, HP: {hit.Key.Health}");
            if (hit.Key is WhiteKing && hit.Key.Health == 0)
                whiteKingKilled = true;
        }
        if (whiteKingKilled)
            GameManager.Instance.OnWhiteKingDefeated();
        CancelAim();
        PlayerMoveCompleted?.Invoke();
    }

    private void OnDisable()
    {
        CancelAim();
        if (_selectedPiece != null) ReturnSelectedPiece();
    }

    private void OnDestroy()
    {
        if (_aimMaterial != null) Destroy(_aimMaterial);
    }

    private void TrySelectPiece()
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            PieceView pieceView = hit.GetComponentInParent<PieceView>();

            if (pieceView == null || pieceView.Piece is not BlackKing)
            {
                continue;
            }

            _selectedPiece = pieceView;
            return;
        }
    }

    private void DragSelectedPiece()
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        worldPosition.z = _selectedPiece.transform.position.z;

        _selectedPiece.transform.position = worldPosition;
    }

    private void TryReleasePiece()
    {
        Vector3 worldPosition = GetMouseWorldPosition();

        if (!_boardManager.TryGetBoardPosition(
                worldPosition,
                out int toX,
                out int toY))
        {
            ReturnSelectedPiece();
            return;
        }

        int fromX = _selectedPiece.BoardX;
        int fromY = _selectedPiece.BoardY;

        bool moveSucceeded = _board.TryMovePiece(
            fromX,
            fromY,
            toX,
            toY,
            out ChessPiece capturedPiece);

        if (!moveSucceeded)
        {
            ReturnSelectedPiece();
            return;
        }

        _boardView.ApplyMove(
            _selectedPiece,
            toX,
            toY,
            capturedPiece);

        if (capturedPiece is WhiteKing)
            GameManager.Instance.OnWhiteKingDefeated();

        _selectedPiece = null;
        Ammo.OnKingMoved();
        PlayerMoveCompleted?.Invoke();
    }

    private void ReturnSelectedPiece()
    {
        _boardView.ReturnToCell(_selectedPiece);
        _selectedPiece = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            -_worldCamera.transform.position.z);

        return _worldCamera.ScreenToWorldPoint(screenPosition);
    }
}
