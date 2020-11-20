using Godot;
using System;
using System.Collections.Generic;
using Yarn;
using Yarn.Compiler;
using Events;

public class YarnController : Godot.Node {
	[Export]
	public Godot.Collections.Array<TextFile> FF2;

	[Export(PropertyHint.File)]
	public Godot.Collections.Array<Resource> FilePathes2;

	[Export]
	public Godot.Collections.Array<string> FilePathes;

	Dialogue dialogue;
	bool isWaitingForAnswer = false;
	IDictionary<string, StringInfo> stringTable;

	public override void _Ready() {
		LoadCompileAndLoad();
	}

	void LoadCompileAndLoad() 
	{
		var compiledProgramms = new List<Program>();
		foreach(string path in FilePathes) {
			Program program;
			var file = new Godot.File();
			file.Open(path, File.ModeFlags.Read);
			var absolutePath = file.GetPathAbsolute();
			Compiler.CompileFile(absolutePath, out program, out IDictionary<string, StringInfo> strings);
			foreach(var str in strings) {
			   stringTable.Add(str.Key, str.Value); 
			}
			compiledProgramms.Add(program);
		}
		var resultProgram = Program.Combine(compiledProgramms.ToArray());
		dialogue = InitDialogue();
		InitFunctions(dialogue);

		this.Subscribe<StartDialogueMessage>(OnStartDialogueMessage);
		this.Subscribe<OptionSelectedMessage>(OnOptionSelectMessage);
	}

	private void OnOptionSelectMessage(OptionSelectedMessage obj)
	{
		OnOptionSelected(obj.ID);
	}
	private void OnStartDialogueMessage(StartDialogueMessage obj)
	{
		Run(obj.NodeName);
	}
	public override void _ExitTree() 
	{
		this.Unsubscribe<StartDialogueMessage>(OnStartDialogueMessage);
	}

	Dialogue InitDialogue() {
		var variableStorage = new DictStorage();
		var dialogue = new Dialogue(variableStorage);

		dialogue.dialogueCompleteHandler = dialogueCompleteHandle;
		dialogue.nodeCompleteHandler = NodeCompleteHandler;
		dialogue.nodeStartHandler = NodeStartHandler;
		dialogue.commandHandler = CommandHandler; 
		dialogue.optionsHandler = OptionsHandler;
		dialogue.lineHandler = LineHandler;
		dialogue.LogDebugMessage = (msg) => GD.Print(msg);
		dialogue.LogErrorMessage = (msg) => GD.PrintErr(msg);
		return dialogue;
	}

	void InitFunctions(Dialogue dialogue)
	{
		AddFunction(dialogue, "visited",1,Visited); 
	}

	public void Run(string node)
	{
		dialogue.SetNode(node);
		dialogue.Continue();
	}


	private Dialogue.HandlerExecutionType NodeStartHandler(string startedNodeName)
	{
		return Dialogue.HandlerExecutionType.ContinueExecution;
	}

	private void dialogueCompleteHandle() => GD.Print("dialogue complete");

	private object Visited(params Value[] p)
	{
		return false;
	}

	public override void _Input(InputEvent ev) {
		if (isWaitingForAnswer == false) {
			return;
		}
		if ( ev is InputEventKey ke && ke.Pressed) {
			var textInput = ke.AsText();
			int num;
			if (int.TryParse(textInput, out num)) {
				isWaitingForAnswer = false;
				OnOptionSelected(num);
			}
		}
	}

	private void AddFunction(Dialogue dialogue, string name, int parameterCount, ReturningFunction implementation)
		=> dialogue.library.RegisterFunction(name, parameterCount, implementation);

	private Dialogue.HandlerExecutionType NodeCompleteHandler(string completedNodeName)
	{
		//throw new NotImplementedException();
		GD.Print("node completed ", completedNodeName);
		return Dialogue.HandlerExecutionType.ContinueExecution;

	}
	Dialogue.HandlerExecutionType CommandHandler(Command command)
	{
		throw new NotImplementedException();
	}

	void OptionsHandler(OptionSet options) {
		isWaitingForAnswer = true;
		Dictionary<int,StringInfo> optionsStrings = new Dictionary<int,StringInfo>();
		foreach(OptionSet.Option opt in options.Options) {
			stringTable.TryGetValue(opt.Line.ID, out var info);
			optionsStrings[opt.ID]=info;
		}


		this.SendEvent( new OptionsProvidedMessage { Options=optionsStrings } );
	}

	void OnOptionSelected(int optionId) {
		GD.Print(">",optionId);
		dialogue.SetSelectedOption(optionId);
		dialogue.Continue();
	}
	
	Dialogue.HandlerExecutionType LineHandler(Line line) {
		stringTable.TryGetValue(line.ID, out var info);
		var text = info.text;
		GD.Print(text);
		//return Dialogue.HandlerExecutionType.PauseExecution;
		return Dialogue.HandlerExecutionType.ContinueExecution;
	}

}
