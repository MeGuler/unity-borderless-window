using System;
using System.Collections;
using System.Collections.Generic;
using Borderless;
using UnityEngine;

public class SendMessageTest : MonoBehaviour
{
    private void OnGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var activeWindow = WinApi.GetActiveWindow();
            var result = WinApi.SendMessage(activeWindow, (int) WindowMessages.NCHITTEST, 10, (IntPtr) 10);
            Debug.Log("SendMessage: " + result);
        }
    }
}