this is only the plugin that handles voice, the actual skin is in a separate dll completely made with the skin builder in unity

if you want to know how to do that, start here: https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Skins/Creating-skin-for-vanilla-characters-with-custom-model/



why separate plugins? to make the skin work client sided with you want

this way, if you uninstall BaseVoiceoverLib(which all players need to have) the actual skin plugin will load anyways, making it vanilla compatible without the voices



by the way you should also go see the github for BaseVoiceoverLib by Moffein if you want to make a skin with voices: https://github.com/Moffein/BaseVoiceoverLib/tree/master

theyre the GOAT that made all this voice shenanigans possible(or atleast much easier to do)
