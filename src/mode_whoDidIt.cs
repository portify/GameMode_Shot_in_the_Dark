function SitdWhoDidIt::onAdd(%script)
{
	%script.name = "Who Did It?";
	%script.enableDuel = true;
}

function SitdWhoDidIt::onRemove(%script)
{
	cancel(%script.event);
}

function SitdWhoDidIt::onStart(%script)
{
	%maxPlayer = -1;
	for (%i = 0; %i < $DefaultMiniGame.numMembers && %i < 16; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
        if(!isObject(%player))
            continue;
		if (%player.chair !$= "")
			%player[%maxPlayer++] = %player;
	}

	%killer = %player[getRandom(%maxPlayer)];
	%killer.killer = "1";
	%script.killerPlayer = %killer;
	%script.killerClient = %killer.client;
	%script.killerName = %killer.client.getPlayerName();

	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6One person is a killer. Find out who it is before you're all dead.");
	centerPrint(%killer.client, "<font:verdana:28>\c0You are the killer in this game! Eliminate everyone else.");
	%script.event = %script.schedule(3000, step1);
}

function SitdWhoDidIt::onDeath(%script)
{
	if (%script.waitingForKill)
		%script.step2();
}

function SitdWhoDidIt::step1(%script)
{
	sitdLightOff();

	%script.waitingForKill = "1";
	%script.killerPlayer.mountImage(sitd_gun_image, "0");
	fixArmReady(%script.killerPlayer);

	$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer is choosing somebody to kill.");

	if (%script.killerWaited)
		centerPrint(%script.killerClient, "<font:verdana:24>\c0You have 10 seconds to kill somebody.");
	else
		centerPrint(%script.killerClient, "<font:verdana:24>\c0You have 10 seconds to kill somebody.\n\c3You may choose not to kill once.");

	%script.event = %script.schedule(10000, step1Timeout);
}

function SitdWhoDidIt::step1Timeout(%script)
{
	%script.waitingForKill = "";
	%script.killerPlayer.unMountImage("0");
	fixArmReady(%script.killerPlayer);
	sitdLightOn();

	if (%script.killerWaited)
	{
		$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer took too long to kill.");
		%script.killerPlayer.schedule("1500", "kill");
	}
	else
	{
		%script.killerWaited = "1";
		$DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer has chosen not to kill this round.");
		%script.event = %script.schedule(2000, step2);
	}
}

function SitdWhoDidIt::step2(%script)
{
	cancel(%script.event);

	%script.waitingForKill = "";
	%script.killerPlayer.unMountImage("0");
	fixArmReady(%script.killerPlayer);
	sitdLightOn();

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;

		if (%player.chair !$= "")
		{
			%player.voteCount = "0";
			%player.voteTarget = "0";
			%player.canReceiveVote = "1";
			%player.canCastVote = "1";
			%player.playThread(1, armReadyRight);
			sitdWhoUpdateCastVote(%player);
		}
	}

	$DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6Look at the person you think is the killer within 10 seconds.<br>The person with the most votes will die.");
	%script.event = %script.schedule(10000, step3);
}

function SitdWhoDidIt::step3(%script)
{
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(!isObject(%player))
			continue;
		%player.playThread(1, root);
		if (%player.canCastVote)
		{
			if (%highestVotes !$= "" && %player.voteCount $= %highestVotes)
				%tie = 1;
			else if (%highestVotes $= "" || %player.voteCount > %highestVotes)
			{
				%tie = 0;
				%highestVotes = %player.voteCount;
				%unfortunate = %player;
			}

			cancel(%player.sitdWhoUpdateCastVote);
			%player.voteTarget = "";
			%player.canReceiveVote = "";
			%player.canCastVote = "";
			%player.voteCount = "";
			%player.setShapeName("", "8564862");
		}
	}

	if (%tie)
		$DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6It's a tie, nobody will be eliminated. Moving on.");
	else if (isObject(%unfortunate))
	{
		$DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6The person with the most votes has been eliminated.");
		%unfortunate.kill();
	}
	else
		$DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6Nobody voted. Unfortunate for everyone but the killer. Let's go again.");

	if (%script.inDuel) // clean this up
		return;

	%script.event = %script.schedule(2000, step1);
}

function sitdWhoUpdateCastVote(%player)
{
	cancel(%player.sitdWhoUpdateCastVote);

	if (!%player.canCastVote)
		return;

	%a = %player.getEyePoint();
	%v = %player.getEyeVector();
	%b = VectorAdd(%a, VectorScale(%v, 100));
	%mask = $TypeMasks::PlayerObjectType;
	%ray = containerRayCast(%a, %b, %mask, %player);
	%col = firstWord(%ray);

	if (%col && !%col.canReceiveVote)
		%col = "0";

	if (%col !$= %player.voteTarget)
	{
		if (isObject(%player.voteTarget))
		{
			%player.voteTarget.voteCount--;
			%player.voteTarget.setShapeName(%player.voteTarget.voteCount ? %player.voteTarget.voteCount : "", "8564862");
		}

		%player.voteTarget = %col;

		if (isObject(%player.voteTarget))
		{
			%player.voteTarget.voteCount++;
			%player.voteTarget.setShapeName(%player.voteTarget.voteCount ? %player.voteTarget.voteCount : "", "8564862");
		}
	}

	%player.sitdWhoUpdateCastVote = schedule(32, %player, "sitdWhoUpdateCastVote", %player);
}