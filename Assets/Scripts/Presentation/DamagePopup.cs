using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float offsetY = 0.4f;
    [SerializeField] private float shakeStrength = 0.02f;
    [SerializeField] private float shakeSpeed = 20f;

    private Transform _follow;

    public static DamagePopup Create(GameObject prefab, Transform follow, int damage)
    {
        Vector3 spawnPos = follow.position + new Vector3(-0.3f, 0.4f, 0f);
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
        DamagePopup popup = go.GetComponent<DamagePopup>();
        popup.Setup(damage, follow);
        return popup;
    }

    private void Setup(int damage, Transform follow)
    {
        text.text = damage.ToString();
        _follow = follow;
        GetComponent<MeshRenderer>().sortingOrder = 100;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;

            Vector3 basePos = _follow != null
                ? _follow.position + new Vector3(-0.3f, 0.25f, 0f)
                : transform.position;

            float shake = Mathf.Sin(elapsed * shakeSpeed) * shakeStrength;
            transform.position = basePos + new Vector3(shake, 0f, 0f);

            yield return null;
        }

        Destroy(gameObject);
    }
}
