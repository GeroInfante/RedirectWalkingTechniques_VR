using UnityEngine;

public class ChangeRoomManager : MonoBehaviour
{

    public enum state
    {
        roomA = 0,
        inZoneA = 1,
        mid = 2,
        inZoneB = 3,
        roomB = 4
    }
    public GameObject roomA;
    public GameObject roomB;
    private state currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = state.roomA;
    }

    public void ChangeStateWithEnterZoneATrigger()
    {
        switch(currentState)
        {
            case state.roomA:
                currentState = state.inZoneA;
                break;
            case state.inZoneB:
                currentState = state.mid;
                break;
        }

    }

    public void ChangeStateWithExitZoneATrigger()
    {
        switch(currentState)
        {
            case state.inZoneA:
                currentState = state.roomA;
                break;
            case state.mid:
                currentState = state.inZoneB;
                ActiveRoomB();
                break;
        }
    }
    public void ChangeStateWithEnterZoneBTrigger()
    {
        switch(currentState)
        {
            case state.roomB:
                currentState = state.inZoneB;
                break;
            case state.inZoneA:
                currentState = state.mid;
                break;
        }

    }

    public void ChangeStateWithExitZoneBTrigger()
    {
        switch(currentState)
        {
            case state.inZoneB:
                currentState = state.roomB;
                break;
            case state.mid:
                currentState = state.inZoneA;
                ActiveRoomA();
                break;
        }
    }

    private void ActiveRoomA()
    {
        roomA.SetActive(true);
        roomB.SetActive(false);
    }
    private void ActiveRoomB()
    {
        roomA.SetActive(false);
        roomB.SetActive(true);
    }
}


