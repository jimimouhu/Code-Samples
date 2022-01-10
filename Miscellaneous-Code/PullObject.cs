using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullObject : MonoBehaviour
{
    public LayerMask moving;
    public float radius;
    public float pullStrength;

    void Update()
    {

        Collider[] suck = Physics.OverlapSphere(transform.position, radius, moving);
        foreach (Collider obj in suck)
        {
            Vector3 original = obj.transform.position;
            if (obj.transform.GetComponent<LStatePatternEnemy>())
            {
                obj.transform.GetComponent<LStatePatternEnemy>().gameObject.transform.position = Vector3.MoveTowards(original, transform.position, pullStrength);
            }else if (obj.GetComponent<Rigidbody>())
            {
                Debug.Log("REVITÄÄN PELAAJAA VOIMALLA: " + ((transform.position - original).normalized * ((pullStrength * 2000 + radius * 10) * (1 / (transform.position - original).magnitude))).magnitude);
                obj.GetComponent<Rigidbody>().AddForce((transform.position - original).normalized * ((pullStrength * 2000 + radius * 10) * (1 / (transform.position - original).magnitude)));
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, 10.0f);
    }
}
