using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed;

    public string areaTransitionName;

    public static PlayerController instance;

    private Vector3 bottomLeftLimits;
    private Vector3 topRightLimits;


    public Animator myAnim;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;

        if (instance == null)
        {
            instance = this;
        }

        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
     
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) {

            theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"),
           Input.GetAxisRaw("Vertical")) * moveSpeed;

        } else
        {
            theRB.velocity = Vector2.zero;
        }

        myAnim.SetFloat("moveX", theRB.velocity.x);
        myAnim.SetFloat("moveY", theRB.velocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 ||
        Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            if (canMove)
            {
                myAnim.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
            } 
        }

        // Mantém player dentro dos limites
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimits.x, topRightLimits.x),
            Mathf.Clamp(transform.position.y, bottomLeftLimits.y, topRightLimits.y), transform.position.z);

    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        bottomLeftLimits = botLeft + new Vector3(1f, 1f, 0f);
        topRightLimits = topRight + new Vector3(-1f, -1f, 0f);
    }
}
