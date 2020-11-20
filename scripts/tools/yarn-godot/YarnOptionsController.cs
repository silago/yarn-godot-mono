using System;
using Godot;
using Events;
using System.Collections.Generic;
using Yarn.Compiler;
public class YarnOptionsController : Node {
	[Export]
	PackedScene _optionNode;

	YarnDialogueOptionNode InstantiateNode()
	{
		return (YarnDialogueOptionNode) _optionNode.Instance();
	}

	public override void _EnterTree() 
	{
		this.Subscribe<OptionsProvidedMessage>(OnOptionsProvided);
	}

	private void OnOptionsProvided(OptionsProvidedMessage obj)
	{
		foreach(KeyValuePair<int, StringInfo> kvp in obj.Options) {
			var option =InstantiateNode(); 
			option.Init(kvp.Key, kvp.Value.text);
		}
	}
	public override void _ExitTree() 
	{
		this.Unsubscribe<OptionsProvidedMessage>(OnOptionsProvided);
	}
}
