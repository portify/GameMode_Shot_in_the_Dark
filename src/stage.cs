function createShape(%data, %position, %rotation, %scale, %color)
{
	if (%color $= "") %color = "0.5 0.3 0.1 1";
	%shape = new StaticShape()
	{
		datablock = %data;
		position = %position;
		rotation = %rotation;
		scale = %scale;
	};
	%shape.setNodeColor("ALL", %color);
	return %shape;
}

function createLight(%data, %position)
{
	return new fxLight()
	{
		datablock = %data;
		position = %position;
	};
}

function createStage()
{
    if (isObject(StageGroup))
        StageGroup.deleteAll();
    else
	    new SimGroup(StageGroup);
    %oldInstantGroup = $instantGroup;
    $instantGroup = StageGroup;
	createShape(sitd_cube, "0 8 0.1", "", "32 24 0.2", "0.6 0.2 0.1 1");
	createShape(sitd_cube, "0 -4 8", "", "32 0.2 16").hideNode("ALL");
	createShape(sitd_cube, "0 20 8", "", "32 0.2 16").hideNode("ALL");
	createShape(sitd_cube, "-16 8 8", "", "0.2 24 16").hideNode("ALL");
	createShape(sitd_cube, "16 8 8", "", "0.2 24 16").hideNode("ALL");
	createShape(sitd_table, "-0.15 8 1.95");
	$chair0 = createShape(sitd_chair, "-4 0 0.2");
	$chair1 = createShape(sitd_chair, "0 0 0.2");
	$chair2 = createShape(sitd_chair, "4 0 0.2");
	$chair3 = createShape(sitd_chair, "8 1 0.2", "0 0 -1 30");
	$chair4 = createShape(sitd_chair, "11 4 0.2", "0 0 -1 60");
	$chair5 = createShape(sitd_chair, "12 8 0.2", "0 0 -1 90");
	$chair6 = createShape(sitd_chair, "11 12 0.2", "0 0 -1 120");
	$chair7 = createShape(sitd_chair, "8 15 0.2", "0 0 -1 150");
	$chair8 = createShape(sitd_chair, "4 16 0.2", "0 0 -1 180");
	$chair9 = createShape(sitd_chair, "0 16 0.2", "0 0 -1 180");
	$chair10 = createShape(sitd_chair, "-4 16 0.2", "0 0 -1 180");
	$chair11 = createShape(sitd_chair, "-8 15 0.2", "0 0 -1 210");
	$chair12 = createShape(sitd_chair, "-11 12 0.2", "0 0 -1 240");
	$chair13 = createShape(sitd_chair, "-12 8 0.2", "0 0 -1 270");
	$chair14 = createShape(sitd_chair, "-11 4 0.2", "0 0 -1 300");
	$chair15 = createShape(sitd_chair, "-8 1 0.2", "0 0 -1 330");
	$light1 = createLight(sitd_light_top, "-4 8 8.2");
    $light1.setScopeAlways();
	$light2 = createLight(sitd_light_top, "4 8 8.2");
    $light2.setScopeAlways();
	//createLight(CyanLight, "11 4 6");
}