using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCard : MonoBehaviour
{
    public float punchLinePosY;

    bool isDragging = false;
    Vector2 startPos;
    Vector2 offset;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MoveCard(mousePos);
        }
    }
    private void OnMouseDown()
    {
        isDragging = true;
        offset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    void MoveCard(Vector2 mousePos)
    {
        Vector2 targetPos = new Vector2(transform.position.x, mousePos.y + offset.y);
        targetPos.y = Mathf.Clamp(targetPos.y, punchLinePosY, startPos.y + 1);
        transform.position = targetPos;

        if (!GameManager.clockedIn && transform.position.y < punchLinePosY * 0.9f)
        {
            FindObjectOfType<MainMenuManager>().ClockIn();
        }
    }
}
