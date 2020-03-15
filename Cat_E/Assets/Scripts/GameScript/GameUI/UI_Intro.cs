using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI ;
using System;
using DG.Tweening ; 

public class UI_Intro : MonoBehaviour, IDisposable
{
    private List<CCatType> m_listCatType ; 
    public GameObject GP_CatTypes ;
    public Action onClickCallBack ; 
    public GameObject Prefab_CatDisplay ; 
    public Button Btn_NextPage ;  

    private List<GameObject> m_ListCats ; // 存储玩家选择的猫咪

    public UI_Intro()
    {
        Init();     
    }
    private void Init()
    { 
        m_ListCats = new List<GameObject>();   
    }
    private void Start() {
        this.m_listCatType = UI_OverViewMgr.Instance.listCCatTypes ; 
         Btn_NextPage.onClick.AddListener(OnClickCallBack) ;  // 初始化对象

        foreach(var catTyp in m_listCatType)//数据加载, 设置基本按钮东西
        { 
            if(catTyp.nLevel!= 1)
                continue ; 

            GameObject prefab =  GameObject.Instantiate<GameObject>(Prefab_CatDisplay);  // intantiate the prefab 
            prefab.transform.SetParent(GP_CatTypes.transform); 

            
            Sprite img_CatImg =  CAssetMgr.Load<Sprite>(catTyp.szImgName) ; // Load the resource , Load the Img and the text 
            if(img_CatImg == null)
                throw new NullReferenceException(); 

            

            if(prefab.GetComponent<Image> () != null ) //换相片
                prefab.GetComponent<Image> ().sprite  = img_CatImg ;
            else
                throw new NullReferenceException();

            if(prefab.GetComponent<Button>() != null )
                prefab.GetComponent<Button>().onClick.AddListener(() => {
                         UI_OverViewMgr.Instance.PlayerChoiceIndex =  int.Parse(catTyp.key) ;   
                        Sequence mySequence = DOTween.Sequence(); 
                        mySequence.Append( prefab.transform.DOLocalMoveY(100f,0.5f).SetEase(Ease.OutQuad)) ; 
                        mySequence.Append( prefab.transform.DOLocalMoveY(0,0.5f).SetEase(Ease.OutBounce)) ;
                  }) ; 
            else
                throw new NullReferenceException(); 

            prefab.SetActive(false) ; //先隐藏
            m_ListCats.Add(prefab) ;  //对点击时间添加回调函数， 
        } 


       ShowGamePreView();
            
    }

    public void ShowGamePreView()
    { 
          foreach (var catTyp in m_ListCats)
          {
              catTyp.GetComponent<CanvasGroup>().alpha = 0 ; 

              catTyp.SetActive(true) ; 

              catTyp.GetComponent<CanvasGroup>().DOFade(1f, 1f); 
              


          }
    }



    private void OnClickCallBack() // 到下一个页面中， 删除当前回调函数
    {
        if(Btn_NextPage != null && onClickCallBack != null )
        {
            if(UI_OverViewMgr.Instance.PlayerChoiceIndex != 0 )
            {
                foreach (var button in m_ListCats)
                {
                    button.GetComponent<Button>().onClick.RemoveAllListeners() ; 
                }
            } 
            onClickCallBack();  
        }
        else
        {
            Debug.Log("Some thing null ");
        } 
    }

 
    // CleanUp the dataType 
   public void Dispose()
   { 
       onClickCallBack = null ; 
       m_listCatType.Clear() ; 

   }
 
}
