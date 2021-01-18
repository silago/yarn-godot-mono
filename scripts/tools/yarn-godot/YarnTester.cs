using Godot;
using System;
using Events;

public class YarnTester : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	[Export]
	String start;


	public override void _Ready()
	{
		GD.Print("!!!!!!!!!");
		this.Subscribe<OptionsProvidedMessage>(OnOptionsProvided);
		this.Subscribe<NewLineMessage>(OnNewLine);
		this.SendEvent(new StartDialogueMessage { NodeName = start });	
	}

	private void OnNewLine(NewLineMessage obj)
	{
		GD.Print(">>>", obj.Text);
	}

	private void OnOptionsProvided(OptionsProvidedMessage obj)
	{
		foreach(var item in obj.Options) {
			GD.Print(item.Key, " : ", item.Value);
		}
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
  {
	  if(Input.IsKeyPressed((int)KeyList.Key0)) {
		  this.SendEvent(new OptionSelectedMessage { ID = 0 });
	  }

	  if(Input.IsKeyPressed((int)KeyList.Key1)) {
		  this.SendEvent(new OptionSelectedMessage { ID = 1 });
	  }
	  
  }
}
