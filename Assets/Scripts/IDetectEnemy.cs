using UnityEngine;

public interface IDetectEnemy
{
    string Name { get; }
    void DetectEnemy(Enemy enemigo);
}
