using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
    [SerializeField] private float speed = 9.0f;
    [SerializeField] private AnimationCurve speedCurve = default;

    private bool collided = false;
    private GameObject targetBody = null;

    public async Task SeekTarget(Transform targetPoint, GameObject targetBody) 
    {
        this.targetBody = targetBody;
        float t = 0f;
        while (!collided) {
            transform.LookAt(targetPoint);
            transform.position += transform.forward * Time.deltaTime * speed * speedCurve.Evaluate(t);
            await UniTask.DelayFrame(0);
            t += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject == targetBody) {
            collided = true;
        }
    }
}
