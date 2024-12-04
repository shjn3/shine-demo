using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    void Awake()
    {

    }

    void Start()
    {
        SceneTransition.Transition("GameScene");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
