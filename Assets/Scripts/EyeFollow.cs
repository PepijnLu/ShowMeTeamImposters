using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    // void Start()
    // {
    //     player = GameObject.Find("Player");
    // }

    // Update is called once per frame
    void Update()
    {
        EyeFollowMethod();
    }

    void EyeFollowMethod(){
        Vector3 playerpos = player.transform.position;

        Vector2 direction = new Vector2(
            playerpos.x - transform.position.x,
            playerpos.y - transform.position.y
        );
        transform.up = direction;
        
    }
}
