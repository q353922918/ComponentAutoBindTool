using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AutoBindTest : MonoBehaviour
{

    private int count = 0;


    void Start()
    {
        GetBindComponents(gameObject);
        
        view.btnTest2.onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        count++;
        view.txtTest3.text = "点击了按钮" + count + "次";
        view.imgTest1.gameObject.SetActive(!view.imgTest1.gameObject.activeInHierarchy);
    }
}
