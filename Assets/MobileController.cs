using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool IsPressed;
    public bool IsLeftButtonDown;
    public bool IsRightButtonDown;

    public PlayerSetting playerMoveHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }

    private void Update()
    {
        if (IsLeftButtonDown && IsPressed)
        {
            playerMoveHandler.MoveLeft();
        }
        else if (IsRightButtonDown && IsPressed)
        {
            playerMoveHandler.MoveRight();
        }
    }
}
