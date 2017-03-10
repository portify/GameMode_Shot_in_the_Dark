package SitdUtil
{
    function centerPrint(%client, %text, %time, %what, %withTimer)
    {
        if (!%withTimer)
            cancel(%client.centerPrintWithTimer);

        Parent::centerPrint(%client, %text, %time, %what);
    }
};

activatePackage("SitdUtil");

function centerPrintWithTimer(%client, %text, %time)
{
    cancel(%client.centerPrintWithTimer);
    %timerText = "\n<font:verdana:32>\c3" @ %time @ "s";
    centerPrint(%client, %text @ %timerText, "", "", "1");
    if (%time > 1)
        %client.centerPrintWithTimer = schedule("1000", %client, "centerPrintWithTimer", %client, %text, %time - 1);
}

function sitdLightOff()
{
    $lightHL.setEnable(0);

    $light1.setEnable(1);
    $light1.setDataBlock(sitd_light_danger);
    // $light1.clearScopeAlways();
    $light1.setNetFlag(6, 1);
    // sitdScopeToKillerAndSpecs($light1);
    sitdClearScopeAllButKiller($light1);

    $light2.setEnable(1);
    $light2.setDataBlock(sitd_light_danger);
    // $light2.clearScopeAlways();
    $light2.setNetFlag(6, 1);
    // sitdScopeToKillerAndSpecs($light2);
    sitdClearScopeAllButKiller($light2);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.chair !$= "")
        {
            // %player.clearScopeAlways();
            %player.setNetFlag(6, 1);
            sitdClearScopeAllButKiller(%player);
            %player.scopeToClient(%client);
            // sitdScopeToKillerAndSpecs(%player);
        }
    }
}

function sitdLightOn()
{
    $light1.setEnable(1);
    $light1.setDataBlock(sitd_light_top);
    // $light1.setScopeAlways();
    $light1.setNetFlag(6, 0);
    scopeToAll($light1);
    $light2.setEnable(1);
    $light2.setDataBlock(sitd_light_top);
    // $light2.setScopeAlways();
    $light2.setNetFlag(6, 0);
    scopeToAll($light2);
    $lightHL.setEnable(0);

    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.chair !$= "")
        {
            // %player.setScopeAlways();
            %player.setNetFlag(6, 0);
            scopeToAll(%player);
        }
    }
}

// function sitdScopeToKillerAndSpecs(%object)
// {
//     %script = $DefaultMiniGame.currentMode;
    
//     if (isObject(%script.killerClient))
//         %object.scopeToClient(%script.killerClient);

//     for (%i = 0; %i < ClientGroup.getCount(); %i++)
//     {
//         %client = ClientGroup.getObject(%i);
        
//         if ((!%client.player || %client.player.chair $= "") && !%client.player.killer)
//         {
//             %object.scopeToClient(%client);
//         }
//     }
// }

function sitdClearScopeAllButKiller(%object)
{
    for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
    {
        %client = $DefaultMiniGame.member[%i];
        %player = %client.player;

        if (%player.chair !$= "" && !%player.killer)
            %object.clearScopeToClient(%client);
    }
}

function scopeToAll(%object)
{
    %i = ClientGroup.getCount();

    while (%i-- >= 0)
    {
        %client = ClientGroup.getObject(%i);

        if (%client.hasSpawnedOnce)
            %object.scopeToClient(%client);
    }
}