using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider volumeSlider;

    [Header("FMOD Bus")]
    public string masterBusPath = "bus:/Master";
    public string sfxBusPath = "bus:/UI";
    public string musicBusPath = "bus:/World";

    [Header("Screen")]
    public Toggle fullscreenCheckbox;

    private Bus musicBus;
    private Bus sfxBus;
    private Bus masterBus;

    private void Start() {
        //musicBus = RuntimeManager.GetBus(musicBusPath);
        //masterBus = RuntimeManager.GetBus(masterBusPath);
        //sfxBus = RuntimeManager.GetBus(sfxBusPath);
    }

    /* Methods to save, load, and apply settings for:
    - 3 sliders for MusicVolume, SoundVolume, and MainVolume.
    - A dropdown for choosing resolution.
    - A checkbox to toggle fullscreen.
    */
    public void SetVolume(Bus bus, float volume)
    {
        bus.setVolume(Mathf.Clamp01(volume));
    }

    public void SaveSettings() 
    {
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("Sound", soundSlider.value);
        PlayerPrefs.SetFloat("Master", volumeSlider.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenCheckbox.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Music", 0.75f);
        soundSlider.value = PlayerPrefs.GetFloat("Sound", 0.75f);
        volumeSlider.value = PlayerPrefs.GetFloat("Master", 0.75f);
        fullscreenCheckbox.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }

    public void ApplySettings()
    {
        SetVolume(masterBus, volumeSlider.value);
        SetVolume(sfxBus, soundSlider.value);
        SetVolume(musicBus, musicSlider.value);
        Screen.fullScreen = fullscreenCheckbox.isOn;
        SaveSettings();
    }
}