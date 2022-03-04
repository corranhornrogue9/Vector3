using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (this.GetComponent<GravityField>().enabled)
        {
            foreach (var light in this.transform.GetComponentsInChildren<Light>())
            {
                light.color = Color.green;
            }
        }
        else
        {
            foreach (var light in this.transform.GetComponentsInChildren<Light>())
            {
                light.color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
