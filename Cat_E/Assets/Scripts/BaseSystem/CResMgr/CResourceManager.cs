// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class CAssetMgr2
// {
// 	private static CAssetMgr instance;

// 	public static CAssetMgr Instance {
// 		get {
// 			if(instance==null)
// 			{
// 				instance=new CAssetMgr();
// 			}
// 			return instance;
// 		}
// 	}

// 	// 已解压的Asset列表 [prefabPath, asset]
// 	private Dictionary<string, Object> dicAsset = new Dictionary<string, Object>();

// 	public Dictionary<string, Object> DicAsset {
// 		get {
// 			return dicAsset;
// 		}
// 	}
//     // 网络资源
// 	// "正在"加载的资源列表 [prefabPath, www]
// 	private Dictionary<string, WWW> dicLoadingReq = new Dictionary<string, WWW>();
	
// 	public Object GetResource(string name)
// 	{
// 		Object obj = null;
// 		if (dicAsset.TryGetValue(name, out obj) == false)
// 		{
// 			Debug.LogWarning("<GetResource Failed> Res not exist, res.Name = " + name);
// 			if (dicLoadingReq.ContainsKey(name))
// 			{
// 				Debug.LogWarning("<GetResource Failed> The res is still loading");
// 			}
// 		}
// 		return obj;
// 	}
	
// 	// name表示prefabPath，eg:Prefab/Pet/ABC
// 	public void LoadAsync(string assetName, GlobalSetting.GetAssetType at)
// 	{
// 		// 如果已经下载，则返回
// 		if (dicAsset.ContainsKey(assetName))
// 			return;
		
// 		// 如果正在下载，则返回
// 		if (dicLoadingReq.ContainsKey(assetName))
// 			return;

// 		// 添加引用
// 		RefAsset(assetName);
// 		// 如果没下载，则开始下载
// 		GlobalSetting.StartCoroutine(AsyncLoadCoroutine(assetName, at));
// 	}
	
// 	private IEnumerator AsyncLoadCoroutine(string assetName, GlobalSetting.GetAssetType at)
// 	{
// 		System.Type type=GlobalSetting.GetLoadTypeByAssetType(at);
// 		string assetBundlePath = GlobalSetting.ConvertToAssetBundleName(assetName,at);
// 		string url = GlobalSetting.ConverToFtpPathByBundlePath(assetBundlePath);
// 		int verNum = GlobalSetting.GetVersionNum(assetBundlePath);
		
// 		Debug.Log("WWW AsyncLoad name = " + assetName + " assetBundleName ="+ assetBundlePath+" url = "+url+" versionNum = " + verNum+" type = "+type.ToString());
// 		if (Caching.IsVersionCached(url, verNum) == false)
// 			Debug.Log("Version Is not Cached, which will download from net!");
		
// 		WWW www = WWW.LoadFromCacheOrDownload(url,verNum);
// 		dicLoadingReq.Add(assetName, www);
// 		while (www.isDone == false)
// 			yield return null;
		
// 		// AssetBundleRequest req = www.assetBundle.LoadAsync(assetName, type);
// 		// while (req.isDone == false)
// 		// 	yield return null;

// 		// Debug.Log("assetType="+req.asset.GetType().Name);

// 		// dicAsset.Add(assetName, req.asset);
// 		dicLoadingReq.Remove(assetName);
// 		www.assetBundle.Unload(false);
// 		www = null;
// 		// Debug.Log("WWW AsyncLoad Finished " + assetBundleName + " versionNum = " + verNum);
// 	}
	
// 	public bool IsResLoading(string name)
// 	{
// 		return dicLoadingReq.ContainsKey(name);
// 	}
	
// 	public bool IsResLoaded(string name)
// 	{
// 		return dicAsset.ContainsKey(name);
// 	}
	
// 	public WWW GetLoadingWWW(string name)
// 	{
// 		WWW www = null;
// 		dicLoadingReq.TryGetValue(name, out www);
// 		return www;
// 	}
	
// 	// 移除Asset资源的引用，name表示prefabPath
// 	public void UnrefAsset(string name)
// 	{
// 		dicAsset.Remove(name);
// 	}
	
// 	public void UnloadUnusedAsset()
// 	{
// //		bool effectNeedUnload = GameApp.GetEffectManager().UnloadAsset();
// //		bool worldNeedUnload = GameApp.GetWorldManager().UnloadAsset();
// //		bool sceneNeedUnload = GameApp.GetSceneManager().UnloadAsset();
// //		if (effectNeedUnload || worldNeedUnload || sceneNeedUnload)
// //		{
// //			Resources.UnloadUnusedAssets();
// //		}
// 	}
	
// 	// 根据资源路径添加资源引用，每个管理器管理自己的引用
// 	private void RefAsset(string name)
// 	{
// //		// 模型之类的
// //		if (name.Contains(GlobalSetting.CharacterPath))
// //			GameApp.GetWorldManager().RefAsset(name);
// //		// 图片之类的
// //		else if (name.Contains(GlobalSetting.TexturePath))
// //			GameApp.GetUIManager().RefPTexture(name);// 特效之类的
// //		else if (name.Contains(GlobalSetting.EffectPath))
// //			GameApp.GetEffectManager().RefAsset(name);
// //		　　　　 ......
// //			　　　　 else
// //				Debug.LogWarning("<Res not ref> name = " + name);
// 	}



//     // 本地获取对象
// 	/// <summary>
// 	/// Load the specified path.
// 	/// 直接在resource中加载资源
// 	/// </summary>
// 	/// <param name="path">Path.</param>
// 	/// <typeparam name="T">The 1st type parameter.</typeparam>
// 	public static T Load<T>(string path) where T:Object
// 	{
// 		return (T)Resources.Load(path,typeof(T));
// 	}


// 	/// <summary>
// 	/// Loads the dynamic.
// 	/// 加载动态资源,加载前需确保资源已载入
// 	/// </summary>
// 	/// <returns>The dynamic.</returns>
// 	/// <param name="assetName">Asset name.</param>
// 	/// <typeparam name="T">The 1st type parameter.</typeparam>
// 	public static T LoadDynamic<T>(string assetName) where T:Object
// 	{
// 		// if(!CAssetMgr.Instance.IsResLoaded(assetName))
// 		// {
// 		// 	throw new UnityException("res \""+assetName+"\" not load over");
// 		// }
// 		return (T)CAssetMgr.Instance.GetResource(assetName);
// 	}
 
// }
