using GDF.Data;
using GDF.Input;
using GDF.Util;
using Godot;

namespace Game.UI;

[Tool]
[GlobalClass]
public partial class ButtonExtension : Button, IDataContext
{
    private static readonly StringName ThemePropertyNameNormal = "normal";
    private static readonly StringName ThemePropertyNameHover = "hover";
    private static readonly StringName ThemePropertyNamePressed = "pressed";
    private static readonly StringName ThemePropertyNameDisabled = "disabled";
    
    [Signal]
    public delegate void UpdatedEventHandler();

    [Signal]
    public delegate void SubmittedEventHandler();

    [Signal]
    public delegate void PlayerSubmittedEventHandler(int playerId);

    [Export(PropertyHint.MultilineText)]
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            if (_buttonText == value) return;
            _buttonText = value;
            Update();
            UpdateContentSizing();
        }
    }

    [Export]
    public Node DataContext
    {
        get => _contextNode;
        set
        {
            (_contextNode as IDataContext)?.DisconnectUpdateSignal(new Callable(this, MethodName.OnContextUpdated));
            _contextNode = value;
            (_contextNode as IDataContext)?.ConnectUpdateSignal(new Callable(this, MethodName.OnContextUpdated));
            OnContextUpdated();
        }
    }

    [Export]
    public GdfInputAction ShortcutAction
    {
        get => _shortcutAction;
        set
        {
            if (_shortcutAction == value) return;
            _shortcutAction = value;
            Update();
        }
    }

    [Export] public bool ShortcutAsIconOnly = false;

    [ExportGroup("Content")]
    [Export] public Control ButtonContentControl = null;
    [Export] public bool SyncMinimumSizeToContent = true;
    [Export] public Vector2 ButtonCustomMinimumSize = Vector2.Zero;

    private string _buttonText = "";
    private Node _contextNode;
    private GdfInputAction _shortcutAction;
    private ParsedDataQuery _textQueryCache;
    private ValueChangeTracker<bool> _disabledChange;

    private void OnContextUpdated()
    {
        Update();
    }

    private void Update()
    {
        EmitSignalUpdated();
    }

    public void Submit(int playerId)
    {
        if (Disabled) return;
        EmitSignalPlayerSubmitted(playerId);
        EmitSignalSubmitted();
    }

    public override void _Ready()
    {
        base._Ready();
        CallDeferred(MethodName.ConnectResizingSignals);
    }

    private void ConnectResizingSignals()
    {
        if (ButtonContentControl == null) return;
        this.Resized += UpdateContentSizing;
        this.ButtonDown += UpdateContentSizing;
        this.ButtonUp += UpdateContentSizing;
        this.MouseEntered += UpdateContentSizing;
        this.MouseExited += UpdateContentSizing;
        ButtonContentControl.MinimumSizeChanged += UpdateContentSizing;
        UpdateContentSizing();
    }

    private void UpdateContentSizing()
    {
        if (ButtonContentControl is not { } control) return;
        
        GetContentMargins(out float marginLeft, out float marginTop, out float marginRight, out float marginBottom);

        var newContentRect = new Rect2(Vector2.Zero, this.GetSize())
            .GrowIndividual(-marginLeft, -marginTop, -marginRight, -marginBottom);

        control.Position = newContentRect.Position;
        control.Size = newContentRect.Size;

        if(SyncMinimumSizeToContent)
            this.CustomMinimumSize = (ButtonContentControl.GetCombinedMinimumSize() + new Vector2(marginLeft + marginRight, marginTop + marginBottom)).Max(ButtonCustomMinimumSize);
    }

    private void GetContentMargins(out float marginLeft, out float marginTop, out float marginRight, out float marginBottom)
    {
        var statePropertyName = ThemePropertyNameNormal;
        if (this.ButtonPressed)
        {
            if(KeepPressedOutside || new Rect2(0, 0, Size).HasPoint(GetLocalMousePosition()))
                statePropertyName = ThemePropertyNamePressed;
        }

        var themeType = ThemeTypeVariation;
        if (ThemeTypeVariation.IsNullOrEmpty()) themeType = nameof(Button);

        var stylebox = this.GetThemeStylebox(statePropertyName, themeType);
        marginLeft = marginRight = marginTop = marginBottom = 0;
        if (stylebox != null)
        {
            marginLeft = stylebox.GetMargin(Side.Left);
            marginRight = stylebox.GetMargin(Side.Right);
            marginTop = stylebox.GetMargin(Side.Top);
            marginBottom = stylebox.GetMargin(Side.Bottom);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(_disabledChange.HasChanged(Disabled))
            EmitSignalUpdated();
    }

    public StringName UpdatedSignalName => SignalName.Updated;
    public IDataContext ParentContext => _contextNode as IDataContext;

    bool IDataContext.GetContextVariable(string key, string input, ref Variant output,
        IDataQueryOptions options)
    {
        switch (key)
        {
            case "shortcut_action":
            {
                if (input.Length > 0 && ShortcutAsIconOnly && input != "icon")
                {
                    output = default;
                    return true;
                }

                output = ShortcutAction;
                return true;
            }
            case "disabled":
            case "is_disabled":
            {
                return this.OutputBooleanVariable(Disabled, ref output, input);
            }
        }

        return false;
    }

    bool IDataContext.GetContextString(string key, string input, ref string replacement,
        IDataQueryOptions options)
    {
        switch (key)
        {
            case "button_text":
            {
                replacement = this.Format(ButtonText, ref _textQueryCache);
                return true;
            }
        }

        return false;
    }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        var propName = property["name"].AsStringName();
        var usage = property["usage"].As<PropertyUsageFlags>();

        if (propName == Button.PropertyName.Text) usage &= ~PropertyUsageFlags.Editor;
        property["usage"] = Variant.From(usage);
    }
}