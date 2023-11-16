using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionItem : MonoBehaviour
{
    public float soundRadius = 10f;
    public GameObject[] enemyList;
    public List<GameObject> enemyAlarmed;

    public GameObject GM;
    private AudioSource audioSource;
    public AudioClip groundHitSound;

    // Start is called before the first frame update
    void Start()
    {
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        audioSource = GM.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Distraction collided with " + collision.gameObject.name);

        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().HitByDistraction();
        }
        else if (collision.gameObject.tag == "Floor")
        {
            audioSource.PlayOneShot(groundHitSound);
            // Check for all enemies within the noise radius
            foreach(var obj in enemyList)
            {
                float distance = Vector3.Distance(obj.transform.position, transform.position);
                if (distance <= soundRadius)
                {
                    enemyAlarmed.Add(obj);
                }
            }
            
            // Distract those enemies
            foreach(var obj in enemyAlarmed)
            {
                Enemy e = obj.GetComponent<Enemy>();
                e.distractionPosition = transform.position;
                e.isDistracted = true;
            }
        }

        Destroy(gameObject);
    }
}
