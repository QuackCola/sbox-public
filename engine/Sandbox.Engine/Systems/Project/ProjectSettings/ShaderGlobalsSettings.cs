namespace Sandbox;

[Expose]
public class ShaderGlobalsSettings : ConfigData
{
	public ShaderGlobalsSettings()
	{
		ShaderGlobals = new List<ShaderGlobal>();
	}

	/// <summary>
	/// A list of shader globals used by the game.
	/// </summary>
	public List<ShaderGlobal> ShaderGlobals { get; set; }
}
