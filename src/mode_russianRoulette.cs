function SitdRussianRoulette::onAdd(%script)
{
    %script.name = "Russian Roulette";
    %script.index = "0";
}

function SitdRussianRoulette::onRemove(%script)
{
    cancel(%script.event);
}

function SitdRussianRoulette::onStart(%script)
{
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

    %script.index %= %numAlive;
    %who = %alive[%script.index];
    %script.index++;

    %who.client.play2D(weaponSwitchSound);

    %script.victim = %who;
    $DefaultMiniGame.centerPrintAll("<font:verdana:18>\c3" @ %who.client.name @ " \c6has 5 seconds to pull the trigger.");
    centerPrint(%who.client, "<font:verdana:24>\c0Suicide to pull the trigger. You have 5 seconds.\nYou'll get a proper revolver later.");
    %script.event = %script.schedule(5000, step1Timeout);
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
