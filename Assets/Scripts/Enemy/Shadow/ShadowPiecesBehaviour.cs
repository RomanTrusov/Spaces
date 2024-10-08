using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPiecesBehaviour : MonoBehaviour
{

    public float pushForce;
    public float rotationSpeed;

    public ParticleSystem sparckles;

    //childs array
    GameObject[] children;
    //speed array
    Vector3[] randomSpeeds;
    //rotaion array
    Vector3[] randomRotations;

    private bool isSFXPlayed;

    [Header("SFX")]
    [SerializeField]
    private AudioClip damageAudio;

    // Start is called before the first frame update
    void Start()
    {
        //default froces
        pushForce = 1;
        rotationSpeed = 1;

        //get all child objs
        children = GetAllChildren(gameObject);
        //set random speed and rotations for every child
        randomSpeeds = GetRandomSpeeds(gameObject);
        //get random rotations
        randomRotations = GetRandomRotations(gameObject);

        //activate effect
        sparckles.gameObject.SetActive(true);

        
    }

    // Update is called once per frame
    void Update()
    {

        if (!isSFXPlayed)
        {
            //play sfx
            GetComponent<AudioSource>().PlayOneShot(damageAudio, 0.5f);
            isSFXPlayed = true;
        }

        //move and rotate parts
        ExplodeParts(children, randomSpeeds, randomRotations);
    }


    GameObject[] GetAllChildren(GameObject parent)
    {
        //new obj array
        GameObject[] children = new GameObject[parent.transform.childCount];

        // get every chld obj
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }

        return children;
    }

    Vector3[] GetRandomSpeeds(GameObject parent)
    {
        //new obj array
        Vector3[] speeds = new Vector3[parent.transform.childCount];

        // get rand speed for every child
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            speeds[i] = new Vector3(Random.Range(-1,1) * pushForce, Random.Range(-1, 1) * pushForce, Random.Range(-1, 1) * pushForce);
        }

        return speeds;
    }

    Vector3[] GetRandomRotations(GameObject parent)
    {
        //new obj array
        Vector3[] rotations = new Vector3[parent.transform.childCount];

        // get rand speed for every child
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            rotations[i] = new Vector3(Random.Range(-1, 1) * pushForce, Random.Range(-1, 1) * pushForce, Random.Range(-1, 1) * pushForce);
        }

        return rotations;
    }

    void ExplodeParts(GameObject[] children, Vector3[] speed, Vector3[] rotation)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            //move every parts
            children[i].transform.position += speed[i] * pushForce * Time.deltaTime;

            //rotate every part
            children[i].transform.Rotate(rotation[i] * rotationSpeed);

            //reduce the scale
            children[i].transform.localScale = children[i].transform.localScale * 0.99f;

            if (children[i].transform.localScale.x < 0.05f)
            {
                children[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }

        //check for destroy obj
        if (children[0].transform.localScale.x < 0.1f && !sparckles.gameObject.activeSelf) Destroy(transform.parent.gameObject, 0);


    }

}
