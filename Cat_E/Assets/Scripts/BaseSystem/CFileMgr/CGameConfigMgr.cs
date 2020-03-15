using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
#if UNITY_EDITOR
using UnityEditor ;
#endif 
using System.IO;
 
#if UNITY_EDITOR
public class CGameConfigMgr : EditorWindow 
{
    public static string szPath = "/Scripts/CSVObj/" ; 
    private static Object m_OSelectedObj ;   
    private string m_szAllCsvFile   = "";  
    [MenuItem("Tools/Generate_ConfigFile")]
    private static void ShowWindow() 
    {
        var window = GetWindow<CGameConfigMgr>(); 

        window.titleContent = new GUIContent("CGameConfigMgr");
        
        window.Show();
    } 
    
    private void OnGUI() 
    { 
        BeginWindows(); 
        
        GUILayout.BeginVertical() ; 
        
        GUILayout.MinHeight(100); 
        
        GUILayout.MinWidth(100); 
        
        GUILayout.Label("The Genreated Config File Path") ; 

        if(Selection.activeObject !=null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject) as string ; //管理器直接获取类型 
            if(path.Length > 5)
            {
                    if(path.Substring(path.Length - 4, 4) == ".csv")
                    {
                        m_OSelectedObj = Selection.activeObject ; 
                        GUILayout.Label(path) ;
                        m_szAllCsvFile = string.Empty ;  
                    }
            }
            
        }
        else
        { 
            EditorGUILayout.BeginScrollView(Vector2.zero ); 

            GUILayout.Label(m_szAllCsvFile)  ;

            EditorGUILayout.EndScrollView();  
        }

        //The user clicked on the file
        if(GUILayout.Button("Generate"))
        {
            if(m_OSelectedObj != null )
            {
                
                CreateConfigFile(); 
            }
        }

        GUILayout.EndVertical(); 
        EndWindows();
    }
    
    private void OnSelectionChange() 
    {
        Repaint(); 
    }

    public static void CreateConfigFile()
    {
        string sztmpFileName = m_OSelectedObj.name ; 

        string sztmpClassName = sztmpFileName  ; 

        StreamWriter psw = new StreamWriter(Application.dataPath + szPath + "C" + sztmpClassName + ".cs") ; 

        psw.WriteLine("using UnityEngine;\nusing System.Collections;\n"); 
        
        psw.WriteLine("public class C"+ sztmpClassName + " : CGameConfigData") ; 
        
        psw.WriteLine("{") ; 
        
        string filePath = AssetDatabase.GetAssetPath (m_OSelectedObj);
        //writer the file 
        
        CCsvStreamReader pCsvFileReader = new CCsvStreamReader(filePath) ;
        
        for(int nColX = 1 ; nColX < pCsvFileReader.ColCount + 1  ; nColX++ )
        {
            string szFileName=pCsvFileReader[2,nColX];
			string szFileType=pCsvFileReader[1,nColX];
			psw.WriteLine ("\t" + "public " + szFileName + " " + szFileType + ";" + "");
        } 
        // string szFilePath = AssetDatabase.GetAssetPath(m_OSelectedObj) ; 
        
        psw.WriteLine ("\t" + "protected override string getFilePath ()");
		
        psw.WriteLine ("\t" + "{");
		
        filePath.Replace("Assets/Resources/","");
		// filePath.Substring(0,filePath.LastIndexOf('.'));
		psw.WriteLine ("\t\t" + "return " + "\"" + filePath + "\";");
		
        psw.WriteLine ("\t" + "}");

        //Object tostring Method 
        psw.WriteLine("\t"+ "public override string ToString(){\n") ;
        
        string szReturnString = ""  ;  
		
        for(int nColX = 1 ; nColX < pCsvFileReader.ColCount + 1  ; nColX++ )
        {
            string szFileName=pCsvFileReader[2,nColX];
			string szFileType=pCsvFileReader[1,nColX];
            if(nColX != pCsvFileReader.ColCount )
            szReturnString += "\"" + szFileType + "\"" + "+"   + szFileType  + "+" ; 
            else
                szReturnString += "\"" + szFileType + "\"" + "+"   + szFileType ; 
        } 
        
        psw.WriteLine("\t\t" + "return " + szReturnString+ ";\n") ;
        
        psw.WriteLine("\t" + "}") ;

		psw.WriteLine ("}");
		
        psw.Flush ();
		
        psw.Close ();
		
        AssetDatabase.Refresh();
    }
} 
#endif