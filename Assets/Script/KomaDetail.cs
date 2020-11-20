using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 1駒の詳細クラス
 */ 

public class KomaDetail 
{
    public int x;
    public int y;
    public string komaName = "*"; // 駒の種類
    public string objName = "*"; // 駒のオブジェクト名
    public bool enemyFlag = true; // 味方はfalse, 敵はtrue
}
