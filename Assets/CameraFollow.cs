using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // 追いかける対象（Player）
    public Vector3 offset;     // カメラとプレイヤーの距離
    public float smoothSpeed = 5f; // 追従スピード

    void LateUpdate()
    {
        if (target == null) return;

        // 目標位置
        Vector3 desiredPosition = target.position + offset;

        // スムーズに補間して追いかける
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 常にプレイヤーの方向を向く
        transform.LookAt(target);
    }
}
