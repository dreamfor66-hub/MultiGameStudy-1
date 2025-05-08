using UnityEngine;

public class ColliderDebugger : MonoBehaviour
{
    private CapsuleCollider col;

    void Awake()
    {
        col = GetComponent<CapsuleCollider>();
        Debug.Log($"[Awake] CapsuleCollider.enabled = {col?.enabled}");
    }

    void Start()
    {
        Debug.Log($"[Start] CapsuleCollider.enabled = {col?.enabled}");
    }

    void OnEnable()
    {
        Debug.Log($"[OnEnable] CapsuleCollider.enabled = {col?.enabled}");
    }

    void Update()
    {
        if (col != null && !col.enabled)
        {
            Debug.LogWarning($"[Update] CapsuleCollider disabled at runtime on {gameObject.name}");
            //enabled = false; // 로그 반복 방지
        }
    }
}
