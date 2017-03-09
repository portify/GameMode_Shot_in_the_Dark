function sitdLightOff()
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
}

function sitdLightOn()
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
            %player.setScopeAlways();
    }
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
        }
    }
}