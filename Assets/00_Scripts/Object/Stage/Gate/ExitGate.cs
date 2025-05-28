using UnityEngine;

public class ExitGate : BaseGate
{
    [SerializeField] private Door door;

    public Door Door => door;

    private void Awake()
    {
        OnPassingGate += () =>
        {
            door.Close();
        };
    }
}