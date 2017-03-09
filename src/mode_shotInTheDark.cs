//Department of Redundancy Department
function SitdShotInTheDark::onAdd(%script)
{
	%script.name = "Shot In The Dark";
}

function SitdShotInTheDark::onRemove(%script)
{
	cancel(%script.event);
	deactivatePackage("SitdShotInTheDark");
}

function SitdShotInTheDark::onStart(%script)
{
	activatePackage("SitdShotInTheDark");
	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Get ready...");
	%script.event = %script.schedule(3000, step1);
}

function SitdShotInTheDark::step1(%script)
{
	if(%script.killerPlayer)
	{
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

		if (%player.chair !$= "")
			%player[%maxPlayer++] = %player;
	}

	%killer = %player[getRandom(%maxPlayer)];
	%script.killerPlayer = %killer;
	%script.killerClient = %killer.client;
	%script.killerName = %killer.client.getPlayerName();

	%script.waitForPrompt = true;
	centerPrint(%script.killerClient, "<font:verdana:24>\c6Kill in secret?\n\c2Left Click\c6 - Kill in Darkness, \c0Right Click\c6 - Kill in Public");
	%script.event = %script.schedule(5000, step2, 1);
}

function SitdShotInTheDark::onDeath(%script)
{
	if (%script.waitingForKill)
		%script.step1();
}

function SitdShotInTheDark::step2(%script, %dark)
{
	if (%script.inDuel) // clean this up
		return;

	%script.waitForPrompt = false;
	cancel(%script.event);
	if(%dark)
		sitdLightOff();

	%script.waitingForKill = "1";
	%script.killerPlayer.mountImage(sitd_gun_image, "0");
	fixArmReady(%script.killerPlayer);

	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer is choosing somebody to kill.");	
	centerPrint(%script.killerClient, "<font:verdana:24>\c0You have 10 seconds to kill somebody.");

	%script.event = %script.schedule(10000, step2Timeout);
}

function SitdShotInTheDark::step2Timeout(%script)
{
	%script.waitingForKill = "";
	%script.killerPlayer.unMountImage(0);
	fixArmReady(%script.killerPlayer);
	sitdLightOn();

	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer took too long to kill.");
	%script.killerPlayer.schedule(1500, "kill");
	%script.event = %script.schedule(2000, step1);
}

package SitdShotInTheDark
{
	function Armor::onTrigger(%this, %obj, %trig, %tog)
	{
		Parent::onTrigger(%this, %obj, %trig, %tog);
		%script = $DefaultMiniGame.currentMode;
		if(%script.class !$= "SitdShotInTheDark")
			return;
		if(!%script.waitForPrompt || %script.killerPlayer != %obj)
			return;
		if(%trig == 0 && %tog) //click
			%script.step2(1);
		else if(%trig == 4 && %tog)
			%script.step2(0);
	}
};