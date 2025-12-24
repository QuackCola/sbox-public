
namespace Editor.ProjectSettingPages;

public enum ShaderGlobalType
{
	Int,
	Bool,
	Float,
	Vector2,
	Vector3,
	Vector4,
	Color,
	Texture
}

[Title( "Shader Globals" )]
internal sealed class ShaderGlobalsCategory : ProjectInspector.Category
{
	LineEdit LineEdit;
	ShaderGlobalsList ShaderGlobalsList;

	ShaderGlobalsSettings Settings;

	ShaderGlobalType CurrentGlobalType;

	public override void OnInit( Project project )
	{
		base.OnInit( project );

		Settings = EditorUtility.LoadProjectSettings<ShaderGlobalsSettings>( "ShaderGlobals.config" );

		var header = BodyLayout.AddRow( 0, false );
		header.Spacing = 4;

		LineEdit = new LineEdit() { PlaceholderText = "Add New Shader Global..." };
		
		header.Add( LineEdit );
		
		var globalTypeButton = new ShaderGlobalTypeButton( ShaderGlobalType.Float );
		globalTypeButton.OnGlobalTypeChosen += ( globalType ) =>
		{
			CurrentGlobalType = globalType;
		};
		globalTypeButton.FixedHeight = Theme.ControlHeight;
		
		header.Add( globalTypeButton );
		
		var addButton = new Button.Primary( "Add", "new_label" );
		addButton.Enabled = true;
		addButton.ToolTip = $"Add new shader global";
		addButton.Clicked += AddGlobal;
		addButton.FixedHeight = Theme.ControlHeight;
		
		header.Add( addButton );
		
		ShaderGlobalsList = new ShaderGlobalsList( null, Settings );
		
		BodyLayout.Add( ShaderGlobalsList );

		BodyLayout.AddStretchCell( 2 );
	}

	private void AddGlobal()
	{
		// TODO
	}

	public override void OnSave()
	{
		EditorUtility.SaveProjectSettings( Settings, "ShaderGlobals.config" );

		base.OnSave();
	}
}

internal sealed class ShaderGlobalsList : Widget
{
	Layout Content;
	ScrollArea ScrollArea;
	
	List<ShaderGlobalEntry> ShaderGlobalEntries;
	ShaderGlobalsSettings Settings;

	public Action<ShaderGlobal> OnGlobalUpdated { get; set; }

	public ShaderGlobalsList( Widget parent, ShaderGlobalsSettings settings ) : base( parent, false )
	{
		Layout = Layout.Column();
		Layout.Margin = 16;
		Layout.Spacing = 8;

		Settings = settings;
		ShaderGlobalEntries = new List<ShaderGlobalEntry>();

		Layout.AddStretchCell();

		{
			ScrollArea = new ScrollArea( this );
			ScrollArea.Canvas = new Widget();
			ScrollArea.Canvas.Layout = Layout.Column();
			ScrollArea.Canvas.VerticalSizeMode = SizeMode.CanGrow;
			ScrollArea.Canvas.HorizontalSizeMode = SizeMode.Default;

			Content = Layout.Column();
			Content.Margin = 4;
			Content.AddStretchCell();

			ScrollArea.Canvas.Layout.Add( Content );
			ScrollArea.Canvas.Layout.AddStretchCell();

			Layout.Add( ScrollArea );
		}

		UpdateList();

	}

	private void UpdateList()
	{
		Content?.Clear( true );
		ShaderGlobalEntries.Clear();

		if ( Settings.ShaderGlobals.Any() )
		{
			foreach ( var global in Settings.ShaderGlobals )
			{
				var globalEntry = Content.Add( new ShaderGlobalEntry( this, global ) );

				globalEntry.OnGlobalUpdated += var =>
				{
					OnGlobalUpdated?.Invoke( var );
				};

				ShaderGlobalEntries.Add( globalEntry );
			}
		}
	}

	protected override void OnPaint()
	{
		Paint.ClearPen();
		Paint.SetBrush( Theme.ControlBackground );
		Paint.DrawRect( LocalRect, 4 );

		base.OnPaint();
	}

	class ShaderGlobalEntry : Widget
	{
		ControlSheet ControlSheet;
		Layout ButtonLayout;
		Drag dragData;
		ShaderGlobalsList ParentList;

		SerializedObject SerializedObject;

		public ShaderGlobal ShaderGlobal { get; }
		
		public Action<ShaderGlobal> OnGlobalUpdated { get; set; }

		public ShaderGlobalEntry( ShaderGlobalsList parentList, ShaderGlobal shaderGlobal )
		{
			Layout = Layout.Row();
			ControlSheet = new ControlSheet();
			ButtonLayout = Layout.Row();
			MaximumHeight = 200;
			IsDraggable = true;
			ParentList = parentList;
			ShaderGlobal = shaderGlobal;
			SerializedObject = shaderGlobal.GetSerialized();

			if ( SerializedObject is null )
				return;

			SerializedObject.OnPropertyFinishEdit += x =>
			{
				if ( x.Name == "Name" )
				{
					var updatedVarName = x.GetValue<string>();

					x.SetValue( $"{updatedVarName}" );
				}

				OnGlobalUpdated?.Invoke( ShaderGlobal );
			};

			ControlSheet.AddObject( SerializedObject );

			Layout.Add( ControlSheet );
		}

		protected override void OnPaint()
		{

			base.OnPaint();
		}
	}
}

file class ShaderGlobalTypeButton : Button
{
	private List<ShaderGlobalType> _globalTypes = new List<ShaderGlobalType>()
	{
		ShaderGlobalType.Int,
		ShaderGlobalType.Bool,
		ShaderGlobalType.Float,
		ShaderGlobalType.Vector2,
		ShaderGlobalType.Vector3,
		ShaderGlobalType.Vector4,
		ShaderGlobalType.Color,
		ShaderGlobalType.Texture
	};

	private ShaderGlobalType _currentType;
	public ShaderGlobalType CurrentType
	{
		get => _currentType;
		set
		{
			_currentType = value;

			OnGlobalTypeChosen?.Invoke( value );
		}
	}

	public Action<ShaderGlobalType> OnGlobalTypeChosen { get; set; }

	public ShaderGlobalTypeButton( ShaderGlobalType globalType ) : base( null )
	{
		SetStyles( $"padding-left: 32px; padding-right: 32px; font-family: '{Theme.DefaultFont}'; padding-top: 6px; padding-bottom: 6px;" );
		
		FixedWidth = 128;
		FixedHeight = Theme.RowHeight + 6;

		_currentType = globalType;

		UpdateButtonText();

		Clicked = Click;
	}


	private void UpdateButtonText()
	{
		Text = _currentType.ToString();
		//Icon = "";
	}

	private void Click()
	{
		var menu = new ContextMenu();

		foreach ( var globalType in _globalTypes )
		{
			var option = new Option();
			option.Text = globalType.ToString();
			//option.Icon = "";
			option.Triggered = () =>
			{
				_currentType = globalType;
				UpdateButtonText();
				OnGlobalTypeChosen?.Invoke( globalType );
			};

			menu.AddOption( option );
		}

		menu.OpenAt( ScreenRect.BottomLeft, false );
	}

	//[EditorEvent.Frame]
	//public void Frame()
	//{
	//	UpdateButtonText();
	//}

	protected override void OnPaint()
	{
		Paint.Antialiasing = true;
		Paint.ClearPen();
		Paint.SetBrush( Theme.ControlBackground );
		Paint.DrawRect( LocalRect, Theme.ControlRadius );

		var fg = Theme.Text;

		Paint.SetDefaultFont();
		Paint.SetPen( fg.WithAlphaMultiplied( Paint.HasMouseOver ? 1.0f : 0.9f ) );
		Paint.DrawIcon( LocalRect.Shrink( 8, 0, 0, 0 ), Icon, 14, TextFlag.LeftCenter );
		Paint.DrawText( LocalRect.Shrink( 32, 0, 0, 0 ), Text, TextFlag.LeftCenter );

		Paint.DrawIcon( LocalRect.Shrink( 4, 0 ), "arrow_drop_down", 18, TextFlag.RightCenter );
	}
}
