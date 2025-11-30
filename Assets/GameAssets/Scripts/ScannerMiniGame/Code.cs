using UnityEngine;

/// <summary>
/// Класс, хранящий категорию материала для сканируемого кода в мини-игре сканера
/// </summary>
public class Code : MonoBehaviour
{
    public MaterialReference MaterialLink => materialLink;

    [SerializeField] private MaterialReference materialLink;

    public enum MaterialReference
    {
        PLA, ABS, PETG, Nylon, TPU, SLA
    }
}