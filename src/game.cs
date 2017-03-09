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
	}
}

function sitdEndGame()
{
    if (isObject($DefaultMiniGame.currentMode))
        $DefaultMiniGame.currentMode.delete();

    cancel($DefaultMiniGame.gameSchedule);
    cancel($DefaultMiniGame.restartSchedule);

    $DefaultMiniGame.restartSchedule = schedule("5000", $DefaultMiniGame, sitdPrepareGame);
}

function sitdPrepareGame()
{
    if (isObject($DefaultMiniGame.currentMode))
        $DefaultMiniGame.currentMode.delete();

    cancel($DefaultMiniGame.gameSchedule);
    cancel($DefaultMiniGame.restartSchedule);

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

    $DefaultMiniGame.currentMode = new ScriptObject()
    {
        class = "SitdWhoDidIt";
        // class = "SitdRussianRoulette";
    };

    seatPlayers();

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Welcome to port's Shot In The Dark\n\c6Next up: \c3" @ $DefaultMiniGame.currentMode.name);
    $DefaultMiniGame.gameSchedule = $DefaultMiniGame.currentMode.schedule(3000, onStart);
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
        
        %script = $DefaultMiniGame.currentMode;
        
        if (!isObject(%script))
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
            
            if (%script.class $= "SitdWhoDidIt")
            {
                if (%player.killer)
                    %killerAlive = 1;
                else
                    %otherAlive = 1;
            }

            %alive++;
            %last = %client;
        }

        if (%script.class $= "SitdWhoDidIt")
        {
            if (%killerAlive && !%otherAlive)
            {
                %miniGame.centerPrintAll("<font:verdana:24>\c0The killer (\c3" @ %script.killerName @ "\c0) won!");
                return sitdEndGame();
            }

            if (!%killerAlive && %otherAlive)
            {
                %miniGame.centerPrintAll("<font:verdana:24>\c2The killer (\c3" @ %script.killerName @ "\c2) has been eliminated!");
                return sitdEndGame();
            }
        }

        if (%alive == 2 && %script.class !$= "SitdRussianRoulette")
        {
            sitdLightOn();
            cancel($DefaultMiniGame.gameSchedule);
            cancel(%script.event);
            %script.waitingForKill = "";
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

        $DefaultMiniGame.currentMode.onDeath();
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

        %script = $DefaultMiniGame.currentMode;
        
        if (%script.class !$= "SitdRussianRoulette")
            return;
        
        if (!isObject(%script.victim))
            return;
        
        if (%client.player !$= %script.victim)
            return;
        
        cancel(%script.event);
        
        if (getRandom() < 0.8)
        {
            $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Click! Lucky. The game continues.");
            %script.event = %script.schedule(1000, step1);
            return;
        }

        serverPlay3D(gunShot1Sound, %client.player.getPosition());
        %client.player.kill();

        %script.event = %script.schedule(1000, step1);
    }
};

activatePackage("ShotInTheDark");