using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Editor;

public partial class PanelDockWindow : Panel
{
	public DockHost DockHost { get; private set; }

	public string WindowTitle
	{
		get;
		set
		{ 
			field = value;

			_windowTitleLabel?.Text = value;
		}
	}

	private readonly PanelWindow _window;

	private PanelDockWindows _dockWindows;
	//private string _defaultLayout;
	//private string _savedLayout;

	private Sandbox.UI.Label _titleBarLabel;
	private Sandbox.UI.Label _windowTitleLabel;
	private Sandbox.UI.Label _maximizeIcon;

	protected Sandbox.UI.StatusBar StatusBar { get; private set; }

	private readonly List<RootPanel> _floatingRoots = new();

	public PanelDockWindow( PanelWindow window ) : base()
	{
		_window = window;

		BuildTitleBar();
		BuildBodyInternal();
		BuildStatusBarInternal();

		ApplyTheme();
	}

	private void BuildTitleBar()
	{
		var bar = Add.Panel( "titlebar window-drag" );
		_titleBarLabel = bar.Add.Label( "s&", "brand" );

		// Menus live in the title bar, same as the editor's
		var menus = bar.AddChild( new Sandbox.UI.MenuBar() );
		menus.AddClass( "window-nodrag" );

		BuildMenuBar( menus );

		bar.Add.Panel( "grow" );

		// Centred over the bar, so the menus on the left don't push it off centre
		var titleWrap = bar.Add.Panel( "titlewrap" );
		_windowTitleLabel = titleWrap.Add.Label( "", "title" );

		BuildTitleBarSpecial( bar );

		_themeIcon = WindowButton( bar, "light_mode", "theme", ToggleTheme );

		WindowButton( bar, "remove", null, _window.Minimize );
		_maximizeIcon = WindowButton( bar, "crop_square", null, () =>
		{
			_window.ToggleMaximized();
			UpdateMaximizeIcon();
		} );
		WindowButton( bar, "close", "close", _window.Dispose );

		Add.Panel( "accentline" );
	}

	private void BuildBodyInternal()
	{
		DockHost = new DockHost();
		DockHost.AddClass( "editor-dock-workspace" );

		_dockWindows = new PanelDockWindows( DockHost ) { ConfigureWindow = ConfigureFloatingWindow };

		BuildBody();

		var body = Add.Panel( "body" );
		body.AddChild( DockHost );
	}

	private void BuildStatusBarInternal()
	{
		StatusBar = AddChild( new Sandbox.UI.StatusBar() );

		BuildStatusBar();
	}

	protected virtual void BuildMenuBar( Sandbox.UI.MenuBar menuBar )
	{
	}

	/// <summary>
	/// Anything to the left of the window minimize, maximize and close buttons.
	/// </summary>
	/// <param name="bar"></param>
	protected virtual void BuildTitleBarSpecial( Panel bar )
	{
	}

	protected virtual void BuildBody()
	{
	}

	protected virtual void BuildStatusBar()
	{
	}

	private void ConfigureFloatingWindow( RootPanel root )
	{
		root.AddClass( "mock-editor-surface" );
		root.AddClass( "editor-window" );
		root.StyleSheet.Load( "/styles/gallery.scss" );

		_floatingRoots.Add( root );
		ApplyFloatingTheme( root );

		// A floated tool no longer bubbles keys through EditorWindow.
		var host = root.Children.OfType<DockHost>().Single();
		var shortcuts = root.AddChild( new FloatingShortcuts( this ) );
		host.Parent = shortcuts;
	}

	protected void ShowDock( string id )
	{
		var host = DockHost.Find( id ) is not null ? DockHost : _floatingRoots
			.Where( x => x.IsValid )
			.SelectMany( x => x.Descendants.OfType<DockHost>() )
			.FirstOrDefault( x => x.Find( id ) is not null );

		if ( host is null )
			return;

		if ( !host.IsOpen( id ) )
			host.Dock( id );

		host.Activate( id );
		PanelWindow.FromPanel( host )?.Focus();
	}

	public void ResetLayout()
	{

	}

	public void SaveLayout()
	{

	}

	private void UpdateMaximizeIcon()
	{
		_maximizeIcon.Text = _window.IsMaximized ? "filter_none" : "crop_square";
	}

	public void SetWindowIcon( string text )
	{
		_titleBarLabel.Text = text;
	}

	protected virtual bool HandleShortcut( ButtonEvent ev, Panel scope )
	{
		return false;
	}

	public override void OnButtonEvent( ButtonEvent ev )
	{
		if ( ev.Pressed && HandleShortcut( ev, this ) )
		{
			ev.StopPropagation = true;
			return;
		}

		base.OnButtonEvent( ev );
	}

	public override void OnDeleted()
	{
		_dockWindows?.Dispose();
		_floatingRoots.Clear();
		base.OnDeleted();
	}

	// TODO
	protected Sandbox.UI.Label WindowButton( Panel parent, string icon, string classname, Action onClick )
	{
		//var button = parent.Clickable( "windowbutton window-nodrag", onClick );
		//if ( classname is not null ) button.AddClass( classname );
		//return button.Icon( icon );
		return null;
	}

	sealed class FloatingShortcuts : Panel
	{
		private readonly PanelDockWindow _dockWindow;

		internal FloatingShortcuts( PanelDockWindow dockWindow )
		{
			_dockWindow = dockWindow;
			Style.Width = Length.Percent( 100 );
			Style.Height = Length.Percent( 100 );
		}

		public override void OnButtonEvent( ButtonEvent ev )
		{
			if ( ev.Pressed && _dockWindow.IsValid && _dockWindow.HandleShortcut( ev, this ) )
			{
				ev.StopPropagation = true;
				return;
			}
			base.OnButtonEvent( ev );
		}
	}
}
