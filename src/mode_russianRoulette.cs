function SitdRussianRoulette::onAdd(%script)
{
	%script.name = "Russian Roulette";
	%script.chairIndex = getRandom(15);
}

function SitdRussianRoulette::onRemove(%script)
{
	cancel(%script.event);
}

function SitdRussianRoulette::onStart(%script)
{
	for (%i = 0; %i < $DefaultMiniGame.numMembers && %i < 16; %i++)
	{
		%client = $DefaultMiniGame.member[%i];

		if (%client.player.chair !$= "")
		{
			%script.spinChamber(%client.player, getRandom(4,8));
			%client.player.badNumber = getRandom(1, 6);
			%client.player.spins = 1;
		}
	}

	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Get ready...");
	%script.event = %script.schedule(3000, step1);
}

function SitdRussianRoulette::onDeath(%script)
{
}

function SitdRussianRoulette::step1(%script)
{
	%numAlive = 0;

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];

		if (%client.player.chair !$= "")
		{
			%alive[%numAlive] = %client.player;
			%numAlive++;
		}
	}

	if (!%numAlive)
	{
		talk("nobody is alive even though the game is still going, what?");
		return;
	}

	$light1.setEnable(0);
	$light2.setEnable(0);

	for (%i = 0; %i < 16; %i++)
	{
		%index = (%script.chairIndex + %i) % 16;
		%who = $chair[%index].player;

		if (isObject(%who) && !%who.isDead)
		{
			$lightHL.setTransform(vectorAdd(%who.position, "0 0 4"));
			$lightHL.setEnable(1);

			%who.client.play2D(weaponSwitchSound);

			%script.victim = %who;
			%script.victim.mountImage(sitd_gun_image, 0);
			fixArmReady(%script.victim);
			$DefaultMiniGame.centerPrintAll("<font:verdana:18>\c3" @ %who.client.name @ " \c6has 10 seconds to pull the trigger.");
			centerPrint(%who.client, "<font:verdana:24>\c0Click to pull the trigger. You have 10 seconds.\nLight key to spin the camber (once per game)");
			%script.event = %script.schedule(10000, step1Timeout);

			%script.chairIndex = %index + 1;
			return;
		}
	}
}

function SitdRussianRoulette::step1Timeout(%script)
{
	%victim = %script.victim;
	%script.victim = "";

	if (!isObject(%victim) || %victim.isDead)
	{
		talk("victim took too long to pull the trigger yet they're already dead");
	}
	else
	{
		$DefaultMiniGame.centerPrintAll("<font:verdana:18>\c6They took too long to pull the trigger and have been killed.");
		%victim.kill();
	}

	%script.victim = "";
	%script.event = %script.schedule(1000, step1);
}

function SitdRussianRoulette::onLight(%script, %client)
{
	if(!isObject(%client.player) || %client.player != %script.victim)
		return;

	if(%client.player.spins-- >= 0)
	{
		%script.spinChamber(%client.player, getRandom(4,8));
	}
}

function SitdRussianRoulette::spinChamber(%script, %player, %spins)
{
	cancel(%player.spinSchedule);

	if(%spins <= 0)
		return;

	%player.playThread(2, "rotCCW");

	%player.chambered++;
	if(%player.chambered > 6)
		%player.chambered = 1;
	%hit = "hit";		
	if (%player.chambered == %player.badNumber)
		%hit = "miss";

	serverPlay3D("wrench" @ %hit @ "Sound", %player.getHackPosition());
	%player.spinSchedule = %script.schedule(getMax(100, 500 / %spins), "spinChamber", %player, %spins--);
}

function SitdRussianRoulette::onGunFire(%script, %image, %obj)
{
	cancel(%script.event);
	cancel(%obj.spinSchedule);
	%script.victim = "";
	
	if (%obj.chambered != %obj.badNumber)
	{
		serverPlay2D(SitdRevolverClickSound, %obj.getPosition());
		$DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6Click! Lucky. The game continues.");
		%script.event = %script.schedule(1000, step1);
		%obj.unMountImage(0);
		fixArmReady(%obj);
		%obj.chambered++;
		if(%obj.chambered > 6)
			%obj.chambered = 1;
		return 0;
	}

	serverPlay2D(SitdRevolverWalkerFireSound, %obj.getPosition());
	$DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6What a shame.");
	%obj.kill();
	%script.event = %script.schedule(1000, step1);
	return 0;
}