
namespace Sandbox;

[Expose]
public class ShaderGlobal
{
	public string Name { get; set; }

	public object DefaultValue { get; set; }

	public ShaderGlobal( string name, object defaultValue )
	{
		Name = name;
		DefaultValue = defaultValue;
	}

	public ShaderGlobal()
	{
	}
}
