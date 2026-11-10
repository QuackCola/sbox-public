using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Editor;

/// <summary>
/// A titlebar. Has the standard assortment of features - icon, menubar, title and window controls.
/// </summary>
public class PanelTitleBar : Panel
{
	private readonly Sandbox.UI.Label _titlebarIcon;
	private readonly Sandbox.UI.Label _titlebarTitle;
	private readonly Sandbox.UI.Label _maximizeIcon;
	private readonly Sandbox.UI.MenuBar _menubar;

	public string Icon
	{
		get => _titlebarIcon.Text;
		set => _titlebarIcon.Text = value;
	}

	public string Title
	{
		get => _titlebarTitle.Text;
		set => _titlebarTitle.Text = value;
	}

	public string MaximizeControlIcon
	{
		get => _maximizeIcon.Text;
		set => _maximizeIcon.Text = value;
	}

	/// <summary>
	/// When the minimize button is clicked.
	/// </summary>
	public Action OnMinimize { get; set; }

	/// <summary>
	/// When the maximize button is clicked.
	/// </summary>
	public Action OnMaximize { get; set; }

	/// <summary>
	/// When the close button is clicked.
	/// </summary>
	public Action OnClose { get; set; }

	/// <param name="onBuildMenuBar"></param>
	/// <param name="onBuildSpecial">
	/// Any additional items to the left of the window control buttons.
	/// </param>
	/// <param name="accentLine"></param>
	public PanelTitleBar( Action<Sandbox.UI.MenuBar> onBuildMenuBar = null, Action<Sandbox.UI.Panel> onBuildSpecial = null, bool accentLine = false ) : base()
	{
		AddClass( "titlebar window-drag" );
		_titlebarIcon = Add.Label( "s&", "brand" );

		// Menus live in the title bar, same as the editor's
		_menubar = new Sandbox.UI.MenuBar();
		_menubar.AddClass( "window-nodrag" );

		onBuildMenuBar?.Invoke( _menubar );

		AddChild( _menubar );

		Add.Panel( "grow" );

		// Centred over the bar, so the menus on the left don't push it off centre
		var titleWrap = Add.Panel( "titlewrap" );
		_titlebarTitle = titleWrap.Add.Label( "", "title" );

		onBuildSpecial?.Invoke( this );

		WindowButton( this, "remove", null, () => OnMinimize?.Invoke() );
		_maximizeIcon = WindowButton( this, "crop_square", null, () => OnMaximize?.Invoke() );
		WindowButton( this, "close", "close", () => OnClose?.Invoke() );

		if ( accentLine )
			Add.Panel( "accentline" );
	}

	protected virtual void BuildMenuBar( Sandbox.UI.MenuBar menus )
	{

	}

	protected Sandbox.UI.Label WindowButton( Panel parent, string icon, string classname, Action onClick )
	{
		var button = Clickable( parent, "windowbutton window-nodrag", onClick );
		if ( classname is not null ) button.AddClass( classname );
		return IconGlyph( button, icon );
	}

	/// <summary>
	/// Material icon glyph. The engine ships the font, so the text is the ligature name.
	/// </summary>
	static Sandbox.UI.Label IconGlyph( Panel parent, string icon, string classname = null )
	{
		var label = parent.Add.Label( icon, "icon" );
		if ( classname is not null ) label.AddClass( classname );
		return label;
	}

	/// <summary>
	/// A panel with a click handler and a class.
	/// </summary>
	static Panel Clickable( Panel parent, string classname, Action onClick )
	{
		var panel = parent.Add.Panel( classname );
		panel.AddEventListener( "onclick", onClick );
		return panel;
	}

	/// <summary>
	/// Square icon button, like the ones in a toolbar.
	/// </summary>
	static Panel IconButton( Panel parent, string icon, Action onClick )
	{
		var button = Clickable( parent, "iconbutton", onClick );
		IconGlyph( button, icon );
		return button;
	}
}
