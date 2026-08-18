using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]

    [SerializeField] private AudioMixerGroup _bgmMixerGroup;
    [SerializeField] private AudioMixerGroup _soundEffectMixerGroup;


    [Header("Sound Effects")]
    [SerializeField] private int _soundEffectPoolSize = 20;
    [SerializeField] private float _sameSoundInterval = 0.05f;
    [SerializeField] private int _maxQueuedSoundEffects = 10;
    [SerializeField] private float _pitchVariation = 0.05f;

    [Header("BGM")]
    [SerializeField] private float _bgmFadeDuration = 2f;

    private readonly List<AudioSource> _soundEffectSources = new();
    private readonly Dictionary<AudioSource, float> _soundEffectStartTimes = new();

    // AudioClipごとの未再生回数
    private readonly Dictionary<AudioClip, int> _soundEffectCounts = new();

    // AudioClipごとの再生処理
    private readonly Dictionary<AudioClip, Coroutine> _soundEffectCoroutines = new();

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
        _bgmSource = CreateAudioSource("BGM", _bgmMixerGroup);
    }

    private void InitializeSoundEffectPool()
    {
        for (int i = 0; i < _soundEffectPoolSize; i++)
        {
            _soundEffectSources.Add(CreateAudioSource($"SE{i}", _soundEffectMixerGroup));
        }
    }

    private AudioSource CreateAudioSource(string sourceName, AudioMixerGroup mixerGroup)
    {
        GameObject audioObject = new GameObject(sourceName);
        audioObject.transform.SetParent(transform);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.outputAudioMixerGroup = mixerGroup;

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
            _soundEffectStartTimes.GetValueOrDefault(
                oldestSource,
                float.MaxValue
            );

        foreach (AudioSource source in _soundEffectSources)
        {
            float startTime =
                _soundEffectStartTimes.GetValueOrDefault(
                    source,
                    float.MaxValue
                );

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
            return;
        }

        // 現在の未再生回数を取得
        _soundEffectCounts.TryGetValue(clip, out int count);

        // 最大再生待ち数を超えていたら追加しない
        if (count >= _maxQueuedSoundEffects)
        {
            return;
        }

        // 再生要求を1つ追加
        _soundEffectCounts[clip] = count + 1;

        // すでに再生処理中なら、カウントだけ増やして終了
        if (_soundEffectCoroutines.ContainsKey(clip))
        {
            return;
        }

        // このSEの再生処理を開始
        Coroutine coroutine = StartCoroutine(ProcessSoundEffect(clip));
        _soundEffectCoroutines.Add(clip, coroutine);
    }

    private IEnumerator ProcessSoundEffect(AudioClip clip)
    {
        while (_soundEffectCounts.TryGetValue(clip, out int count) && count > 0)
        {
            AudioSource source = GetAvailableSoundEffectSource();

            source.Stop();

            // 同じSEでも毎回少しだけピッチを変える
            source.pitch = Random.Range(
                1f - _pitchVariation,
                1f + _pitchVariation
            );

            source.PlayOneShot(clip);

            _soundEffectStartTimes[source] = Time.time;

            // 未再生回数を1つ消費
            _soundEffectCounts[clip] = count - 1;

            // 次の再生まで待機
            yield return new WaitForSeconds(_sameSoundInterval);
        }

        _soundEffectCounts.Remove(clip);
        _soundEffectCoroutines.Remove(clip);
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

        foreach (Coroutine coroutine in _soundEffectCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }

        _soundEffectCoroutines.Clear();
        _soundEffectCounts.Clear();
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