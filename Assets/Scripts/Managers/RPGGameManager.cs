using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGGameManager : MonoBehaviour
{
    public RPGCameraManager cameraManager;

    public static RPGGameManager sharedInstance = null;

    RPGGameManager gameManager = RPGGameManager.sharedInstance;

    public SpawnPoint playerSpawnPoint;

    private void Awake()
    {
        if(sharedInstance != null && sharedInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
         SetupScene();
    }

    public void SetupScene()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if(playerSpawnPoint != null)
        {
            GameObject player = playerSpawnPoint.SpawnObject();
            cameraManager.virtualCamera.Follow = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
