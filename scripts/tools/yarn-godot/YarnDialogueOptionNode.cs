using Godot;
using Events;

public class YarnDialogueOptionNode : Button {
    int optionId;
    public void Init(int optionId, string text) {
        this.optionId = optionId;
        this.Text = text;
        this.Connect("pressed", this, nameof(OnPressed) );
    }

    public override void _EnterTree() {
        this.SubscribeOnce<OptionSelectedMessage>( _ => QueueFree() );
    }

    public override void _ExitTree() {
    }
    
    void OnPressed()
    {
        this.SendEvent( new OptionSelectedMessage { ID = optionId } );
    }

}
