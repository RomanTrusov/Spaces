using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBrokenPartsBehaviour : MonoBehaviour
{
    //for parts throwing
    Vector3 randomDirection;

    [SerializeField] ParticleSystem sparcles;

    void Start()
    {
        //invoke destroy after delay
        Invoke(nameof(DestroyParent),3f);
        // get random direction to throw parts
        randomDirection = new Vector3(Random.Range(-2f,2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));

        //emit sparcles
        ParticleSystem clone3 = Instantiate(sparcles, transform.position, transform.rotation, transform);
        clone3.gameObject.SetActive(true);

    }


    void FixedUpdate()
    {
        // scale down parts
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 4f);
        
        //add force to direction to throw parts in random direction
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().AddForce(randomDirection * 60f, ForceMode.Force);
    }


    private void DestroyParent()
    {
        //destroe parent (DRONE)
        Destroy(gameObject.transform.parent.gameObject);
    }
}
