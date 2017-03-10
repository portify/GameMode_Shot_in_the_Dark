function SitdWerewolf::onAdd(%script)
{
	%script.name = "Werewolf";
}

function SitdWerewolf::onRemove(%script)
{
	cancel(%script.event);
}

function SitdWerewolf::onStart(%script)
{
	%playerCount = 0;
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%player = $DefaultMiniGame.member[%i].player;
		if (%player.chair !$= "")
        {
			%player[%playerCount] = %player;
            %playerCount++;
        }
	}

    %i = %playerCount;
    while (%i--)
	{
		%j = getRandom(%i);
		%x = %player[%i - 1];
		%player[%i - 1] = %player[%j];
		%player[%j] = %x;
	}

    %werewolfCount = 2;
    %werewolfIndex = %werewolfCount;
    while (%werewolfIndex-- >= 0)
    {
        %werewolf[%werewolfIndex] = %player[%playerCount--];
        %werewolf[%werewolfIndex].werewolf = "1";
    }

    if (getRandom(1)) // 50% chance for alpha werewolf
    {
        %alpha = %werewolf[getRandom(%werewolfCount - 1)];
        %alpha.alpha = "1";
    }

    %seer = %player[%playerCount--];
    %seer.role = "seer";
    %script.seer = %seer;

    %doctor = %player[%playerCount--];
    %doctor.role = "doctor";
    %script.doctor = %doctor;

    // TODO: assign the following rules
    //       - villager & drunk
    //       - villager & witch

    sitdLightOff();
    %script.disableChat = "1";

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
        %client = $DefaultMiniGame.member[%i];
		%player = %client.player;

		if (%player.chair !$= "")
        {
			if (%player.werewolf)
            {
                if (%player.alpha)
                    centerPrint(%client, "<font:verdana bold:30><color:ffa0a0>ALPHA WEREWOLF<br><font:verdana bold:30><color:ffffff>Must say \"Werewolf\" once a day<br>You will die if you do not");
                else
                    centerPrint(%client, "<font:verdana bold:30><color:ffa0a0>WEREWOLF<br><font:verdana bold:30><color:ffffff>Take over the Village");
            }
            else switch$ (%player.role)
            {
            case "":
                centerPrint(%client, "<font:verdana bold:30><color:ffffff>VILLAGER<br><font:verdana:26>Defend the Village");
            case "seer":
                centerPrint(%client, "<font:verdana bold:30><color:ffffff>VILLAGER & SEER<br><font:verdana:26>Find the Werewolves");
            case "doctor":
                centerPrint(%client, "<font:verdana bold:30><color:ffffff>VILLAGER & DOCTOR<br><font:verdana:26>Heal the innocent");
            case "drunk":
                centerPrint(%client, "<font:verdana bold:30><color:ffffff>VILLAGER & DRUNK<br><font:verdana:26>Cannot use words");
            case "witch":
                centerPrint(%client, "<font:verdana bold:30><color:ffffff>VILLAGER & WITCH<br><font:verdana:26>Heal once, poison once");
            default:
                centerPrint(%client, "<font:verdana bold:30><color:ffffff>I AM ERROR<br><font:verdana:26>You should not see this");
            }
        }
        else
            centerPrint(%client, "<font:verdana:24>\c6Starting the game...");
	}

	%script.event = %script.schedule(5000, step1);
}

function SitdWerewolf::onDeath(%script)
{
}

function SitdWerewolf::step1(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Werewolves, open your eyes.");
    %script.event = %script.schedule(1000, step2);
}

function SitdWerewolf::step2(%script)
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.werewolf)
        {
            %player.playThread("0", "root");
            %player.setArmThread("look");
            sitdWereOpenEyes(%client);
        }
    }

    %script.event = %script.schedule(1000, step3);
}

function SitdWerewolf::step3(%script)
{
    $DefaultMiniGame.centerPrintAll("");
    %script.event = %script.schedule(1000, step4);
}

function SitdWerewolf::step4(%script)
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.werewolf)
        {
			%player.voteTarget = "0";
			%player.canCastVote = "1";
			%player.playThread(1, armReadyRight);
			sitdWhoUpdateCastVote(%player);
            centerPrint(%client, "<font:verdana:24>\c6Werewolves, pick someone to kill.\n\c3Pick a villager by looking at them.\n\c3You have 10 seconds to decide with a majority.");
        }
        else
        {
            if (%player.chair !$= "")
            {
                %player.voteCount = "0";
                %player.canReceiveVote = "1";
            }

            centerPrint(%client, "<font:verdana:24>\c6Werewolves, pick someone to kill.\n\c6You have 10 seconds.");
        }
    }

    %script.event = %script.schedule(10000, step5);
}

function SitdWerewolf::step5(%script)
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;

        if (%player.canCastVote)
        {
            cancel(%player.sitdWhoUpdateCastVote);
            %player.canCastVote = "";
            %player.voteTarget = "";
            %player.playThread("1", "root");
        }

		if (%player.canReceiveVote)
		{
			if (%highestVotes !$= "" && %player.voteCount $= %highestVotes)
				%tie = 1;
			else if (%highestVotes $= "" || %player.voteCount > %highestVotes)
			{
				%tie = 0;
				%highestVotes = %player.voteCount;
				%unfortunate = %player;
			}

			%player.canReceiveVote = "";
			%player.voteCount = "";
			%player.setShapeName("", "8564862");
		}
	}

    if (%tie || !isObject(%unfortunate))
    {
        %script.werewolfKillVote = "";

        for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
        {
            %client = $DefaultMiniGame.member[%i];
            %player = %client.player;

            if (%player.werewolf || %player.chair $= "")
                centerPrint(%client, "<font:verdana:24>\c6Since you were unable to decide, no-one will die tonight.");
            else
                centerPrint(%client, "<font:verdana:24>\c6I understand.");
        }
    }
    else
    {
        %script.werewolfKillVote = %unfortunate;
        $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6I understand.");
    }

    %script.event = %script.schedule(1500, step6);
}

function SitdWerewolf::step6(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Werewolves, close your eyes.");
    %script.event = %script.schedule(1000, step7);
}

function SitdWerewolf::step7(%script)
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.werewolf)
        {
            %player.playThread("0", "sit");
            %player.setArmThread("root");
            sitdWereCloseEyes(%client);
        }
    }

    %script.event = %script.schedule(1000, step8);
}

function SitdWerewolf::step8(%script)
{
    $DefaultMiniGame.centerPrintAll("");
    %script.event = %script.schedule(1000, step9);
}

function SitdWerewolf::step9(%script)
{
    if (!isObject(%script.doctor) || %script.doctor.isDead)
    {
        %script.event = %script.schedule(1, step11);
        return;
    }

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Doctor, who would you like to heal?");
    centerPrint(%script.doctor.client, "<font:verdana:20>\c6Doctor, who would you like to heal?\n\c3Click somebody to save them if the Werewolves decided to kill them.\n\c3Right click to save yourself. You have 10 seconds.");
    sitdWereOpenEyes(%script.doctor.client);
    %script.doctor.playThread(1, armReadyRight);
    %script.event = %script.schedule("10000", "step10", "");
    %script.doctorVoteEvent = %script.event;
}

function SitdWerewolf::step10(%script, %choice)
{
    cancel(%script.event);

    if (isObject(%script.doctor))
    {
        %script.doctor.playThread("1", "root");

        if (isObject(%script.doctor.client))
            sitdWereCloseEyes(%script.doctor.client);
    }

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Understood. Close your eyes, Doctor.");

    if (isObject(%script.werewolfKillVote) && %choice == %script.werewolfKillVote)
        %script.doctorDidSave = "1";
    
    %script.event = %script.schedule("1500", "step11");
}

function SitdWerewolf::step11(%script)
{
    if (!isObject(%script.seer) || %script.seer.isDead)
    {
        %script.event = %script.schedule(1, step14);
        return;
    }

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Seer, open your eyes. Pick someone to ask about.");
    centerPrint(%script.seer.client, "<font:verdana:20>\c6Seer, open your eyes. Pick someone to ask about by pointing to them.\n\c3You have 10 seconds.");
    sitdWereOpenEyes(%script.seer.client);
    %script.seer.playThread(1, armReadyRight);
    %script.event = %script.schedule("10000", "step12", "");
    %script.seerVoteEvent = %script.event;
}

function SitdWerewolf::step12(%script, %choice)
{
    cancel(%script.event);

    $DefaultMiniGame.centerPrintAll("");

    if (isObject(%script.seer))
    {
        %script.seer.playThread("1", "root");

        if (isObject(%script.seer.client))
        {
            if (isObject(%choice))
            {
                if (%choice.werewolf)
                    centerPrint(%script.seer.client, "<font:verdana:20>\c6Thumbs up. That person is a Werewolf.");
                else
                    centerPrint(%script.seer.client, "<font:verdana:20>\c6Thumbs down. That person is just a Villager.");
            }
        }
    }

    %script.event = %script.schedule("2000", "step13");
}

function SitdWerewolf::step13(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Seer, close your eyes.");

    if (isObject(%script.seer.client))
        sitdWereCloseEyes(%script.seer.client);

    %script.event = %script.schedule("1000", "step14");
}

function SitdWerewolf::step14(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Everyone wake up; it's daytime.");
    %script.event = %script.schedule("500", "step15");
}

function SitdWerewolf::step15(%script)
{
    %script.disableChat = "";
    sitdLightOn();
    %script.event = %script.schedule("1000", "step16");
}

function SitdWerewolf::step16(%script)
{
    if (isObject(%script.werewolfKillVote) && !%script.werewolfKillVote.isDead)
    {
        if (%script.doctorDidSave)
            $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Someone has been saved by the Doctor. Nobody died tonight.");
        else
        {
            $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c0Somebody has been killed.");
            %script.werewolfKillVote.schedule("300", "kill");
        }
    }
    else
        $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Nobody was killed tonight.");
    
    %script.doctorDidSave = "";
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Discuss?", 3);
    %script.event = %script.schedule("30000", "step17");
}

function SitdWerewolf::step17(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6I haven't implemented lynching yet, so you're a bit screwed.");
    %script.event = %script.schedule("3000", "step18");
}

function SitdWerewolf::step18(%script)
{
    $DefaultMiniGame.centerPrintAll("It's nighttime. Everyone go to sleep.");
    %script.event = %script.schedule("500", "step19");
}

function SitdWerewolf::step19(%script)
{
    %script.disableChat = "1";
    sitdLightOff();
    %script.event = %script.schedule("1500", "step1");
}

function sitdWereOpenEyes(%client)
{
    $light1.scopeToClient(%client);
    $light2.scopeToClient(%client);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %member = $DefaultMiniGame.member[%i];
        %player = %member.player;

        if (%player.chair !$= "")
            %player.scopeToClient(%client);
    }
}

function sitdWereCloseEyes(%client)
{
    $light1.clearScopeToClient(%client);
    $light2.clearScopeToClient(%client);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %member = $DefaultMiniGame.member[%i];
        %player = %member.player;

        if (%player.chair !$= "" && %member != %client)
            %player.clearScopeToClient(%client);
    }
}