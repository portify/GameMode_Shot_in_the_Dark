function r(%p)
{
	setModPaths(getModPaths());
	if (%p !$= "")
		exec("./src/" @ %p @ ".cs");
	else
		exec("./server.cs");
}

exec("./src/datas.cs");
exec("./src/gun.cs");
exec("./src/stage.cs");
exec("./src/util.cs");
exec("./src/mode_whoDidIt.cs");
exec("./src/mode_russianRoulette.cs");
exec("./src/game.cs");

if (!isObject(MissionCleanup))
	schedule("0", "0", "createStage");