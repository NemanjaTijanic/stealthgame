using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Animator animController;
    public Transform camera;
    private AudioSource audioSource;
    private AudioSource gmAudioSource;

    [Header("General")]
    public float health = 100;
    public float maxHealth = 100;

    [Header("Player Movement")]
    public float speed = 5f;
    public float runSpeed = 5f;
    public float walkSpeed = 2f;
    public bool isWalking;
    public bool isMoving;

    [Header("Player Rotation")]
    public float playerTurnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("HUD")]
    public GameObject stealthIcon;
    public Image healthIcon;
    public TMP_Text distractionNumberText;

    [Header("Sound")]
    public AudioClip runningSound;
    public AudioClip walkingSound;
    public AudioClip outOfAmmoSound;

    [Header("GM")]
    public GameObject gm;
    private GameGM gmScript;

    [Header("Secondary Items")]
    public int distractionMax = 4;
    public int distractionCurrent;
    public GameObject distractionObject;
    public GameObject throwLocation;
    public float throwForce = 20f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animController = GetComponentInChildren<Animator>();
        gmScript = gm.GetComponent<GameGM>();
        audioSource = GetComponent<AudioSource>();
        gmAudioSource = gm.GetComponent<AudioSource>();

        isWalking = false;
        isMoving = false;

        // throw
        distractionCurrent = distractionMax;
        distractionNumberText.text = distractionCurrent.ToString();
    }

    void Update()
    {
        // throw
        if (Input.GetKeyDown(KeyCode.Q))
        { 
            if(distractionCurrent > 0)
            {
                // creating throw object
                Vector3 pos = throwLocation.transform.position;
                GameObject throwObject = Instantiate(distractionObject, pos, transform.rotation);

                // throw the object
                Vector3 dir = transform.forward;
                throwObject.GetComponent<Rigidbody>().AddForce(dir * throwForce, ForceMode.Impulse);

                // reduce number of currently held throwables
                distractionCurrent--;
                distractionNumberText.text = distractionCurrent.ToString();
            }
            else
            {
                // out of rocks msg
                gmAudioSource.PlayOneShot(outOfAmmoSound);
            }
        }

        // walk 
        if (Input.GetKeyDown(KeyCode.C))
        {
            isWalking = !isWalking;

            if (isWalking)
            {
                speed = walkSpeed;
                animController.SetBool("walking", true);

                stealthIcon.SetActive(true);
            }
            else
            {
                speed = runSpeed;
                animController.SetBool("walking", false);

                stealthIcon.SetActive(false);
            }

            audioSource.Stop();
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if(move.magnitude >= 0.1f)
        {
            isMoving = true;
            // player rotation
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, playerTurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // player movement
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * Time.deltaTime * speed);

            // animation
            animController.SetFloat("speed", 100);

            // sound
            if (isWalking && !audioSource.isPlaying)
            {
                audioSource.clip = walkingSound;
                audioSource.Play();
            }      
            else if(!audioSource.isPlaying)
            {
                audioSource.clip = runningSound;
                audioSource.Play();
            }    
        }
        else
        {
            isMoving = false;
            // animation
            animController.SetFloat("speed", 0);
            // sound
            audioSource.Stop();
        }
    }

    public void SetHealth()
    {
        float hp = (0.5f / maxHealth) * health;

        healthIcon.fillAmount = hp;
    }

    public void LostHealth()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Victory Point")
        {
            gmScript.VictoryScreen();
        }
    }
}
