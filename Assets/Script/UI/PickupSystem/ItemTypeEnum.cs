using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemTypeEnum // Menggunakan class statis untuk menampung enum
{
    public enum SlotItemType // Nama enum tidak perlu diawali dengan tipe data
    {
        Melee,
        Range,
        Particle,
        Armor,
        Helmet,
        Accessory,
        Consumable,
        Material,
        Projectile,
        DefaultItem,
    }
}
