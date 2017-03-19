//Department of Redundancy Department
function SitdShotInTheDark::onAdd(%script)
{
	%script.name = "Shot In The Dark";
	%script.enableDuel = true;
}

function SitdShotInTheDark::onRemove(%script)
{
	cancel(%script.event);
	deactivatePackage("SitdShotInTheDark");
}

function SitdShotInTheDark::onStart(%script)
{
	activatePackage("SitdShotInTheDark");
	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6It's time to make or break your trust, because it's time for everybody to be a killer.");
	%script.event = %script.schedule(3000, step1);
}

function SitdShotInTheDark::step1(%script)
{
	cancel(%script.event);
	if(%script.killerPlayer)
	{
		%script.killerPlayer.killer = 0;
		%script.waitingForKill = "";
		%script.killerPlayer.unMountImage(0);
		fixArmReady(%script.killerPlayer);
		sitdLightOn();
	}
	%maxPlayer = -1;
	for (%i = 0; %i < $DefaultMiniGame.numMembers && %i < 16; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;

		if (%player.chair !$= "" && %player != %script.killerPlayer)
			%player[%maxPlayer++] = %player;
	}

	%killer = %player[getRandom(%maxPlayer)];
	%killer.killer = "1";
	%script.killerPlayer = %killer;
	%script.killerClient = %killer.client;
	%script.killerName = %killer.client.getPlayerName();

	%script.waitForPrompt = true;
	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer is choosing how to kill.");
	centerPrint(%script.killerClient, "<font:verdana:24>\c6Kill in secret?\n\c2Left Click\c6 - Kill in Darkness, \c0Right Click\c6 - Kill in Public");
	%script.dark = 1;
	%script.event = %script.schedule(5000, step2, 1);
}

function SitdShotInTheDark::onDeath(%script)
{
	if (%script.waitingForKill)
		%script.step2Timeout(0);
}

function SitdShotInTheDark::onGunFire(%script, %image, %obj)
{
	return 1;
}

function SitdShotInTheDark::step2(%script)
{
	if (%script.inDuel) // clean this up
		return;

	%script.waitForPrompt = false;
	cancel(%script.event);
	%time = 20000;
	if(%script.dark)
	{
		sitdLightOff();
		%time = 10000;
	}

	%script.waitingForKill = "1";
	%script.killerPlayer.mountImage(sitd_gun_image, "0");
	fixArmReady(%script.killerPlayer);

	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer is choosing somebody to kill.");
	centerPrint(%script.killerClient, "<font:verdana:24>\c0You have " @ %time/1000 @ " seconds to kill somebody.");

	%script.event = %script.schedule(%time, step2Timeout, 1);
}

function SitdShotInTheDark::step2Timeout(%script, %kill)
{
	cancel(%script.event);
	if (%script.inDuel) // clean this up
		return;

	%script.waitingForKill = "";
	%script.killerPlayer.unMountImage(0);
	fixArmReady(%script.killerPlayer);
	sitdLightOn();
	if(%kill)
	{
		$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer took too long to kill.");
		%script.killerPlayer.schedule(1500, "kill");
	}
	$DefaultMiniGame.centerPrintAll("");
	%script.event = %script.schedule(5000, step1);
}

package SitdShotInTheDark
{
	function Player::playThread(%this, %slot, %sequenceName)
	{
		%script = $DefaultMiniGame.currentMode;
		if(%script.class $= "SitdShotInTheDark" && %sequenceName $= "activate")
			return;
		Parent::playThread(%this, %slot, %sequenceName);
	}

	function Armor::onTrigger(%this, %obj, %trig, %tog)
	{
		Parent::onTrigger(%this, %obj, %trig, %tog);
		%script = $DefaultMiniGame.currentMode;
		if(%script.class !$= "SitdShotInTheDark")
			return;
		if(!%script.waitForPrompt || %script.killerPlayer != %obj)
			return;
		if(%trig == 0 && %tog) //click
		{
			%script.dark = 1;
			%script.waitForPrompt = false;
			centerPrint(%script.killerClient, "<font:verdana:24>\c6You decided to \c2Kill in Darkness\c6.");
		}
		else if(%trig == 4 && %tog) //rightclick
		{
			%script.dark = 0;
			%script.waitForPrompt = false;
			centerPrint(%script.killerClient, "<font:verdana:24>\c6You decided to \c0Kill in Public\c6.");
		}
	}
};