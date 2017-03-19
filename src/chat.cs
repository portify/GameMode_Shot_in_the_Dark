package SitdChat
{
	function serverCmdMessageSent(%client, %text)
	{
		%player = %client.player;

		if (%client.miniGame != $DefaultMiniGame)
			return Parent::serverCmdMessageSent(%client, %text);

		%text = getSubStr(trim(stripMLControlChars(%text)), 0, $Pref::Server::MaxChatLen);

		if (%text $= "")
			return;

		if (%player.chair $= "")
		{
			for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
			{
				%member = $DefaultMiniGame.member[%i];

				if (!%member.player || %member.player.chair $= "")
				{
					messageClient(%member, '', '<color:808080>%1\c6<color:b0b0b0>: %2', %client.getPlayerName(), %text);
				}
			}
			return;
		}

		serverPlay3D(SitdChatSound, %player.getHackPosition());

		%shape = new Item()
		{
			datablock = SitdEmptyFloatItem;
			position = VectorAdd(%player.position, "0 0 2");
		};

		%shape.setCollisionTimeout(%player);
		%shape.setShapeName(%text);
		%shape.setVelocity("0 0 1");
		%shape.deleteSchedule = %shape.schedule("3000", "delete");

		messageAll('', '<color:80ff80>%1\c6<color:f0fff0>: %2', %client.getPlayerName(), %text);
	}
};

activatePackage("SitdChat");