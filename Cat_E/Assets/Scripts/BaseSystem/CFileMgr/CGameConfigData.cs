using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
/*
	主要通过反射获取配置文件里面的信息

 */
public class CGameConfigData
{
        /*
            return the Sub-Class File path 
         */
		protected virtual string getFilePath ()
		{
				return "";
		}
        /*
            贯穿游戏整个声明周期所需的数据库
         */
		static Dictionary<Type,Dictionary<string,CGameConfigData>> m_DicDatas = new Dictionary<Type, Dictionary<string, CGameConfigData>> ();
        //get the data with the key value 
		public static T GetConfigData<T> (string key) where T:CGameConfigData
		{
				Type setT = typeof(T);

				if (!m_DicDatas.ContainsKey (setT)) {
						ReadConfigData<T> ();
				}

				Dictionary<string,CGameConfigData> tmp_DicConfig = m_DicDatas [setT];
 
				if (!tmp_DicConfig.ContainsKey (key)) {
						throw new Exception ("Error:  ");
				}
				return (T)(tmp_DicConfig [key]);
		}
        //get all the Datas 
		public static List<T> GetConfigDatas<T> () where T:CGameConfigData
		{ 
				List<T> returnList = new List<T> ();
                
				Type setT = typeof(T);//获取对象类型 

				if (!m_DicDatas.ContainsKey (setT)) 
				{
						ReadConfigData<T> (); // 如果已经导入进去的时候 直接读取
				}

				Dictionary<string,CGameConfigData> tmp_DicConfig = m_DicDatas [setT]; // dict 前面是Id 后面是对象内容

				foreach (KeyValuePair<string,CGameConfigData> kvp in tmp_DicConfig)  
				{
					returnList.Add ((T)(kvp.Value)); // 将 获取到的CGameConfigData 直接强制转换成 T 类型 再 添加到列表里面
				}

				return returnList;
		}
		/*
			先获取当前文件中有多少 列 --- 代表着多少 属性值
				然后从文件的第三行开始遍历 获取我们所需要的数据
					依据一行的所有数据构建对象

		 */
		static void ReadConfigData<T> () where T:CGameConfigData
		{
				T obj = Activator.CreateInstance<T> ();
				
				if(obj == null){return;}

				string tmp_szFileName = obj.getFilePath ();

				if(tmp_szFileName.ToString().Equals(null)) return ;
				 
				tmp_szFileName = tmp_szFileName.Replace("Assets/Resources/" , "") ; //cut off the unuse prefix
				tmp_szFileName = tmp_szFileName.Replace(".csv", "") ;  

            var szFromFile = Resources.Load<TextAsset>(tmp_szFileName) ;  //reading the file 
 
				CCsvReaderByString tmp_pCsvStringReader = new CCsvReaderByString(szFromFile.text);
 
				Dictionary<string,CGameConfigData> tmp_DicConfig = new Dictionary<string, CGameConfigData> (); // key---ID value---值 pair

				FieldInfo[] tmp_arrayFI = new FieldInfo[tmp_pCsvStringReader.ColCount];//创建用于存储对象属性的数组

				for (int nColX = 1; nColX < tmp_pCsvStringReader.ColCount + 1 ; nColX++) 
				{
					tmp_arrayFI [nColX - 1] = typeof(T).GetField (tmp_pCsvStringReader [1, nColX]); //通过反射将获取的对象的属性
				}

				for (int nRowY = 3; nRowY < tmp_pCsvStringReader.RowCount + 1; nRowY++) 
				{
						T tmp_TGenericObj = Activator.CreateInstance<T> (); // 创建通用类型的对象

						for (int nColX = 0; nColX < tmp_arrayFI.Length; nColX++) 
						{
								string tmp_szFieldValue = tmp_pCsvStringReader [nRowY, nColX + 1]; //获取当前行数 特定的列数上面的信息

								object tmp_TValue = new object (); // 获取特定对象， 然后进行解析 

								switch (tmp_arrayFI [nColX].FieldType.ToString ()) 
								{
									case "System.Int32": // int32
											tmp_TValue = int.Parse (tmp_szFieldValue);
											break;

									case "System.Int64": // int64
											tmp_TValue = long.Parse (tmp_szFieldValue);
											break;

									case "System.String": // string 
											tmp_TValue = tmp_szFieldValue;
											break;
									
									case "System.Single": //float	±1.5 x 10−45 to ±3.4 x 1038	~6-9 digits	4 bytes
											tmp_TValue = float.Parse(tmp_szFieldValue) ; 
											break ; 

									case "System.Double": //double	±5.0 × 10−324 to ±1.7 × 10308	~15-17 digits	8 bytes	System.Double
											tmp_TValue = double.Parse(tmp_szFieldValue) ; 
											break ; 
							
									default:
											Debug.Log ("error data type");
											break;
								}

								tmp_arrayFI [nColX].SetValue (tmp_TGenericObj, tmp_TValue); //设置特定的属性位置 属性值

								if (tmp_arrayFI [nColX].Name == "key" || tmp_arrayFI [nColX].Name == "id") // 如果当前的列表名字叫做id 或者key的或直接添加到里面 ， 作为新成员
								{
									tmp_DicConfig.Add (tmp_TValue.ToString (), tmp_TGenericObj);
								}
						}
				}

				m_DicDatas.Add (typeof(T), tmp_DicConfig);
		}
}
