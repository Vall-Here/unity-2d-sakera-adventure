interface IWeapon
{
    string WeaponType { get; }
    public void Attack();

    public  WeaponInfo GetWeaponInfo();
}