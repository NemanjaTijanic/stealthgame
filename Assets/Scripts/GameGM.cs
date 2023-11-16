using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameGM : MonoBehaviour
{
    private AudioSource source;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject pauseMenuPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public Button resButton;
    private Animator resumeAnimator;
    public Button optButton;
    private Animator optionAnimator;
    public Button quitButton;
    private Animator quitAnimator;
    private bool pauseMenuActive = false;

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

    [Header("HUD")]
    public GameObject hud;
    public GameObject objectiveDescription;

    [Header("Death")]
    public GameObject mainCamera;
    public GameObject deathCamera;
    public GameObject deathCameraParent;
    public GameObject player;

    [Header("Tutorial")]
    public GameObject tutorialPanel;
    private bool tutorialActive = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        source = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Button animation
        resumeAnimator = resButton.GetComponent<Animator>();
        optionAnimator = resButton.GetComponent<Animator>();
        quitAnimator = resButton.GetComponent<Animator>();

        StartCoroutine(ObjectiveDisplay());
        StartCoroutine(TutorialScreenDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) & pauseMenuActive == false)
        {
            pauseMenuActive = true;
            pauseMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0;
        }

        if(Input.GetKeyDown(KeyCode.Return) && tutorialActive)
        {
            TutorialScreenOff();
        }

        deathCameraParent.transform.position = mainCamera.transform.position;
        deathCameraParent.transform.rotation = mainCamera.transform.rotation;
    }

    public void OnButtonHover()
    {
        source.PlayOneShot(hoverSound);
    }

    public void ButtonClick()
    {
        source.PlayOneShot(clickSound);
    }

    public void ResumeButton()
    {
        pauseMenuActive = false;
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;

        resumeAnimator.SetTrigger("Normal");
        optionAnimator.SetTrigger("Normal");
        quitAnimator.SetTrigger("Normal");
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
        else if (currentVolume == 2)
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
        else if (currentResolution == 2)
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
        else if (currentTexture == 2)
        {
            currentTexture--;
            textureText.text = "Low";
        }
        else if (currentTexture == 3)
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

    public void LoadMainMenu()
    {
        Time.timeScale = 1;

        StartCoroutine(LoadSceneAsync());
    }

    public void VictoryScreen()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenu.SetActive(true);
        pauseMenuPanel.SetActive(false);
        hud.SetActive(false);
        victoryPanel.SetActive(true);

        source.Stop();
    }

    public void DefeatScreen()
    {
        deathCameraParent.SetActive(true);

        Animator deathAnimator = deathCamera.GetComponent<Animator>();
        deathAnimator.SetTrigger("PlayerKilled");

        player.SetActive(false);

        StartCoroutine(DeathScreenDelay());
    }

    public void TutorialScreenOn()
    {
        tutorialPanel.SetActive(true);
        tutorialActive = true;

        Time.timeScale = 0;
    }

    public void TutorialScreenOff()
    {
        tutorialPanel.SetActive(false);
        tutorialActive = false;

        Time.timeScale = 1;
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    IEnumerator ObjectiveDisplay()
    {
        yield return new WaitForSeconds(7);

        objectiveDescription.SetActive(false);
    }

    IEnumerator DeathScreenDelay()
    {
        yield return new WaitForSeconds(2.5f);

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenu.SetActive(true);
        pauseMenuPanel.SetActive(false);
        hud.SetActive(false);
        defeatPanel.SetActive(true);

        source.Stop();
    }

    IEnumerator TutorialScreenDelay()
    {
        yield return new WaitForSeconds(10);

        TutorialScreenOn();
    }
}
