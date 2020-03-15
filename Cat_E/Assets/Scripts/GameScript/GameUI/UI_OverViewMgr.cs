using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ; 
using System; 
using UnityEngine.UI ;
using UnityEngine.SceneManagement;

public class UI_OverViewMgr : MonoBehaviour, ITimerHandler
{
    private UI_Intro GP_IntroView ; 
    private UI_GameView GP_GameView ;  
    
    public GameObject IntroView ; 
    public GameObject GameView ; 
    
    private CTimerAxis m_Timers = new CTimerAxis() ; 
    public GameObject GP_Warning ; 
    public GameObject GP_WarningOne ; 
    private int m_PlayerChoiceIndex ; 
    public List<CCatType> listCCatTypes ; 

    public GameObject GP_EndGame ;

    public GameObject Final_Text ;   
    public int PlayerChoiceIndex{get {return this.m_PlayerChoiceIndex ;}
        set {this.m_PlayerChoiceIndex = value ; }}
     private static UI_OverViewMgr _instance;

    public static UI_OverViewMgr Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        } 
        listCCatTypes = CGameConfigData.GetConfigDatas<CCatType>();
        m_Timers.Create(); 
        //初始化窗口
        this.IntroView.SetActive(true) ; 
        this.GameView.SetActive(false) ; 
        this.GP_Warning.SetActive(false) ; 
        this.GP_EndGame.SetActive(false) ;  
    }
    private void Start() {
        this.GP_GameView = GameView.GetComponent<UI_GameView>() ; 
        this.GP_IntroView = IntroView.GetComponent<UI_Intro>() ; 
        GP_IntroView.onClickCallBack += DisPlayNextPage; 
    }
    public UI_OverViewMgr()
    {
        Init(); 
    } 
        private void Init()
    {
        if(GP_IntroView == null || GP_GameView  == null )
            return ; 
        m_PlayerChoiceIndex =0  ; 
        
    } 
 

    public void DisPlayNextPage()
    {
        if(this.m_PlayerChoiceIndex == 0 )
            { 
                if(GP_Warning != null && GP_WarningOne != null  )
                {
                    GP_Warning.SetActive(true ) ; 
                    GP_WarningOne.SetActive(true  ) ; 
                    m_Timers.AddTimer(1 ,fTime: 2f, this , 1, "") ; 

                }
              return ; 
            }
        else
        { 
            if(IntroView != null && GameView != null ) 
            { 
                        //显示GameView
                        IntroView.SetActive(false) ;
                        GameView.SetActive(true) ;   


                        GP_GameView.StartGame(m_PlayerChoiceIndex, 1) ; 
            }
            else
            {
                    return ; 
            }
        }
    }
  
   void ITimerHandler.OnTimer(int dwEventID)
   {
        if(dwEventID == 1 )
           GP_Warning.SetActive(false) ; 
   }
   private void LateUpdate()
   {
       m_Timers.UpdateTimers(Time.deltaTime) ; 
   }


   public void ShowResult()
   {
       this.GameView.GetComponent<CanvasGroup>().alpha = 1 ;///Close AllPanel 
       this.GameView.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(()
        => {
            GameView.SetActive(false) ; 
        })
        ; 

            this.GP_EndGame.SetActive(true) ; // show EndGame Panel; 
            this.Final_Text.transform.DOLocalMoveY(0, 1).SetEase(Ease.OutBack) ; 
   }
}
