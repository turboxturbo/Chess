using TMPro;
using UnityEngine;

public class WaveView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;

    private void LateUpdate()
    {
        if (GameManager.Instance == null || waveText == null) return;
        if (GameManager.Instance.IsGameOver) return;
        waveText.text = GameManager.Instance.WaveCount.ToString();
    }
}
