using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private float fadeSpeed = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    private int _currentTrack;
    private bool _fadingOut;

    private void Start()
    {
        if (tracks.Length == 0) return;
        audioSource.loop = false;
        audioSource.volume = volume;
        PlayTrack(0);
    }

    private void Update()
    {
        if (tracks.Length == 0) return;

        if (!audioSource.isPlaying && !_fadingOut)
            PlayNext();
    }

    private void PlayTrack(int index)
    {
        _currentTrack = index % tracks.Length;
        audioSource.clip = tracks[_currentTrack];
        audioSource.volume = volume;
        audioSource.Play();
    }

    private void PlayNext()
    {
        PlayTrack((_currentTrack + 1) % tracks.Length);
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        audioSource.volume = volume;
    }

    public void Pause() => audioSource.Pause();
    public void Resume() => audioSource.UnPause();
}
