using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class GlobalSetting{
	//public static string NetAssetsUrl="http://120.24.218.58:8080/resource/netAssets/";
	public static string NetAssetsUrl="file://D:/work/KLSaiche20150512/Assets/GameResources/";
	public static string AssetsBundlesUrl
	{
		get
		{
			return NetAssetsUrl+"AssetBundles/";
		}
	}
	public static string PlatformResourcesUrl
	{
		get
		{
			switch(Application.platform)
			{
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				return AssetsBundlesUrl+"Windows32/";
			// case RuntimePlatform.WindowsWebPlayer:
			// 	return AssetsBundlesUrl+"WebPlayer/";
			case RuntimePlatform.Android:
				return AssetsBundlesUrl+"Android/";
			case RuntimePlatform.IPhonePlayer:
				return AssetsBundlesUrl+"IOS/";
			}
			return AssetsBundlesUrl+"Other/";
		}
	}

	public static string ScenesUrl="Scenes/";
	public static string TextAssetsUrl="TextAssets/";
	public static string PublicGameObjectsUrl="PublicGameObjects/";
	public static string SceneObjectsUrl="SceneObjects/";

	public static string VersionNumBundleName="VersionNum/VersionNum.assetBundle";

	public static string GetSceneBundleNameBySceneName(string sceneName)
	{
		return ScenesUrl+sceneName+".unity3d";
	}

	public static string SceneXmlsUrl
	{
		get
		{
			return TextAssetsUrl+"SceneXmls/";
		}
	}

	public static string GetPublicGameObjectBundleNameByResourceName(string bundleName)
	{
		return PublicGameObjectsUrl+bundleName+".assetBundle";
	}

	public static string GetSceneXmlBundleNameBySceneName(string sceneName)
	{
		return SceneXmlsUrl+sceneName+"Xml.assetBundle";
	}

	public static string GameConfigDatasName
	{
		get
		{
			return TextAssetsUrl+"GameConfigDatas/";
		}
	}

	public static string GetGameConfigBundleNameByConfigName(string bundleName)
	{
		return GameConfigDatasName+bundleName+".assetBundle";
	}

	public static string GetSceneObjectName(string bundleName)
	{
		return SceneObjectsUrl+bundleName+".assetBundle";
	}

	public static string GetTextAssetsName(string bundleName)
	{
		return TextAssetsUrl+bundleName+".assetBundle";
	}
	
	public enum GetAssetType
	{
		PublicGameObject,
		SceneObject,
		CarAvt,
		PetAvt,
		RoleAvt,
		GameConfig,
		Scene,
		SceneXml,
		TextAssets,
		GameConfigDatas,
	}


	public static string ConvertToAssetBundleName(string resourceName,GetAssetType at)
	{
		switch(at)
		{
		case GetAssetType.PublicGameObject:
			return GetPublicGameObjectBundleNameByResourceName(resourceName);
		case GetAssetType.GameConfig:
			return GetGameConfigBundleNameByConfigName(resourceName);
		case GetAssetType.SceneObject:
			return GetSceneObjectName(resourceName);
		case GetAssetType.Scene:
			return GetSceneBundleNameBySceneName(resourceName);
		case GetAssetType.SceneXml:
			return GetSceneXmlBundleNameBySceneName(resourceName);
		case GetAssetType.TextAssets:
			return GetTextAssetsName(resourceName);
		case GetAssetType.GameConfigDatas:
			return GetGameConfigBundleNameByConfigName(resourceName);
		}
		return "";
	}

	public static string ConverToFtpPathByBundlePath(string assetBundleName)
	{
		return PlatformResourcesUrl+assetBundleName;
	}

	public static Dictionary<string,int> versionDic;
	public static void ImportVersionXml(XmlDocument versionXml)
	{
		versionDic=new Dictionary<string, int>();
		XmlElement root=versionXml["VersionNum"];
		foreach(XmlElement xe in root.ChildNodes)
		{
			if(xe.Name=="File")
			{
				versionDic.Add(xe.GetAttribute("FileName"),int.Parse(xe.GetAttribute("Num")));
			}
		}
	}

	public static int GetVersionNum(string assetBundleName)
	{
		if(versionDic==null)
		{
			throw new UnityException("not import versionXml");
		}
		Debug.Log("assetBundleName:"+assetBundleName);
		return versionDic[assetBundleName];
	}

	public static Coroutine StartCoroutine(IEnumerator routine)
	{
		GameObject coroutineObj=GameObject.Find("coroutineInstance");
		if(coroutineObj==null)
		{
			coroutineObj=new GameObject();
			coroutineObj.AddComponent<MonoBehaviour>();
			coroutineObj.name="coroutineInstance";
			GameObject.DontDestroyOnLoad(coroutineObj);
		}
		MonoBehaviour mb=coroutineObj.GetComponent<MonoBehaviour>();
		return mb.StartCoroutine(routine);
	}

	public static string GetNameByWholePath(string path)
	{
		return path.Substring(path.LastIndexOf('/') + 1, path.LastIndexOf('.') - path.LastIndexOf('/') - 1);
	}

	public static System.Type GetLoadTypeByAssetType (GlobalSetting.GetAssetType at)
	{
		switch (at) {
		case GlobalSetting.GetAssetType.SceneObject:
		case GlobalSetting.GetAssetType.PublicGameObject:
			return typeof(GameObject);
		}
		return typeof(UnityEngine.Object);
	}
}
