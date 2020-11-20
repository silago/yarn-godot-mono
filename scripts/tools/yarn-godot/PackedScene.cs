using Godot;
public class PackedScene<T>: PackedScene  where T : Node{
    public T Instance() {
        return (T)base.Instance();
    }
}
