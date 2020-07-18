using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour {
    // Start is called before the first frame update

    public float SeekingSpeed = 1f;
    private bool hasCollided = false;
    private GameObject TargetCollider = null;

    public AnimationCurve Speed;
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public async Task SeekTarget(Transform target, GameObject targetCollider) {
        this.TargetCollider = targetCollider;
        var t = 0f;
        while (!hasCollided) {
            this.transform.LookAt(target);

            this.transform.position += transform.forward * Time.deltaTime * SeekingSpeed * Speed.Evaluate(t);
            // Move closer to target
            await UniTask.DelayFrame(0);
            t += Time.deltaTime;
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other) {
        if (other.gameObject == this.TargetCollider) {
            hasCollided = true;
        }
    }
}
