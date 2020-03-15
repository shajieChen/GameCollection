using UnityEngine;
using System.Collections;

public class CActivity : CGameConfigData
{
	public string key;
	public int nLevel;
	public int nStrength;
	public string szImgName;
	protected override string getFilePath ()
	{
		return "Assets/Resources/CSVFileCollection/Activity.csv";
	}
	public override string ToString(){

		return "key"+key+"nLevel"+nLevel+"nStrength"+nStrength+"szImgName"+szImgName;

	}
}
