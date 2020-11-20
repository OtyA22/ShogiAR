using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 保持した駒のクラス
 */

public class HoldKomaInfo 
{
    public int x =0;
    public int y =0;
    public GameObject hKoma = null; // 駒のオブジェクト

    public void Init()
    {
        this.x = 0;
        this.y = 0;
        this.hKoma = null;
    }
}
