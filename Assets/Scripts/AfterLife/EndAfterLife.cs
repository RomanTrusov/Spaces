using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndAfterLife : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            Invoke(nameof(ResetLevel),3);
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene("LVL_NoodleArea");
    }
}
