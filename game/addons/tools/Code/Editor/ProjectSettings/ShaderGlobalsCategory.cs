
namespace Editor.ProjectSettingPages;

[Title( "Shader Globals" )]
internal sealed class ShaderGlobalsCategory : ProjectInspector.Category
{
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
		
		ShaderGlobalsList = new( null );
		
		BodyLayout.Add( ShaderGlobalsList );
	}

	private void AddGlobal()
	{
		ShaderGlobalsList.AddItem( CurrentGlobalType );
	}

	public override void OnSave()
	{
		EditorUtility.SaveProjectSettings( Settings, "ShaderGlobals.config" );

		base.OnSave();
	}
}

internal sealed class ShaderGlobalsList : ListView
{
	public ShaderGlobalsList( Widget parent = null ) : base( parent )
	{
		Margin = 6;
		ItemSpacing = 4;
		ItemSize = new Vector2( 0, 24 );
		AcceptDrops = false;


	}

	public void AddItem( ShaderGlobalsCategory.ShaderGlobalType globalType )
	{
		if ( globalType == ShaderGlobalsCategory.ShaderGlobalType.Bool )
		{
			AddItem( new ShaderGlobal( "MyGlobalBool", false  ) );
		}
	}

	protected override void PaintItem( VirtualWidget item )
	{
		var shaderGlobal = item.Object;
		var rect = item.Rect;
		var textColor = Theme.TextControl;
		var itemColor = Theme.ControlBackground;
		var typeColor = Color.White;

		if ( item.Hovered )
		{
			textColor = Color.White;
			itemColor = Theme.Primary.Lighten( 0.1f ).Desaturate( 0.3f ).WithAlpha( 0.4f * 0.6f );
		}
		if ( item.Selected )
		{
			textColor = Theme.TextControl;
			itemColor = Theme.Primary;
		}

		Paint.ClearPen();
		Paint.SetBrush( itemColor );
		Paint.DrawRect( rect, Theme.ControlRadius );

		var iconRect = rect.Shrink( 4, 0, 0, 0 );
		Paint.SetPen( typeColor );
		Paint.DrawIcon( iconRect, "circle", 12f, TextFlag.LeftCenter );
		rect.Left += 24f;

		Paint.SetPen( textColor.WithAlpha( 0.7f ) );
		Paint.SetBrush( textColor.WithAlpha( 0.7f ) );

		Paint.DrawText( rect.Shrink( 4, 0, 0, 0 ), $"Some Global", TextFlag.LeftCenter );
		Paint.DrawText( rect.Shrink( 0, 0, 4, 0 ), $"{DisplayInfo.ForType( shaderGlobal.GetType() ).Name}", TextFlag.RightCenter );
	}

	protected override void OnPaint()
	{
		Paint.ClearPen();
		Paint.SetBrush( Theme.ControlBackground );
		Paint.DrawRect( LocalRect, 4 );

		base.OnPaint();
	}
}

internal sealed class ShaderGlobalTypeButton : Button
{
	private List<ShaderGlobalsCategory.ShaderGlobalType> _globalTypes = new List<ShaderGlobalsCategory.ShaderGlobalType>()
	{
		ShaderGlobalsCategory.ShaderGlobalType.Int,
		ShaderGlobalsCategory.ShaderGlobalType.Bool,
		ShaderGlobalsCategory.ShaderGlobalType.Float,
		ShaderGlobalsCategory.ShaderGlobalType.Vector2,
		ShaderGlobalsCategory.ShaderGlobalType.Vector3,
		ShaderGlobalsCategory.ShaderGlobalType.Vector4,
		ShaderGlobalsCategory.ShaderGlobalType.Color,
		ShaderGlobalsCategory.ShaderGlobalType.Texture
	};

	private ShaderGlobalsCategory.ShaderGlobalType _currentType;
	public ShaderGlobalsCategory.ShaderGlobalType CurrentType
	{
		get => _currentType;
		set
		{
			_currentType = value;

			OnGlobalTypeChosen?.Invoke( value );
		}
	}

	public Action<ShaderGlobalsCategory.ShaderGlobalType> OnGlobalTypeChosen { get; set; }

	public ShaderGlobalTypeButton( ShaderGlobalsCategory.ShaderGlobalType globalType ) : base( null )
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
