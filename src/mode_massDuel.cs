function SitdMassDuel::onAdd(%script)
{
    %script.name = "Mass Duel";
}

function SitdMassDuel::onRemove(%script)
{
    cancel(%script.event);
}

function SitdMassDuel::onStart(%script)
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers && %i < 16; %i++)
	{
		%client = $DefaultMiniGame.member[%i];

        if (%client.player.chair !$= "")
            %client.player.rouletteLives = getRandom(1, 6);
    }

    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Be the last man standing!");
    %script.event = %script.schedule(3000, step1);
}

function SitdMassDuel::onDeath(%script)
{
}

function SitdMassDuel::step1(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Get ready...");
    %script.event = %script.schedule(3000, step2);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %player = $DefaultMiniGame.member[%i].player;

        if (!%player || %player.chair $= "")
            continue;
        
        %player.changeDataBlock(sitd_move_player);
        %player.playThread("0", "root");
        %player.setArmThread("look");
    }
}

function SitdMassDuel::step2(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6On your marks...");
    %script.event = %script.schedule(3000, step3);
}

function SitdMassDuel::step3(%script)
{
    $DefaultMiniGame.centerPrintAll("<font:verdana:24>\c6Fight to the death!", 1);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %player = $DefaultMiniGame.member[%i].player;

        if (!%player || %player.chair $= "")
            continue;
        
        %player.mountImage("GunImage", "0");
        fixArmReady(%player);
    }
}