datablock ItemData(SitdEmptyFloatItem)
{
	shapeFile = "base/data/shapes/empty.dts";
	gravityMod = 0;
	canPickup = 1;
};

function SitdEmptyFloatItem::onPickup() {}

datablock StaticShapeData(sitd_cube)
{
	shapeFile = "Add-Ons/GameMode_Shot_in_the_Dark/data/cube.dts";
};

datablock StaticShapeData(sitd_chair)
{
	shapeFile = "Add-Ons/GameMode_Shot_in_the_Dark/data/chair.dts";
};

datablock StaticShapeData(sitd_table)
{
	shapeFile = "Add-Ons/GameMode_Shot_in_the_Dark/data/table.dts";
};

datablock StaticShapeData(sitd_cylinder2)
{
	shapeFile = "Add-Ons/GameMode_Shot_in_the_Dark/data/cylinder2.dts";
};

datablock fxLightData(sitd_light_top : PlayerLight)
{
	FlareOn = "0";
	radius = "16";
	uiName = "";
};

datablock fxLightData(sitd_light_danger : PlayerLight)
{
	color = "1 0.2 0.1 1";
	FlareOn = "0";
	radius = "16";
	uiName = "";
};

datablock fxLightData(sitd_light_highlight_red : PlayerLight)
{
	color = "1 0.5 0.5 1";
	FlareOn = "0";
	radius = "6";
	brightness = "10";
	uiName = "";
};

datablock PlayerData(sitd_move_player : PlayerStandardArmor)
{
	canJet = "0";
	jumpSound = "";
	maxDamage = "70";
	maxStepHeight = "1";
	runForce = "4320";
	uiName = "Shot in the Dark Normal Player";
};

datablock PlayerData(sitd_fixed_player : PlayerStandardArmor)
{
	airControl = "0";
	canJet = "0";
	crouchBoundingBox = PlayerStandardArmor.boundingBox;
	jumpForce = "0";
	jumpSound = "";
	jumpSurfaceAngle = "0";
	maxBackwardCrouchSpeed = "0";
	maxBackwardSpeed = "0";
	maxDamage = "70";
	maxForwardCrouchSpeed = "0";
	maxForwardSpeed = "0";
	maxJumpSpeed = "0";
	maxSideCrouchSpeed = "0";
	maxSideSpeed = "0";
	maxStepHeight = "0";
	// underwater...
	runForce = "0";
	uiName = "";
};

datablock ParticleData(SitdCdGodfatherParticle)
{
	lifetimeMS = 500;
	textureName = "Add-Ons/GameMode_Shot_In_The_Dark/data/images/gc_godfather";
	times[0] = 0;
	times[1] = 0.1;
	times[2] = 0.2;
	times[3] = 1;
	sizes[0] = 1;
	sizes[1] = 1;
	sizes[2] = 1;
	sizes[3] = 1;
	colors[0] = "1 1 1 1";
	colors[1] = "1 1 1 1";
	colors[2] = "1 1 1 1";
	colors[3] = "1 1 1 1";
};

datablock ParticleEmitterData(SitdCdGodfatherEmitter)
{
	particles = SitdCdGodfatherParticle;
	ejectionPeriodMS = 500;
	ejectionVelocity = 0;
	velocityVariance = 0;
	phiVariance = 0;
	thetaMax = 0;
};