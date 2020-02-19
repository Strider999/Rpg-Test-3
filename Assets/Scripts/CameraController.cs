using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Tilemap theMap;

    private Vector3 bottomLeftLimits;
    private Vector3 topRightLimits;

    private float halfHeight;
    private float halfWidth;

    public int musicToPlay;
    private bool musicStarted;

    // Start is called before the first frame update
    void Start()
    {
        //target = PlayerController.instance.transform;

        target = FindObjectOfType<PlayerController>().transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        bottomLeftLimits = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        topRightLimits = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);

        PlayerController.instance.SetBounds(theMap.localBounds.min, theMap.localBounds.max);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x,
            target.position.y, transform.position.z);

        // Mantém a camera dentro dos limites
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimits.x, topRightLimits.x),
            Mathf.Clamp(transform.position.y, bottomLeftLimits.y, topRightLimits.y), transform.position.z);

        if (!musicStarted)
        {
            musicStarted = true;
            AudioManager.instance.PlayBGM(musicToPlay);
        }
    }
}
