using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public MultiCharStats stats;
    // Start is called before the first frame update
    void Start()
    {
        stats = this.GetComponent<MultiCharStats>();
    }

    // Update is called once per frame
    void Update()
    {
        stats.MoveWithGround(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        stats.LookDirectionGround(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stats.Jump();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            stats.ToggleGravBoots();
        }
    }
}
