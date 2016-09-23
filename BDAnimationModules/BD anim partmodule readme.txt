------FIRESPITTER REPLACEMENT MODULES-----------

MODULE
{
name = BDFSanimateGeneric
animationName = NAME_OF_ANIMATION_HERE //put animation name here
startEventGUIName = Extend Antennae //put text here for right click menu
endEventGUIName = Retract Antennae //put text here for right click menu
toggleActionName = Toggle Antennae //put text here for action group menu
startDeployed = False
// Animation Layer. If you have more than one animation on a part, this must be unique for each one in order for them to not interrupt each other. If the stock engine heat animation module is used, that’s probably on layer 0, so make this 1 or higher.
layer = 1
playAnimationOnEditorSpawn = False
useActionEditorPopup = True
// If you have more than one BDFSanimateGeneric on a part, give each a unique ID to. This makes the popup menu appear in a separate location for each module, and not steal each others windows. values: 0-9.
moduleID = 0
}



MODULE
{
name = BDFStextureSwitch2
moduleID = 0

objectNames = MESH_OBJECT_NAMES_HERE //separate names with a semicolon
 
textureRootFolder = FOLDER_PATH_IN_GAMEDATA //path to texture file inside gamedata folder

textureNames = NAMES_OF_TEXTURE_FILES //separate with semicolon, do not include file extension
textureDisplayNames = NAMES_OF_TEXTURES //separate with semicolon, texture names in right click menu

nextButtonText = Change Texture
prevButtonText = Previous Texture
statusText = Current Texture
switchableInFlight = false
repaintableEVA = false
showPreviousButton = false
updateSymmetry = true
showInfo = true
}



MODULE
{
	name = FSmeshSwitch
	// If there are multiple instances of this module in a part, they must each have a unique module ID to avoid conflicts
	moduleID = 0
	buttonName = Next Mesh //button display name for mesh switch
	previousButtonName = Prev Mesh //button display name for mesh switch

	// The different variants can have a display name that explains what they are
	objectDisplayNames = NAMES_OF_MESHES //separate with semicolon, mesh names in right click menu

	showPreviousButton = true
	useFuelSwitchModule = false

	//Separate objects that belong together with a comma.
	objects = meshexample1, meshexample2; meshexample3; meshexample4, meshexample5 //meshes you want combined separated by commas, meshes are separated into groups using semicolon

	updateSymmetry = true
	affectColliders = true
	showInfo = true
	debugMode = false
}



----------BD ANIMATION MODULE-----------------

MODULE
{
	name = AnimatedEngine
	EngineAnimationName = Deploy //name of animation here
	WaitForAnimation = 0.73 //how long to wait for engine to start, it can start during or after the animation finishes
}


------------ANIMATED DECOUPLER-------------------


MODULE
	{
	    name = BDAnimatedDec
	    ejectionForce = 50
	    explosiveNodeID = top
	    staged = true
	    animationName = Deploy //animation name
		waitForAnimation = false // (If true, then delay decoupling until animation has finished playing)
		layer = 1 // Animation's layer will be set to this. Helps prevent multiple animations on one part interfering with each other
	}


