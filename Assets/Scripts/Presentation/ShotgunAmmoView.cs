using UnityEngine;
using UnityEngine.UI;

public sealed class ShotgunAmmoView : MonoBehaviour
{
    [Header("Images in the UI")]
    [SerializeField] private Image chamberImage;
    [Tooltip("Five existing UI Images in reserve order.")]
    [SerializeField] private Image[] shellSlots = new Image[ShotgunAmmo.ReserveCapacity];

    [Header("Shared sprites")]
    [Tooltip("One shell sprite shared by all loaded slots and the chamber.")]
    [SerializeField] private Sprite shellSprite;
    [Tooltip("Optional empty slot sprite. Without it, empty Images are hidden.")]
    [SerializeField] private Sprite emptyShellSprite;

    private ShotgunAmmo _ammo;

    public void Initialize(ShotgunAmmo ammo)
    {
        _ammo = ammo;
        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_ammo == null) return;
        SetSlot(chamberImage, _ammo.IsLoaded);
        for (int i = 0; i < shellSlots.Length; i++)
            SetSlot(shellSlots[i], i < _ammo.Reserve);
    }

    private void SetSlot(Image image, bool loaded)
    {
        if (image == null) return;
        image.sprite = loaded ? shellSprite : emptyShellSprite;
        image.enabled = image.sprite != null;
    }
}
