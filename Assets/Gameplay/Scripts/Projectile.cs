using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
    public float SeekingSpeed = 1f;
    private bool hasCollided = false;
    private GameObject TargetCollider = null;
    public AnimationCurve Speed;

    public async Task SeekTarget(Transform target, GameObject targetCollider) 
    {
        TargetCollider = targetCollider;
        var t = 0f;
        while (!hasCollided) {
            transform.LookAt(target);
            transform.position += transform.forward * Time.deltaTime * SeekingSpeed * Speed.Evaluate(t);

            await UniTask.DelayFrame(0);
            t += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject == TargetCollider) {
            hasCollided = true;
        }
    }
}
