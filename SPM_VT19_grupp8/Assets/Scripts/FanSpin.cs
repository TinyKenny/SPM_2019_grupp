using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSpin : MonoBehaviour
{
  public  float speed = 10;


    void Start()
    {
       
  
    }

    void Update()
    {

        transform.Rotate(Vector3.up * speed * Time.deltaTime);
        
    
    }
}