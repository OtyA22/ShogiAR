using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    // Panelを格納する変数
    public GameObject NariPanel;
    GameObject refObj; // ReverseKomaのための変数
    public static GameObject holdKoma;
    // Start is called before the first frame update
    void Start()
    {
        refObj = GameObject.Find("Event Controller");
        holdKoma = null;
    }

    public void SelectNari()
    {
        NariPanel.SetActive(false);
        // 駒を裏返す
        IniSet iSet = refObj.GetComponent<IniSet>();
        iSet.ReverseKoma(holdKoma);

        holdKoma = null;
    }

    public void SelectFunari()
    {
        NariPanel.SetActive(false);
        holdKoma = null;
    }

    public void Initialize()
    {

        NariPanel.SetActive(false);
    }

    public void DisplayUI()
    {
        NariPanel.SetActive(true);
    }
}
