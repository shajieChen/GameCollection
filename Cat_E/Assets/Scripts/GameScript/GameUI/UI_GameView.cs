using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI ;
 
public class UI_GameView : MonoBehaviour 
{
   private int m_nLevel ;
   private int m_nCatType; 
   private int m_nCurrentLevelResult  ; 
   public GameObject MiddleCenterGO ; 
   public GameObject ActivityPrefabGO ; 
   public GameObject FoodPrefabGO ; 
   private List<CActivity> m_listActivities ; 
   private List<CFood> m_listFood ; 
   public static List<GameObject> ListActivitiesDisplay = new List<GameObject>(); 
   public static List<GameObject> listFoodDisplay = new List<GameObject>() ; 
   public  GameObject GP_Activity ; 
   public  GameObject GP_Food ; 
    UI_GameView()
    {
       this.m_nLevel = -1 ; 
       this.m_nCatType = -1 ; 
       this.m_nCurrentLevelResult = -1 ; 
    } 
    private void init()
    {
         m_listActivities = new List<CActivity>() ; 
         foreach(var Activity in CGameConfigData.GetConfigDatas<CActivity>())
         {
            m_listActivities.Add(Activity) ; 
         } 
         m_listFood = new List<CFood>(); 
         foreach(var food in CGameConfigData.GetConfigDatas<CFood>())
         {
            m_listFood.Add(food); 
         } 
    }
   public void StartGame(int In_nCatTypeIndex,  int In_nLevel)
   {
      this.m_nCatType = In_nCatTypeIndex  ;
      this.m_nLevel  = In_nLevel ;  
      this.m_nCurrentLevelResult = 0 ;  

      init() ;
      
      ShowMiddleCenter(In_nCatTypeIndex: m_nCatType,In_nLevel:  m_nLevel, In_nCurLevelResult: m_nCurrentLevelResult); // 每一次玩家点击后刷新的东西
      UpdateFoodAndActivity() ;
   } 
    public void ShowMiddleCenter(int In_nCatTypeIndex,  int In_nLevel, int In_nCurLevelResult)
    {  
          if(MiddleCenterGO == null ) 
               Debug.LogError("The middle Center can't been null "); 

         if(In_nCatTypeIndex == -1 || In_nLevel == -1 || In_nCurLevelResult == -1 )
            Debug.LogError("The something is -1 ");  
         foreach(CCatType cat in UI_OverViewMgr.Instance.listCCatTypes)
         { 
            
            if( In_nCatTypeIndex != cat.type|| In_nLevel != cat.nLevel )
                  continue ; 
            // Debug.Log("Update the middle center ");   
            //   Debug.Log("Index : " + In_nCatTypeIndex + "Level :" + In_nLevel + "Result : " + In_nCurLevelResult);
            //      Debug.Log("Cat key :" +(int.Parse(cat.key)%3) + "cat level " + cat.nLevel + "reslut " + cat.nCurrentLevelResult  );
            // Debug.Log("cat imag" + cat.szImgName + "cat key" + cat.key);
             MiddleCenterGO.GetComponent<Image>().sprite = CAssetMgr.Load<Sprite>(cat.szImgName); //Replace Image 
         }

    }
   public void UpdateFoodAndActivity()
   {   
      foreach(var activity in m_listActivities)
      {
         if(activity.nLevel != m_nLevel)
            continue ; 
         GameObject tmpActivityGO = GameObject.Instantiate<GameObject>(ActivityPrefabGO);  //show the activity 
         if(  tmpActivityGO == null ) {Debug.Log("Null Image "); break ;}
         
         tmpActivityGO.GetComponent<Image>().sprite = CAssetMgr.Load<Sprite>(activity.szImgName) ; 
         
         ListActivitiesDisplay.Add(tmpActivityGO) ;   

         tmpActivityGO.GetComponent<Button>().onClick.AddListener(()=>
         {   
            unDisplay();  

            CaclulateResult() ;

            UpdateFoodAndActivity() ;

            ShowMiddleCenter(m_nCatType, m_nLevel, m_nCurrentLevelResult) ; //display the midddle cuenter 
         }) ; 
      } 

      foreach(var food in m_listFood )
      { 
         if(food.nLevel != m_nLevel )
         {  
            continue ; 
         }
            GameObject tmpFoodGO = GameObject.Instantiate<GameObject>(FoodPrefabGO) ; //show the food 
            if( tmpFoodGO == null ) {Debug.Log("Null Image "); break ;}
         
            tmpFoodGO.GetComponent<Image>().sprite = CAssetMgr.Load<Sprite>(food.szImgName) ; 
            listFoodDisplay.Add(tmpFoodGO) ;  

             tmpFoodGO.GetComponent<Button>().onClick.AddListener(()=>
            {  
               unDisplay(); 
               
               CaclulateResult() ; 
               
               UpdateFoodAndActivity() ;

               ShowMiddleCenter(m_nCatType, m_nLevel, m_nCurrentLevelResult); //display the midddle cuenter 
            }) ;  
      }
      Display() ;  
   }
   public void unDisplay() // Clear up all object 
   { 
       foreach(var activity in ListActivitiesDisplay)
      {
         activity.SetActive(false) ; 
         Destroy(activity) ; 
      }  
      foreach(var food in listFoodDisplay)
      {
         food.SetActive(false) ; 
         Destroy(food) ; 
      }
      ListActivitiesDisplay.Clear() ; 
      listFoodDisplay.Clear() ; 

   }
   public void Display()
   {
        foreach(var activity in ListActivitiesDisplay) 
      {
            activity.transform.SetParent(GP_Activity.transform) ;
      }
      foreach(var food in listFoodDisplay)
      {
         food.transform.SetParent(GP_Food.transform) ;
      }
   }  
   public void CaclulateResult() // 更新内部参数
   {
      if(m_nLevel != 5)
         m_nLevel++ ; 
      else
      {
          ShowResult(); 
      }
   }
   public void ShowResult()
   { 
      UI_OverViewMgr.Instance.ShowResult(); 
   }

}