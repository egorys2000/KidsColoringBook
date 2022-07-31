using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BackButton : MonoBehaviour
{
    public async void OnClicked() 
    {
        await ScreenManager.Get.ContextBackButton();
    }
}
