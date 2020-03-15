using UnityEngine;
using System.Collections;

public class CCatType : CGameConfigData
{
	public string key;
	public string szImgName;
	public int nLevel;
	public int type;
	protected override string getFilePath ()
	{
		return "Assets/Resources/CSVFileCollection/CatType.csv";
	}
	public override string ToString(){

		return "key"+key+"szImgName"+szImgName+"nLevel"+nLevel+"type"+type;

	}
}
