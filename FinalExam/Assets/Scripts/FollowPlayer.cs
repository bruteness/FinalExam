using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowPlayer : MonoBehaviour { // This script will make a GUITexture follow a transform.
    [SerializeField] private Transform target;
    private void Update() {
        transform.LookAt(target);
    }
}
