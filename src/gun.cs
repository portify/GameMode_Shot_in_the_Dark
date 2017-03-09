datablock ProjectileData(sitd_gun_projectile : GunProjectile)
{
    projectileShapeName = "base/data/shapes/empty.dts";
    muzzleVelocity = 200;
    uiName = "";
    impactImpulse = 0;
};

datablock ShapeBaseImageData(sitd_gun_image : GunImage)
{
    shapeFile = "Add-Ons/GameMode_Shot_in_the_Dark/data/magnum.dts";
    projectile = sitd_gun_projectile;
    stateEjectShell[2] = "0";
    stateEmitter[2] = "";
    stateEmitter[2] = "";
    stateSound[0] = "";
    stateSound[2] = "";
    uiName = "";
};

function sitd_gun_image::onFire(%data, %player, %slot)
{
    serverPlay2D(gunShot1Sound);
    Parent::onFire(%data, %player, %slot);
}