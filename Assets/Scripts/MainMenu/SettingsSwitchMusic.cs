using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSwitchMusic : MonoBehaviour
{
    public void TurnMusic() 
    {
        MusicManager.Get.TurnMusic();
    }
}
