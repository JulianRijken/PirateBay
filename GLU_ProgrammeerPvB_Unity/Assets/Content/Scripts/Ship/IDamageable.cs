/// <summary>
/// Interface used for taking damage and healing
/// </summary>
public interface IDamageable
{
    void OnHealthChange(float delta);
}