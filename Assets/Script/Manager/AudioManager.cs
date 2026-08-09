using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private int _soundEffectPoolSize = 20;

    [Header("BGM")]
    [SerializeField] private float _bgmFadeDuration = 2f;

    private readonly List<AudioSource> _soundEffectSources = new();
    private readonly Dictionary<AudioSource, float> _soundEffectStartTimes = new();

    private AudioSource _bgmSource;
    private Coroutine _bgmCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSoundEffectPool();
        _bgmSource = CreateAudioSource("BGM");
    }

    private void InitializeSoundEffectPool()
    {
        for (int i = 0; i < _soundEffectPoolSize; i++)
        {
            _soundEffectSources.Add(CreateAudioSource($"SE_{i}"));
        }
    }

    private AudioSource CreateAudioSource(string sourceName)
    {
        GameObject audioObject = new GameObject(sourceName);
        audioObject.transform.SetParent(transform);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        return audioSource;
    }

    private AudioSource GetAvailableSoundEffectSource()
    {
        foreach (AudioSource source in _soundEffectSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return GetOldestSoundEffectSource();
    }

    private AudioSource GetOldestSoundEffectSource()
    {
        AudioSource oldestSource = _soundEffectSources[0];
        float oldestStartTime =
            _soundEffectStartTimes.GetValueOrDefault(oldestSource, float.MaxValue);

        foreach (AudioSource source in _soundEffectSources)
        {
            float startTime =
                _soundEffectStartTimes.GetValueOrDefault(source, float.MaxValue);

            if (startTime < oldestStartTime)
            {
                oldestSource = source;
                oldestStartTime = startTime;
            }
        }

        return oldestSource;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Sound effect clip is null.");
            return;
        }

        AudioSource source = GetAvailableSoundEffectSource();

        source.Stop();
        source.PlayOneShot(clip);

        _soundEffectStartTimes[source] = Time.time;
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("BGM clip is null.");
            return;
        }

        StopBGMTransition();

        _bgmCoroutine = StartCoroutine(SwitchBGM(clip, loop));
    }

    private IEnumerator SwitchBGM(AudioClip clip, bool loop)
    {
        if (_bgmSource.isPlaying)
        {
            yield return FadeOutBGMCoroutine();
        }

        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.volume = 0f;
        _bgmSource.Play();

        yield return FadeInBGMCoroutine();

        _bgmCoroutine = null;
    }

    private IEnumerator FadeInBGMCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _bgmFadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / _bgmFadeDuration;
            _bgmSource.volume = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }

        _bgmSource.volume = 1f;
    }

    private IEnumerator FadeOutBGMCoroutine()
    {
        float startVolume = _bgmSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < _bgmFadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / _bgmFadeDuration;
            _bgmSource.volume = Mathf.Lerp(startVolume, 0f, progress);

            yield return null;
        }

        _bgmSource.Stop();
        _bgmSource.volume = 0f;
    }

    public void StopBGM()
    {
        StopBGMTransition();

        _bgmSource.Stop();
        _bgmSource.volume = 0f;
    }

    public void FadeOutBGM()
    {
        StopBGMTransition();

        _bgmCoroutine = StartCoroutine(FadeOutBGMCoroutine());
    }

    public void StopAllSoundEffects()
    {
        foreach (AudioSource source in _soundEffectSources)
        {
            source.Stop();
        }

        _soundEffectStartTimes.Clear();
    }

    private void StopBGMTransition()
    {
        if (_bgmCoroutine == null)
        {
            return;
        }

        StopCoroutine(_bgmCoroutine);
        _bgmCoroutine = null;
    }
}