using UnityEngine;

/// <summary>
/// Uses the island base class and adds a player start transform
/// </summary>
public class OutpostIsland : IslandBase
{
    [SerializeField] private Transform _playerStart;
    public Transform PlayerStart => _playerStart;
}