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
    public state currentState,currentRoom;
    private int falseTriggerEnterAmount = 4; // Contador de entradas falsas debido a que ambos son trigger y se solapan

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = state.roomA;
    }

    public void ChangeStateWithEnterZoneATrigger()
    {
        if(falseTriggerEnterAmount >0)
        {
            falseTriggerEnterAmount--;
            return;
        }
        Debug.Log(currentState+ " Entrada a zona A");
        switch(currentState)
        {
            case state.roomA:
                currentState = state.inZoneA;
                break;
            case state.inZoneB:
                currentState = state.mid;
                break;
            default:
                break;
        }

    }

    public virtual void ChangeStateWithExitZoneATrigger()
    {
        Debug.Log(currentState +" Salida de zona A");
        switch(currentState)
        {
            case state.inZoneA:
                currentState = state.roomA;
                break;
            case state.mid:
                currentState = state.inZoneB;
                ActiveRoomB();
                break;
            default:
                break;
        }
    }
    public void ChangeStateWithEnterZoneBTrigger()
    {
        if(falseTriggerEnterAmount > 0)
        {
            falseTriggerEnterAmount--;
            return;
        }
        Debug.Log(currentState+ " Entrada a zona B");
        switch(currentState)
        {
            case state.roomB:
                currentState = state.inZoneB;
                break;
            case state.inZoneA:
                currentState = state.mid;
                break;
            default:
                break;
        }

    }

    public virtual void ChangeStateWithExitZoneBTrigger()
    {
        Debug.Log(currentState +" Salida de zona B");
        switch(currentState)
        {
            case state.inZoneB:
                currentState = state.roomB;
                break;
            case state.mid:
                currentState = state.inZoneA;
                ActiveRoomA();
                break;
            default:
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
        roomB.SetActive(true);
        roomA.SetActive(false);
    }
}


