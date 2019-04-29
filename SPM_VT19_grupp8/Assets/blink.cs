using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blink : MonoBehaviour
{
    Vector3 startscale;
    // Start is called before the first frame update
    void Start()
    {
        startscale = transform.localScale;
        InvokeRepeating("Blink", 0f, 0.2f);
    }

    void Blink()
    {
        StartCoroutine(blinko());
    }

    // Update is called once per frame
    IEnumerator blinko()
    {
        if(Random.value<0.1f)
        {
            transform.localScale = startscale*0.2f;
        }
        yield return new WaitForSeconds(0.1f);
        transform.localScale = startscale;

    }
}
