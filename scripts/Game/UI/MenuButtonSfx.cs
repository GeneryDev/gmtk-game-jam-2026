using GDF.Audio;
using Godot;
using System;

public partial class MenuButtonSfx : Node
{
	[Export] public GdfAudioPlayer ClickPlayer;
	[Export] public GdfAudioPlayer StartPlayer;
	[Export] public GdfAudioPlayer HoverPlayer;
	[Export] public String StartButtonName = "Play Button";
	public void Clicked()
	{
		if (GetParent().Name == StartButtonName)
		{
			StartPlayer.playing = true;
		}
		else
		{
			ClickPlayer.playing = true;
		}
		
	}
	public void Hover()
	{
		HoverPlayer.playing = true;   
	}
}
