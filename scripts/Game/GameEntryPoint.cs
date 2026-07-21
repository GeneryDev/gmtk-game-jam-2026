using GDF.IO;
using GDF.Scenes;
using GDF.Util;
using Godot;

namespace Game;

public partial class GameEntryPoint : Node
{
    [Export] public PackedScene MainScene;
    
    public override void _Ready()
    {
        var instantiated = MainScene?.GdfInstantiate();
        if (instantiated != null)
        {
            CallDeferred(MethodName.ToMainScene);
        }
    }

    public void ToMainScene()
    {
        SceneManager.ChangeScene(new SceneChangeRequest()
        {
            SceneReference = new ResourceReference(MainScene.ResourcePath)
        });
    }
}