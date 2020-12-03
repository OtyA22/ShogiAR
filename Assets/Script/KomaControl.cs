using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]

/**
 * 駒の浮きコントロール
 */
public class KomaControl : MonoBehaviour
{
    Rigidbody rigidBody;
    Vector3 floatHeight = new Vector3(0, (float)0.05, 0);

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    public string ClickKomaAction()
    {
        if(this.gameObject.transform.localPosition.y < (float)0.11)
        {
            // 未選択の場合
            this.gameObject.transform.localPosition += floatHeight;
            return this.gameObject.name;
        }
        else
        {
            // 選択済みの場合
            this.gameObject.transform.localPosition -= floatHeight;
            return null;
        }
    }

    public string ClickKomaAction(Vector3 vec)
    {
        if (this.gameObject.transform.localPosition.y-vec.y < (float)0.11)
        {
            // 未選択の場合
            this.gameObject.transform.localPosition += floatHeight;
            return this.gameObject.name;
        }
        else
        {
            // 選択済みの場合
            this.gameObject.transform.localPosition -= floatHeight;
            return null;
        }
    }
}
