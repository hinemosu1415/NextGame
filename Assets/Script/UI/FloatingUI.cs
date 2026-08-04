using UnityEngine;

public class FloatUI : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float moveY = 15f;

    private RectTransform rectTransform;
    private Vector2 firstPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        firstPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        rectTransform.anchoredPosition = new Vector2(firstPosition.x, firstPosition.y + Mathf.Sin(Time.time * speed) * moveY);
    }

}
