using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class FreeWeaponStats : MonoBehaviour {
    public int damage = 0;
    public int durability = 0;
    public bool isShiny = false;
    public int difficulty = 0;
    public int weaponIndex; // 0 axe, 1 sword, 2 spear

    public void MakeShiny() {
        GameObject shiny = transform.Find("ShinyParticles").gameObject;
        shiny.SetActive(true);
        isShiny = true;
    }
}
