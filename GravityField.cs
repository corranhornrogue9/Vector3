using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    public float fieldExtent = 50;
    public float strength = 9.8f;
    public bool pointField = false;
    public Vector3 extent; 
    
    // Start is called before the first frame update
    void Start()
    {
        if (this.TryGetComponent<MeshFilter>(out var mesh))
        {
            Vector3 objectSize = Vector3.Scale(transform.localScale, mesh.mesh.bounds.extents);
            extent = objectSize;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders;
        
        if (pointField)
        {
            hitColliders = Physics.OverlapSphere(gameObject.transform.position, fieldExtent);
        }
        else if(this.TryGetComponent<MeshFilter>(out var mesh))
        {
            //Debug.Log("veratiung overlapBox" + "height = " + (fieldExtent / 2) + " x extent =  " + extent.x + "z extents = " + extent.z);
            hitColliders = Physics.OverlapBox(gameObject.transform.position + (gameObject.transform.up * (fieldExtent / 2)), new Vector3(extent.x  , fieldExtent /2, extent.z),gameObject.transform.rotation);
        }
        else
        {

            hitColliders = Physics.OverlapBox(gameObject.transform.position + (gameObject.transform.up * fieldExtent), new Vector3(fieldExtent / 2, fieldExtent / 2, fieldExtent / 2), gameObject.transform.rotation);
        }


        foreach(var hitColloders in hitColliders)
        {
            GravityObject go;
            if(hitColloders.gameObject.TryGetComponent<GravityObject>(out go))
            {
                var distnace = (this.gameObject.transform.position - go.gameObject.transform.position).magnitude;
                var direction = (this.gameObject.transform.position - go.gameObject.transform.position).normalized;

                var pullStength = (strength / (fieldExtent / 2)) * (fieldExtent - distnace);
                if (pointField)
                {

                    go.AddPull(direction * pullStength);
                }
                else
                {
                    go.AddPull(-transform.up * strength);
                }
            }
        }

    }
}
