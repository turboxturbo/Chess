using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTyper : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip voiceClip;

    [Header("Typing")]
    [SerializeField] private float charDelay = 0.05f;

    [Header("Shake")]
    [SerializeField] private float shakeAmount = 1.2f;

    [Header("Voice")]
    [SerializeField, Range(0.7f, 1f)] private float pitchMin = 0.8f;
    [SerializeField, Range(1f, 1.5f)] private float pitchMax = 1.2f;
    [SerializeField, Min(1)] private int voiceEveryNChars = 1;

    public bool IsTyping { get; private set; }

    public event Action TypewriterFinished;

    private Coroutine _typingCoroutine;
    private Coroutine _shakeCoroutine;

    public void Play(string text)
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);

        textComponent.text = text;
        textComponent.maxVisibleCharacters = 0;
        textComponent.ForceMeshUpdate();

        _typingCoroutine = StartCoroutine(TypeCoroutine());
        _shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    public void Skip()
    {
        if (!IsTyping) return;

        StopCoroutine(_typingCoroutine);
        textComponent.maxVisibleCharacters = textComponent.text.Length;
        IsTyping = false;
        TypewriterFinished?.Invoke();
    }

    private IEnumerator TypeCoroutine()
    {
        IsTyping = true;
        int total = textComponent.text.Length;

        for (int i = 0; i < total; i++)
        {
            textComponent.maxVisibleCharacters = i + 1;

            char c = textComponent.text[i];
            if (c != ' ' && c != '\n' && voiceClip != null && i % voiceEveryNChars == 0)
            {
                audioSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
                audioSource.PlayOneShot(voiceClip);
            }

            float delay = charDelay;
            if (c == ',' || c == ';') delay = charDelay * 4f;
            else if (c == '.' || c == '!' || c == '?') delay = charDelay * 8f;

            yield return new WaitForSecondsRealtime(delay);
        }

        IsTyping = false;
        TypewriterFinished?.Invoke();
    }

    private IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            textComponent.ForceMeshUpdate();
            var meshInfo = textComponent.textInfo.meshInfo;

            for (int c = 0; c < textComponent.textInfo.characterCount; c++)
            {
                var charInfo = textComponent.textInfo.characterInfo[c];
                if (!charInfo.isVisible) continue;

                int matIdx = charInfo.materialReferenceIndex;
                int vertIdx = charInfo.vertexIndex;
                Vector3[] verts = meshInfo[matIdx].vertices;

                Vector2 offset = UnityEngine.Random.insideUnitCircle * shakeAmount;
                for (int v = 0; v < 4; v++)
                    verts[vertIdx + v] += (Vector3)offset;
            }

            for (int i = 0; i < textComponent.textInfo.meshInfo.Length; i++)
            {
                var info = textComponent.textInfo.meshInfo[i];
                info.mesh.vertices = info.vertices;
                textComponent.UpdateGeometry(info.mesh, i);
            }

            yield return null;
        }
    }
}
