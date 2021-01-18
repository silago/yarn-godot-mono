using Events;
using System.Collections.Generic;
using Yarn.Compiler;

public class NewLineMessage : Message {
    public string Text { get; set;}
}

public class StartDialogueMessage : Message {
    public string NodeName { get; set;}
}

public class OptionSelectedMessage : Message {
    public int ID { get; set; }
}

public class OptionsProvidedMessage : Message {
    public Dictionary<int,StringInfo> Options { get; set; } 
}
