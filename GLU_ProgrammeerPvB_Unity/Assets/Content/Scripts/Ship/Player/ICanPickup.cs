/// <summary>
/// Interface used for picking up effects
/// </summary>
public interface ICanPickup
{
    void OnPickup(Effect effect);

    bool CanPickup();
}
