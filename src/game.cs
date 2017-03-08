// TODO: let killer not kill once, but kill them the second time

function seatPlayers()
{
	// prepare
	for (%i = 0; %i < 16; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}
	// place
    %maxPlayer = -1;
	for (%i = 0; %i < $DefaultMiniGame.numMembers && %i < 16; %i++)
	{
		%client = $DefaultMiniGame.member[%i];

        if (%client.player)
            %client.player.delete();
        
        %index = %a[%i];
        %chair = $chair[%index];

        %player = new Player()
        {
            datablock = sitd_fixed_player;
            client = %client;
            chair = %index;
            killer = "0";
        };

        // %player.setShapeName("", "8564862");
		%player.setTransform(%chair.getSlotTransform("0"));
		%player.playThread("0", "sit");

        %client.player = %player;
        %client.applyBodyParts();
        %client.applyBodyColors();
        %client.setControlObject(%player);

        %player[%maxPlayer++] = %player;
	}

    switch$ ($DefaultMiniGame.currentMode)
    {
    case "whoDidIt":
        %killer = %player[getRandom(%maxPlayer)];
        %killer.killer = "1";
        $DefaultMiniGame.killerPlayer = %killer;
        $DefaultMiniGame.killerClient = %killer.client;
        $DefaultMiniGame.killerName = %killer.client.getPlayerName();
    }
}

function sitdEndGame()
{
    cancel($DefaultMiniGame.gameSchedule);
    cancel($DefaultMiniGame.restartSchedule);
    $DefaultMiniGame.currentMode = "";
    $DefaultMiniGame.continueWhoOnNextKill = "";
    $DefaultMiniGame.restartSchedule = schedule("5000", $DefaultMiniGame, sitdPrepareGame);
}

function sitdPrepareGame()
{
    cancel($DefaultMiniGame.restartSchedule);
    cancel($DefaultMiniGame.gameSchedule);

    $DefaultMiniGame.continueWhoOnNextKill = "";
    $DefaultMiniGame.rouletteIndex = "";

    $DefaultMiniGame.currentMode = "whoDidIt";
    // $DefaultMiniGame.currentMode = "russianRoulette";
    // $DefaultMiniGame.rouletteIndex = "0";

    if ($DefaultMiniGame.numMembers < 2)
    {
        $DefaultMiniGame.centerPrintAll("", 1);

        for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
        {
            %client = $DefaultMiniGame.member[%i];

            if (%client.player.chair !$= "")
                %client.instantRespawn();
        }

        return;
    }

    seatPlayers();

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Next up: Who Did It?");
    // $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Next up: Russian Roulette");
    $DefaultMiniGame.gameSchedule = schedule("3000", $DefaultMiniGame, sitdStartGame);
}

function sitdStartGame()
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Get ready...");

    switch$ ($DefaultMiniGame.currentMode)
    {
        case "whoDidIt": %f = sitdWhoTurn1;
        case "russianRoulette": %f = sitdRussianStep1;
    }

    $DefaultMiniGame.gameSchedule = schedule("3000", $DefaultMiniGame, %f);
}

function sitdWhoTurn1()
{
    $light1.setDataBlock(sitd_light_danger);
    $light1.clearScopeAlways();
    $light1.setNetFlag(6, 1);
    sitdScopeToKillerAndSpecs($light1);

    $light2.setDataBlock(sitd_light_danger);
    $light2.clearScopeAlways();
    $light2.setNetFlag(6, 1);
    sitdScopeToKillerAndSpecs($light2);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.chair !$= "")
        {
            %player.clearScopeAlways();
            %player.setNetFlag(6, 1);
            %player.scopeToClient(%client);
            sitdScopeToKillerAndSpecs(%player);
        }
    }

    $DefaultMiniGame.killerPlayer.mountImage(GunImage, "0");
    fixArmReady($DefaultMiniGame.killerPlayer);

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer is choosing somebody to kill.");
    centerPrint($DefaultMiniGame.killerClient, "<font:verdana:24>\c6Kill somebody.");

    for (%i = 0; %i < ClientGroup.getCount(); %i++)
    {
        %client = ClientGroup.getObject(%i);
        
        if ((!%client.player || %client.player.chair $= "") && !%client.player.killer)
        {
            $light1.scopeToClient(%client);
            $light2.scopeToClient(%client);
        }
    }

    $DefaultMiniGame.gameSchedule = schedule("500", $DefaultMiniGame, sitdWhoTurn2);
}

function sitdScopeToKillerAndSpecs(%object)
{
    %object.scopeToClient($DefaultMiniGame.killerClient);

    for (%i = 0; %i < ClientGroup.getCount(); %i++)
    {
        %client = ClientGroup.getObject(%i);
        
        if ((!%client.player || %client.player.chair $= "") && !%client.player.killer)
        {
            %object.scopeToClient(%client);
            %object.scopeToClient(%client);
        }
    }
}

function sitdWhoTurn2()
{
    $DefaultMiniGame.continueWhoOnNextKill = "1";
    $DefaultMiniGame.killerPlayer.mountImage(sitd_gun_image, "0");
    fixArmReady($DefaultMiniGame.killerPlayer);

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer is choosing somebody to kill.");
    centerPrint($DefaultMiniGame.killerClient, "<font:verdana:24>\c6You have 10 seconds to kill somebody.");
    $DefaultMiniGame.gameSchedule = schedule("10000", $DefaultMiniGame, sitdWhoTurn2Timeout);
}

function sitdWhoTurn2Timeout()
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6The killer took too long to kill.");
    $DefaultMiniGame.continueWhoOnNextKill = "";
    $DefaultMiniGame.killerPlayer.unMountImage("0");
    fixArmReady($DefaultMiniGame.killerPlayer);
    $light1.setDataBlock(sitd_light_top);
    $light1.setScopeAlways();
    $light2.setDataBlock(sitd_light_top);
    $light2.setScopeAlways();

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.chair !$= "")
        {
            %player.setScopeAlways();
            %player.voteCount = "0";
            %player.voteTarget = "0";
            %player.canReceiveVote = "1";
            %player.canCastVote = "1";
            sitdWhoUpdateCastVote(%player);
        }
    }
    $DefaultMiniGame.killerPlayer.schedule("1500", "kill");
}

function sitdWhoTurn3()
{
    cancel($DefaultMiniGame.gameSchedule);

    $DefaultMiniGame.continueWhoOnNextKill = "";
    $DefaultMiniGame.killerPlayer.unMountImage("0");
    fixArmReady($DefaultMiniGame.killerPlayer);

    // $DefaultMiniGame.gameSchedule = schedule("500", $DefaultMiniGame, sitdWhoTurn4);
    sitdWhoTurn4();
}

function sitdWhoTurn4()
{
    $light1.setDataBlock(sitd_light_top);
    $light1.setScopeAlways();
    $light2.setDataBlock(sitd_light_top);
    $light2.setScopeAlways();

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.chair !$= "")
        {
            %player.setScopeAlways();
            %player.voteCount = "0";
            %player.voteTarget = "0";
            %player.canReceiveVote = "1";
            %player.canCastVote = "1";
            sitdWhoUpdateCastVote(%player);
        }
    }

    $DefaultMiniGame.centerPrintAll("<font:verdana:20>\c6Look at the person you think is the killer within 10 seconds.<br>The person with the most votes will die.");
    $DefaultMiniGame.gameSchedule = schedule("10000", $DefaultMiniGame, sitdWhoTurn5);
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

function sitdWhoTurn5()
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        cancel(%player.sitdWhoUpdateCastVote);
        %player.voteTarget = "";
        %player.canReceiveVote = "";
        %player.canCastVote = "";

        if (%player.chair !$= "")
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

    if ($DefaultMiniGame.currentMode !$= "whoDidIt")
        return;

    $DefaultMiniGame.gameSchedule = schedule("2000", $DefaultMiniGame, sitdWhoTurn1);
}

function sitdRussianStep1()
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

    $DefaultMiniGame.rouletteIndex %= %numAlive;
    %who = %alive[$DefaultMiniGame.rouletteIndex];
    $DefaultMiniGame.rouletteIndex++;

    %who.client.play2D(weaponSwitchSound);

    $DefaultMiniGame.rouletteVictim = %who;
    $DefaultMiniGame.centerPrintAll("<font:verdana:18>\c3" @ %who.client.name @ " \c6has 7 seconds to pull the trigger.");
    centerPrint(%who.client, "<font:verdana:24>\c0Suicide to pull the trigger. You have 7 seconds.\nYou'll get a proper revolver later.");
    $DefaultMiniGame.gameSchedule = schedule(7000, $DefaultMiniGame, sitdRussianStep2);
}

function sitdRussianStep2()
{
    %victim = $DefaultMiniGame.rouletteVictim;
    $DefaultMiniGame.rouletteVictim = "";

    if (!isObject(%victim) || %victim.isDead)
    {
        talk("victim took too long to pull the trigger yet they're already dead");
    }
    else
    {
        $DefaultMiniGame.centerPrintAll("<font:verdana:18>\c6They took too long to pull the trigger and have been killed.");
        %victim.kill();
    }

    $DefaultMiniGame.rouletteVictim = "";
    $DefaultMiniGame.gameSchedule = schedule(1000, $DefaultMiniGame, sitdRussianStep1);
}

package ShotInTheDark
{
    function Player::removeBody(%player)
    {
        %player.delete();
    }

    function MiniGameSO::addMember(%miniGame, %client)
    {
        Parent::addMember(%miniGame, %client);

        if (!%miniGame.owner && %miniGame.numMembers == 2)
            sitdPrepareGame();
    }

    function MiniGameSO::checkLastManStanding(%miniGame)
    {
        if (%miniGame.owner)
            return Parent::checkLastManStanding(%miniGame);
        
        if ($DefaultMiniGame.currentMode $= "")
            return "0";
        
        %alive = 0;
        %killerAlive = 0;
        %otherAlive = 0;
        
        for (%i = 0; %i < %miniGame.numMembers; %i++)
        {
            %client = %miniGame.member[%i];
            %player = %client.player;

            if (!%player || %player.chair $= "")
                continue;
            
            if ($DefaultMiniGame.currentMode $= "whoDidIt")
            {
                if (%player.killer)
                    %killerAlive = 1;
                else
                    %otherAlive = 1;
            }

            %alive++;
            %last = %client;
        }

        if ($DefaultMiniGame.currentMode $= "whoDidIt")
        {
            if (%killerAlive && !%otherAlive)
            {
                %miniGame.centerPrintAll("<font:verdana:24>\c0The killer (\c3" @ %miniGame.killerName @ "\c0) won!");
                return sitdEndGame();
            }

            if (!%killerAlive && %otherAlive)
            {
                %miniGame.centerPrintAll("<font:verdana:24>\c2The killer (\c3" @ %miniGame.killerName @ "\c3) has been eliminated!");
                return sitdEndGame();
            }
        }

        if (%alive == 2 && $DefaultMiniGame.currentMode !$= "russianRoulette")
        {
            cancel($DefaultMiniGame.gameSchedule);
            $DefaultMiniGame.continueWhoOnNextKill = "";
            %miniGame.centerPrintAll("<font:verdana:36><color:f07070>Duel!");

            for (%i = 0; %i < %miniGame.numMembers; %i++)
            {
                %player = %miniGame.member[%i].player;

                if (!%player || %player.chair $= "")
                    continue;
                
                %player.changeDataBlock(sitd_move_player);
                %player.playThread("0", "root");
                %player.mountImage("SwordImage", "0");
                fixArmReady(%player);
            }

            return;
        }

        if (%alive == 0)
        {
            %miniGame.centerPrintAll("<font:verdana:24>\c6Nobody wins.");
            sitdEndGame();
        }
        else if (%alive == 1)
        {
            %miniGame.centerPrintAll("<font:verdana:24>\c3" @ %last.name @ " \c6wins the game!");
            sitdEndGame();
        }

        return "0";
    }

    function MiniGameSO::pickSpawnPoint(%miniGame, %client)
    {
        if (%miniGame.owner)
            return Parent::pickSpawnPoint(%miniGame, %client);

        %x = getRandom() * 32 - 16;
        return %x @ " -8 0.01 0 0 1 0";
    }

    function Armor::onDisabled(%data, %player, %state)
    {
        if (%player.chair $= "")
            return Parent::onDisabled(%data, %player, %state);
        
        %client = %player.client;

        if (isObject(%client))
        {
            // centerPrint(%client, "");
            %client.camera.setMode("Corpse", %player);
            %client.setControlObject(%client.camera);
            %client.player = "";
            %player.client = "";
        }

        %player.playDeathCry();
        %player.emote("cubeHighExplosionProjectile", "1");
        %player.setDamageFlash("0.25");
        %player.setImageTrigger("0", "0");
        %player.playThread("0", "death1");
        %player.scheduleNoQuota(5000, "delete");

        switch$ ($DefaultMiniGame.currentMode)
        {
        case "whoDidIt":
            if ($DefaultMiniGame.continueWhoOnNextKill)
                sitdWhoTurn3();
        }

        $DefaultMiniGame.checkLastManStanding();
    }

    function serverCmdLight(%client)
    {
        if (%client.miniGame != $DefaultMiniGame)
            Parent::serverCmdLight(%client);
    }

    function serverCmdSuicide(%client)
    {
        if (%client.miniGame != $DefaultMiniGame)
            return Parent::serverCmdSuicide(%client);
        
        if ($DefaultMiniGame.currentMode !$= "russianRoulette")
            return;
        
        if (!isObject($DefaultMiniGame.rouletteVictim))
            return;
        
        if (%client.player !$= $DefaultMiniGame.rouletteVictim)
            return;
        
        cancel($DefaultMiniGame.gameSchedule);
        
        if (getRandom() < 0.8)
        {
            $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Click! Lucky. The game continues.");
            $DefaultMiniGame.gameSchedule = schedule(1000, $DefaultMiniGame, sitdRussianStep1);
            return;
        }

        serverPlay3D(gunShot1Sound, %client.player.getPosition());
        %client.player.kill();

        if ($DefaultMiniGame.currentMode $= "russianRoulette")
            $DefaultMiniGame.gameSchedule = schedule(1000, $DefaultMiniGame, sitdRussianStep1);
    }
};

activatePackage("ShotInTheDark");