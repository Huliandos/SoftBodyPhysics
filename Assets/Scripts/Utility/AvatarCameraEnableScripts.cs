using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCameraEnableScripts : MonoBehaviour
{
    private void OnEnable()
    {
        foreach (AnimationSwapper animSwap in transform.parent.GetComponentsInChildren<AnimationSwapper>()) {
            animSwap.enabled = true;
        }
    }
    private void OnDisable()
    {
        foreach (AnimationSwapper animSwap in transform.parent.GetComponentsInChildren<AnimationSwapper>())
        {
            animSwap.enabled = false;
        }
    }
}
