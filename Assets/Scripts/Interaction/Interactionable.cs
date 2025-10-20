using UnityEngine;
using System.Collections;

public abstract class Interactionable : MonoBehaviour
{
    public DragHand[] hands = { null, null };
    public DragHand hand;
    public abstract void Enter(int index);
    public abstract void Exit(int index);
    
    public abstract void Grab(int index);
    public abstract void Free(int index);
}
