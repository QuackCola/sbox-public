
namespace Editor.ProjectSettingPages;

[Title( "Shader Globals" )]
internal sealed class ShaderGlobalsCategory : ProjectInspector.Category
{
	Button.Danger DeleteButton;
	LineEdit LineEdit;
	ShaderGlobalsList ShaderGlobalsList;

	ShaderGlobalsSettings Settings;

	public override void OnInit( Project project )
	{
		base.OnInit( project );

		Settings = EditorUtility.LoadProjectSettings<ShaderGlobalsSettings>( "ShaderGlobals.config" );

		var header = BodyLayout.AddRow( 0, false );
		BodyLayout.Spacing = 8;
		BodyLayout.Spacing = 4;

		BodyLayout.Add( header );

		LineEdit = new LineEdit() { PlaceholderText = "Add New Shader Global..." };

		header.Add( LineEdit );

		DeleteButton = new Button.Danger( "Delete", "delete" );
		DeleteButton.Enabled = false;
		DeleteButton.ToolTip = $"Delete selected shader global";
		DeleteButton.Clicked += () =>
		{
			// TODO
		};

		header.Add( DeleteButton );

		var addButton = new Button.Primary( "Add", "new_label" );
		addButton.Enabled = true;
		addButton.ToolTip = $"Add new shader global";
		addButton.Clicked += () =>
		{
			// TODO
		};

		header.Add( addButton );
		
		ShaderGlobalsList = new( null );

		BodyLayout.Add( ShaderGlobalsList );
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

	protected override void PaintItem( VirtualWidget item )
	{
		var variable = item.Object;
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

		var textRect = Paint.DrawText( rect.Shrink( 4, 0, 0, 0 ), $"Some Variable", TextFlag.LeftCenter );
		var typeRect = Paint.DrawText( rect.Shrink( 0, 0, 4, 0 ), $"{DisplayInfo.ForType( variable.GetType() ).Name}", TextFlag.RightCenter );

		//Paint.SetPen( Color.Gray.WithAlpha( 0.25f ) );
		//Paint.SetBrush( Color.Gray.WithAlpha( 0.25f ) );
		//Paint.DrawRect( typeRect.Grow( 2 ), Theme.ControlRadius );
	}


	protected override void OnPaint()
	{
		Paint.ClearPen();
		Paint.SetBrush( Theme.ControlBackground );
		Paint.DrawRect( LocalRect, 4 );

		base.OnPaint();
	}
}
