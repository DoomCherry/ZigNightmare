using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSYNC : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1;
    }
}
