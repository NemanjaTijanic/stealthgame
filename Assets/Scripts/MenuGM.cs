using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuGM : MonoBehaviour
{
    private AudioSource source;

    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Sound")]
    public TMP_Text soundStatus;

    [Header("Volume")]
    public TMP_Text volumeLine1;
    public TMP_Text volumeLine2;
    public TMP_Text volumeLine3;
    public TMP_Text volumeLine4;
    public TMP_Text volumeLine5;
    private int currentVolume = 5;

    [Header("Subtitles")]
    public TMP_Text subtitlesText;

    [Header("Resolution")]
    public TMP_Text resolutionText;
    private int currentResolution = 5;

    [Header("Texture")]
    public TMP_Text textureText;
    private int currentTexture = 2;

    [Header("Difficulty")]
    public TMP_Text difficultyText;
    private int currentDifficulty = 2;

    [Header("Mission Select")]
    public TMP_Text levelNameText;
    public TMP_Text levelDescText;
    public Image levelImage;
    public Button playMissionButton;
    private int currentMissionSelected = 1;

    [Header("Loading")]
    public TMP_Text loadingStatusText;

    void Start()
    {
        Time.timeScale = 1;

        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void OnButtonHover()
    {
        source.PlayOneShot(hoverSound);
    }

    public void ButtonClick()
    {
        source.PlayOneShot(clickSound);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SoundButtonLeft()
    {
        source.mute = true;
        soundStatus.text = "Off";
    }

    public void SoundButtonRight()
    {
        source.mute = false;
        soundStatus.text = "On";
    }

    public void VolumeButtonLeft()
    {
        if (currentVolume == 1)
            return;
        else if(currentVolume == 2)
        {
            currentVolume--;
            volumeLine2.color = Color.white;
            volumeLine1.color = Color.yellow;
            source.volume = 0;
        }
        else if (currentVolume == 3)
        {
            currentVolume--;
            volumeLine3.color = Color.white;
            volumeLine2.color = Color.yellow;
            source.volume = 0.25f;
        }
        else if (currentVolume == 4)
        {
            currentVolume--;
            volumeLine4.color = Color.white;
            volumeLine3.color = Color.yellow;
            source.volume = 0.50f;
        }
        else if (currentVolume == 5)
        {
            currentVolume--;
            volumeLine5.color = Color.white;
            volumeLine4.color = Color.yellow;
            source.volume = 0.75f;
        }
    }

    public void VolumeButtonRight()
    {
        if (currentVolume == 1)
        {
            currentVolume++;
            volumeLine1.color = Color.white;
            volumeLine2.color = Color.yellow;
            source.volume = 0.25f;
        }
        else if (currentVolume == 2)
        {
            currentVolume++;
            volumeLine2.color = Color.white;
            volumeLine3.color = Color.yellow;
            source.volume = 0.50f;
        }
        else if (currentVolume == 3)
        {
            currentVolume++;
            volumeLine3.color = Color.white;
            volumeLine4.color = Color.yellow;
            source.volume = 0.75f;
        }
        else if (currentVolume == 4)
        {
            currentVolume++;
            volumeLine4.color = Color.white;
            volumeLine5.color = Color.yellow;
            source.volume = 1;
        }
        else if (currentVolume == 5)
            return;
    }

    public void SubtitlesButtonLeft()
    {
        subtitlesText.text = "Off";
    }

    public void SubtitlesButtonRight()
    {
        subtitlesText.text = "On";
    }

    public void ResolutionButtonLeft()
    {
        if (currentResolution == 1)
            return;
        else if(currentResolution == 2)
        {
            currentResolution--;
            resolutionText.text = "800x600";
            Screen.SetResolution(800, 600, true);

        }
        else if (currentResolution == 3)
        {
            currentResolution--;
            resolutionText.text = "1280x1024";
            Screen.SetResolution(1280, 1024, true);
        }
        else if (currentResolution == 4)
        {
            currentResolution--;
            resolutionText.text = "1366x768";
            Screen.SetResolution(1366, 768, true);
        }
        else if (currentResolution == 5)
        {
            currentResolution--;
            resolutionText.text = "1600x900";
            Screen.SetResolution(1600, 900, true);
        }
    }

    public void ResolutionButtonRight()
    {
        if (currentResolution == 1)
        {
            currentResolution++;
            resolutionText.text = "1280x1024";
            Screen.SetResolution(1280, 1024, true);
        }
        else if (currentResolution == 2)
        {
            currentResolution++;
            resolutionText.text = "1366x768";
            Screen.SetResolution(1366, 768, true);
        }
        else if (currentResolution == 3)
        {
            currentResolution++;
            resolutionText.text = "1600x900";
            Screen.SetResolution(1600, 900, true);
        }
        else if (currentResolution == 4)
        {
            currentResolution++;
            resolutionText.text = "1920x1080";
            Screen.SetResolution(1920, 1080, true);
        }
        else if (currentResolution == 5)
            return;
    }

    public void TextureButtonLeft()
    {
        if (currentTexture == 1)
            return;
        else if(currentTexture == 2)
        {
            currentTexture--;
            textureText.text = "Low";
        }
        else if(currentTexture == 3)
        {
            currentTexture--;
            textureText.text = "Medium";
        }
    }

    public void TextureButtonRight()
    {
        if (currentTexture == 1)
        {
            currentTexture++;
            textureText.text = "Medium";
        }
        else if (currentTexture == 2)
        {
            currentTexture++;
            textureText.text = "High";
        }
        else if (currentTexture == 3)
            return;
    }

    public void DifficultyButtonLeft()
    {
        if (currentDifficulty == 1)
            return;
        else if (currentDifficulty == 2)
        {
            currentDifficulty--;
            difficultyText.text = "Easy";
        }
        else if (currentDifficulty == 3)
        {
            currentDifficulty--;
            difficultyText.text = "Normal";
        }
    }

    public void DifficultyButtonRight()
    {
        if (currentDifficulty == 1)
        {
            currentDifficulty++;
            difficultyText.text = "Normal";
        }
        else if (currentDifficulty == 2)
        {
            currentDifficulty++;
            difficultyText.text = "Hard";
        }
        else if (currentDifficulty == 3)
            return;
    }

    public void MissionSelectButtonLeft()
    {
        if (currentMissionSelected == 1)
            return;
        else if(currentMissionSelected == 2)
        {
            currentMissionSelected--;
            levelNameText.text = "Tutorial";
            levelDescText.text = "Fight your way through a ruined city and learn the basics";
            levelImage.color = new Color32(255, 255, 255, 255);
            playMissionButton.interactable = true;

        }
    }

    public void MissionSelectButtonRight()
    {
        if (currentMissionSelected == 1)
        {
            currentMissionSelected++;
            levelNameText.text = "Locked";
            levelDescText.text = "Complete the previous mission to unlock";
            levelImage.color = new Color32(30, 30, 30, 255);
            playMissionButton.interactable = false;
        }
        else if (currentMissionSelected == 2)
            return;
    }

    public void loadTutorial()
    {
        StartCoroutine(LoadSceneDelay());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        while (!operation.isDone)
        {
            float progress = operation.progress * 100;
            loadingStatusText.text = progress.ToString("#") + "%";

            yield return null;
        }
    }

    IEnumerator LoadSceneDelay()
    {
        yield return new WaitForSeconds(5);

        StartCoroutine(LoadSceneAsync());
    }
}

