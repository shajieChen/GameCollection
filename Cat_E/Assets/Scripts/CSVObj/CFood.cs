using UnityEngine;
using System.Collections;

public class CFood : CGameConfigData
{
	public string key;
	public int nLevel;
	public int nStrengthV;
	public string szImgName;
	protected override string getFilePath ()
	{
		return "Assets/Resources/CSVFileCollection/Food.csv";
	}
	public override string ToString(){

		return "key"+key+"nLevel"+nLevel+"nStrengthV"+nStrengthV+"szImgName"+szImgName;

	}
}
