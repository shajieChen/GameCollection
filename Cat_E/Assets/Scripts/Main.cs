using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Main : MonoBehaviour
{ 
    CTimerAxis cTimerAxis; 
    // Start is called before the first frame update
    void Start()
    {
        Sprite sImg =  GameObject.Instantiate<Sprite>(CAssetMgr.Load<Sprite>("Timer_解析2"));
         
    }   

}
