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

datablock PlayerData(sitd_move_player : PlayerStandardArmor)
{
	canJet = "0";
	jumpSound = "";
	maxDamage = "1";
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
	maxDamage = "1";
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