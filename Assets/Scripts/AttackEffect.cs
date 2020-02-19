using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public float effectLength;
    public int soundFx;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlaySFX(soundFx);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, effectLength);
    }
}
