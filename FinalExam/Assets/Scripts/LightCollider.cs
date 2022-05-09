using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            other.GetComponent<Enemy>().HitWithFlashlight(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Enemy") {
            other.GetComponent<Enemy>().HitWithFlashlight(false);
        }
    }
}
