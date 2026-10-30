using Sandbox.UI;
using System;

namespace Editor;

public partial class PanelDockWindow
{
	/// <summary>
	/// The class that turns the light theme on. The light rules in gallery.scss are scoped under it.
	/// </summary>
	public const string LightModeClass = "editor-light-mode";

	private bool _lightMode;

	private Sandbox.UI.Label _themeIcon;

	private void ApplyFloatingTheme( RootPanel root )
	{
		root.SetClass( LightModeClass, _lightMode );
		root.SetClass( "style-light", _lightMode );
	}

	private void ApplyTheme()
	{
		SetClass( LightModeClass, _lightMode );
		SetClass( "style-light", _lightMode );
		_floatingRoots.RemoveAll( x => !x.IsValid );
		foreach ( var root in _floatingRoots ) ApplyFloatingTheme( root );

		_themeIcon.Text = _lightMode ? "dark_mode" : "light_mode";
		_window.BackgroundColor = _lightMode ? Color.FromBytes( 238, 240, 246 ) : Color.FromBytes( 15, 17, 23 );
	}

	/// <summary>
	/// Swap the palette. Both themes are in the sheet already - the light rules are scoped under
	/// one class on this panel, so flipping it just changes which rules match and every transition
	/// in the window animates the change.
	/// </summary>
	private void ToggleTheme()
	{
		_lightMode = !_lightMode;
		ApplyTheme();
	}
}
