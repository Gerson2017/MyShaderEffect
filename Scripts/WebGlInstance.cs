using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebGlInstance : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Foo();

    [DllImport("__Internal")]
    private static extern void Boo();

    void Awake()
    {
        Foo();
        Boo();
    }
}
