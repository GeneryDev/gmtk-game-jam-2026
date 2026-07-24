using GDF.IO;
using GDF.Scenes;
using GDF.Scenes.Transitions;
using Godot;

namespace Game.UI;

public partial class MenuFunctions : Node
{
    public void Quit()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
        GetTree().Quit();
    }

    public void ChangeScene(ResourceReference sceneReference, ScreenTransitionReference screenTransitionReference = null)
    {
        SceneManager.TransitionToScene(sceneReference, screenTransitionReference);
    }
}