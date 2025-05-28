using System;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    [SerializeField] protected EnterGate enterGate;
    [SerializeField] protected ExitGate exitGate;
    
    protected Transform endPoint;
    
    public Transform EndPoint => endPoint;

    protected virtual void Awake()
    {
        exitGate.Door.DoorClosed += ResetRoom;
    }

    protected virtual void OpenDoor()
    {
        RoomManager.Instance.PlaceNextRoom();
    }

    protected virtual void EnterRoom()
    {
        RoomManager.Instance.PrevRoom = RoomManager.Instance.CurRoom;
        RoomManager.Instance.CurRoom = this;
        
        RoomManager.Instance.PrevRoom.ExitRoom();

        RoomManager.Instance.RoomChangedAction();
    }

    protected virtual void ExitRoom()
    {
        //exitGate.Door.Close();
    }

    protected abstract void ResetRoom();

    public abstract bool CanOpenDoor();
}
