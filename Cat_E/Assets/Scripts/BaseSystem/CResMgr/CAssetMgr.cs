using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAssetMgr : MonoBehaviour
{
    private CAssetMgr m_instance  ; 
    public CAssetMgr Instance{
        get {
                if(m_instance == null )
                    InitCAssetMgr(); 
                return m_instance ; 
            }
    }
    private void InitCAssetMgr()
    {
        this.m_instance = new CAssetMgr() ; 
    }

    public static T Load<T>(string in_szPath) where T : Object
    {
        // // 从 AssetBundle 中加载资源， 并且添加后嘴名字， 不然很难区分 
        // Debug.Log("in_szPath :" + in_szPath );
        return Resources.Load<T>(in_szPath) ; 
    } 
  
}
