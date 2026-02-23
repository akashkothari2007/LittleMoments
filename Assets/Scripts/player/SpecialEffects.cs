using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpecialEffects : MonoBehaviour
{
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] playlist;
    public int currentTrackIndex = 0;
    public float musicVolume = 1f;
    public float musicFadeSpeed = 0.5f;

    [Header("Day-Night Cycle")]
    public bool night = false;
    public float lightChangeSpeed = 0.1f;
    public Light2D globalLight;

    [Header("Drug Effects")]
    public bool drugged = false;
    public Volume volume;

    public LensDistortion lensDistortion;
    public ChromaticAberration chromaticAberration;
    public FilmGrain filmGrain;

    // NEW
    public ColorAdjustments colorAdjustments;

    public float maxDistortion = 0.2f;
    public float minDistortion = -0.75f;

    public float maxChromaticAberration = 1f;
    public float maxFilmGrain = 1f;

    public float distortionEffectChangeSpeed = 0.1f; // how fast intensity ramps in/out
    public float distortionSpeed = 0.1f;             // how fast the wobble moves
    public float chromaticAberrationEffectChangeSpeed = 0.1f;
    public float filmGrainEffectChangeSpeed = 0.1f;

    // NEW: contrast ramp speed
    public float contrastEffectChangeSpeed = 200f; // units per second (0 -> 100 feels good)

    [Header("Screen Fade")]
    public bool blackScreen = false;
    public float screenFadeSpeed = 4f; // higher = faster fade

    private float currentDistortionAmplitude = 0f;
    private bool goingUp = true;

    void Start()
    {
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out lensDistortion);
            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out filmGrain);

            // NEW
            volume.profile.TryGet(out colorAdjustments);
        }
        else
        {
            Debug.LogWarning("SpecialEffects: Volume/Profile not assigned.");
        }
        currentTrackIndex = Random.Range(0, playlist.Length);
        PlayNext();
    }

    void Update()
    {
        updateTimeOfDay();
        updateDrugEffects();
        updateColorAdjustments();   // NEW
        updateBlackScreenFilter();  // NEW

        updateMusicVolume();
        if (musicSource != null && playlist != null && playlist.Length > 0 && !musicSource.isPlaying)
        {
            PlayNext();
        }
    }

    private void updateMusicVolume()
    {
        if (musicSource == null) return;

        if (musicSource.volume < musicVolume)
        {
            musicSource.volume += Time.deltaTime * musicFadeSpeed;
            if (musicSource.volume > musicVolume) musicSource.volume = musicVolume;
        }
        else if (musicSource.volume > musicVolume)
        {
            musicSource.volume -= Time.deltaTime * musicFadeSpeed;
            if (musicSource.volume < musicVolume) musicSource.volume = musicVolume;
        }
    }

    void PlayNext()
    {
        if (playlist == null || playlist.Length == 0 || musicSource == null) return;

        currentTrackIndex++;
        if (currentTrackIndex >= playlist.Length)
            currentTrackIndex = 0;

        musicSource.clip = playlist[currentTrackIndex];
        musicSource.Play();
    }

    private void updateTimeOfDay()
    {
        if (globalLight == null) return;

        if (night && globalLight.intensity > 0.12f)
            globalLight.intensity -= Time.deltaTime * lightChangeSpeed;
        else if (!night && globalLight.intensity < 1f)
            globalLight.intensity += Time.deltaTime * lightChangeSpeed;
    }

    private void updateDrugEffects()
    {
        if (lensDistortion == null || chromaticAberration == null || filmGrain == null)
            return;

        if (drugged)
        {
            if (currentDistortionAmplitude < maxDistortion)
                currentDistortionAmplitude += Time.deltaTime * distortionEffectChangeSpeed;

            if (chromaticAberration.intensity.value < maxChromaticAberration)
                chromaticAberration.intensity.value += Time.deltaTime * chromaticAberrationEffectChangeSpeed;

            if (filmGrain.intensity.value < maxFilmGrain)
                filmGrain.intensity.value += Time.deltaTime * filmGrainEffectChangeSpeed;

            if (goingUp)
            {
                lensDistortion.intensity.value += Time.deltaTime * distortionSpeed;
                if (lensDistortion.intensity.value >= maxDistortion) goingUp = false;
            }
            else
            {
                lensDistortion.intensity.value -= Time.deltaTime * distortionSpeed;
                if (lensDistortion.intensity.value <= minDistortion) goingUp = true;
            }
        }
        else
        {
            if (currentDistortionAmplitude > 0f)
                currentDistortionAmplitude -= Time.deltaTime * distortionEffectChangeSpeed;
            else
                currentDistortionAmplitude = 0f;

            chromaticAberration.intensity.value = Mathf.MoveTowards(
                chromaticAberration.intensity.value, 0f, Time.deltaTime * chromaticAberrationEffectChangeSpeed
            );

            filmGrain.intensity.value = Mathf.MoveTowards(
                filmGrain.intensity.value, 0f, Time.deltaTime * filmGrainEffectChangeSpeed
            );

            lensDistortion.intensity.value = Mathf.MoveTowards(
                lensDistortion.intensity.value, 0f, Time.deltaTime * distortionSpeed
            );

            if (Mathf.Abs(lensDistortion.intensity.value) < 0.01f)
                lensDistortion.intensity.value = 0f;
        }
    }

    // =========================
    // NEW: Color Adjustments
    // =========================
    private void updateColorAdjustments()
    {
        if (colorAdjustments == null) return;

        float targetContrast = drugged ? 100f : 0f;

        // ColorAdjustments.contrast is a ClampedFloatParameter -> use .value
        colorAdjustments.contrast.value = Mathf.MoveTowards(
            colorAdjustments.contrast.value,
            targetContrast,
            Time.deltaTime * contrastEffectChangeSpeed
        );
    }

    // =========================
    // NEW: "Black screen" via color filter
    // =========================
    private void updateBlackScreenFilter()
    {
        if (colorAdjustments == null) return;

        Color target = blackScreen ? Color.black : Color.white;

        // Smoothly blend current colorFilter toward target
        Color current = colorAdjustments.colorFilter.value;
        colorAdjustments.colorFilter.value = Color.Lerp(
            current,
            target,
            Time.deltaTime * screenFadeSpeed
        );
    }
}