namespace Editor.ShaderGraph.Nodes;

/*
/// <summary>
/// If True, do this, if False, do that.
/// Give it a name to use a bool attribute.
/// Use no name to use condition from A and B inputs.
/// </summary>
[Title( "Branch" ), Category( "Logic" ), Icon( "alt_route" )]
public sealed class Branch : ShaderNode
{
	[Hide]
	public override string Title => UseCondition ?
		$"{DisplayInfo.For( this ).Name} (A {Op} B)" :
		$"{DisplayInfo.For( this ).Name} ({Name})";

	[Hide]
	private bool UseCondition => string.IsNullOrWhiteSpace( Name );

	[Input, Hide]
	public NodeInput True { get; set; }

	[Input, Hide]
	public NodeInput False { get; set; }

	[Input, Hide]
	public NodeInput A { get; set; }

	[Input, Hide]
	public NodeInput B { get; set; }

	public string Name { get; set; } = "";

	public bool IsAttribute { get; set; } = true;

	public enum OperatorType
	{
		Equal,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual
	}

	[HideIf( nameof( UseCondition ), false )]
	public OperatorType Operator { get; set; }

	[HideIf( nameof( UseCondition ), true )]
	public bool Enabled { get; set; }

	[InlineEditor]
	public ParameterUI UI { get; set; }

	[Hide]
	private string Op
	{
		get
		{
			return Operator switch
			{
				OperatorType.Equal => "==",
				OperatorType.NotEqual => "!=",
				OperatorType.GreaterThan => ">",
				OperatorType.LessThan => "<",
				OperatorType.GreaterThanOrEqual => ">=",
				OperatorType.LessThanOrEqual => "<=",
				_ => throw new NotImplementedException(),
			};
		}
	}

	[Output]
	[Hide]
	public NodeResult.Func Result => ( GraphCompiler compiler ) =>
	{
		var useCondition = UseCondition;
		var results = compiler.Result( True, False, 0.0f, 0.0f );
		var resultA = useCondition ? compiler.ResultOrDefault( A, 0.0f ) : default;
		var resultB = useCondition ? compiler.ResultOrDefault( B, 0.0f ) : default;

		return new NodeResult( results.Item1.Components, $"{(useCondition ?
			$"{resultA.Cast( 1 )} {Op} {resultB.Cast( 1 )}" : compiler.ResultParameter( Name, Enabled, default, default, false, IsAttribute, UI ))} ?" +
			$" {results.Item1} :" +
			$" {results.Item2}" );
	};
}
*/

/// <summary>
/// If True, do this, if False, do that.
/// </summary>
[Title( "Branch" ), Category( "Logic" ), Icon( "alt_route" )]
public sealed class Branch : ShaderNode
{
	[Hide]
	public override string Title => string.IsNullOrWhiteSpace( Name ) ?
	$"{DisplayInfo.For( this ).Name}" :
	$"{DisplayInfo.For( this ).Name} ({Name})";

	[Input, Hide]
	public NodeInput True { get; set; }

	[Input, Hide]
	public NodeInput False { get; set; }

	public string Name { get; set; } = "";

	public bool IsAttribute { get; set; } = true;

	public bool Enabled { get; set; }

	[InlineEditor]
	public ParameterUI UI { get; set; }

	[Output]
	[Hide]
	public NodeResult.Func Result => ( GraphCompiler compiler ) =>
	{
		var resultPredicate = compiler.ResultParameter( Name, Enabled, default, default, false, IsAttribute, UI );
		var results = compiler.Result( True, False, 0.0f, 0.0f );

		return new NodeResult( results.Item1.Components, $"{resultPredicate} ? {results.Item1} : {results.Item2}" );
	};
}

/// <summary>
/// Compare Input 'A' with Input 'B' and output the input from either 'True' or 'False' based on the result of the comparison.
/// </summary>
[Title( "Comparison" ), Category( "Logic" ), Icon( "compare" )]
public sealed class Comparison : ShaderNode
{
	[Hide]
	public override string Title => $"{DisplayInfo.For( this ).Name} (A {Op} B)";

	[Input, Hide]
	public NodeInput True { get; set; }

	[Input, Hide]
	public NodeInput False { get; set; }

	[Input, Hide]
	public NodeInput A { get; set; }

	[Input, Hide]
	public NodeInput B { get; set; }

	public enum OperatorType
	{
		Equal,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual
	}

	public OperatorType Operator { get; set; }

	[Hide]
	private string Op
	{
		get
		{
			return Operator switch
			{
				OperatorType.Equal => "==",
				OperatorType.NotEqual => "!=",
				OperatorType.GreaterThan => ">",
				OperatorType.LessThan => "<",
				OperatorType.GreaterThanOrEqual => ">=",
				OperatorType.LessThanOrEqual => "<=",
				_ => throw new NotImplementedException(),
			};
		}
	}

	[Output, Title( "Result" )]
	[Description( "Result from either the 'True' or 'False' inputs depending on the result of the comparison with Input 'A' and Input 'B'." )]
	[Hide]
	public NodeResult.Func Result => ( GraphCompiler compiler ) =>
	{
		var results = compiler.Result( True, False, 0.0f, 0.0f );
		var resultA = compiler.ResultOrDefault( A, 0.0f );
		var resultB = compiler.ResultOrDefault( B, 0.0f );

		return new NodeResult( results.Item1.Components, $"{resultA.Cast( 1 )} {Op} {resultB.Cast( 1 )}" );
	};
}
