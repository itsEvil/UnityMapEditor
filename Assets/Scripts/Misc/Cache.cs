using UnityEngine;

public class Cache : MonoBehaviour
{
    public static Cache Instance { get; private set; }
    [SerializeField] private Sprite _regionSprite, _noSprite;
    private void Awake()
    {
        Instance = this;
    }
    public Sprite NoSprite => _noSprite;
    public Sprite GetRegionSprite() => _regionSprite;
    public Sprite GetNoSprite() => _noSprite;
}
