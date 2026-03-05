using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Editor.ShaderGraph;

// I would put the upgrader stuff in the BaseEditorLibrary but then that means i would need to move the nodes that im 
// upgrading to in the BaseEditorLibrary. Need to check with the facepunch devs on that - QuackCola

public sealed partial class Upgraders
{
	/*
	private BaseNode UpgradeBranchNode_v2Upgrade( JsonElement element, JsonSerializerOptions options )
	{
		element.TryGetProperty( "Name", out var nameElement );
		element.TryGetProperty( "Enabled", out var enabledElement );

		if ( string.IsNullOrWhiteSpace( nameElement.GetString() ) )
		{
			var comparisonNode = new Comparison();

			// Copy basic node properties
			DeserializeObject( comparisonNode, element, options );

			element.TryGetProperty( "Operator", out var operatorElement );
			comparisonNode.Operator = operatorElement.Deserialize<Comparison.OperatorType>( options );

			return comparisonNode;
		}
		else
		{
			var branchNode = EditorTypeLibrary.Create<Branch>( "Branch" );

			// Copy basic node properties
			DeserializeObject( branchNode, element, options );
			branchNode.Graph = this;

			BaseNode parameterNode;

			if ( !IsSubgraph )
			{
				var boolParameter = new BoolParameter()
				{
					Name = nameElement.GetString(),
					Value = enabledElement.GetBoolean()
				};

				AddParameter( boolParameter );

				parameterNode = new BoolParameterNode()
				{
					Position = branchNode.Position.WithX( branchNode.Position.x - 192 ),
					ParameterIdentifier = boolParameter.Identifier,
				};
			}
			else
			{
				var boolParameter = new BoolSubgraphInputParameter()
				{
					Name = nameElement.GetString(),
					Value = enabledElement.GetBoolean()
				};

				AddParameter( boolParameter );

				parameterNode = new SubgraphInput()
				{
					Position = branchNode.Position.WithX( branchNode.Position.x - 192 ),
					ParameterIdentifier = boolParameter.Identifier,
				};
			}

			AddNode( parameterNode );

			branchNode.ConnectNode(
				nameof( Branch.Predicate ),
				nameof( SubgraphInput.Result ),
				parameterNode.Identifier
			);

			return branchNode;
		}
		
	}
	*/

	/// <summary>
	/// Upgrade branch nodes depending on if they are named or not. If named just upgrade the old branch node json to the new version
	/// of the branch node. If not named then upgrade the old branch node json to the comparison node.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="options"></param>
	[JsonUpgrader( typeof( ShaderGraph ), 2 )]
	internal static void Upgrader_v2( JsonObject obj, JsonSerializerOptions options )
	{
		if ( obj["_nodes"] is not JsonArray oldNodeArray )
			return;

		var identifiers = new Dictionary<string, string>();
		foreach ( var node in oldNodeArray )
		{
			if ( node[nameof( BaseNode.Identifier )] is not JsonValue identifierValue )
				continue;

			identifiers.Add( identifierValue.GetValue<string>(), $"{identifiers.Count}" );
		}

		var newNodeArray = new JsonArray();

		//obj.TryGetProperty( "Name", out var nameElement );
		//obj.TryGetProperty( "Enabled", out var enabledElement );

		foreach ( var jsonNode in oldNodeArray )
		{
			if ( jsonNode["_class"] is not JsonValue classValue )
				continue;

			var nodeElement = JsonSerializer.Deserialize<JsonElement>( jsonNode.AsObject().ToJsonString() );
			var typeName = classValue.GetValue<string>();

			if ( typeName == "Branch" )
			{
				nodeElement.TryGetProperty( "Name", out var nameElement );
				nodeElement.TryGetProperty( "Enabled", out var enabledElement );

				var branchName = nameElement.GetString();
				var branchEnabled = enabledElement.GetBoolean();

				BaseNode newNode = null;

				if ( string.IsNullOrWhiteSpace( branchName ) )
				{
					var comparisonNode = EditorTypeLibrary.Create<Comparison>( "Comparison" );

					// Copy basic node properties
					//DeserializeObject( comparisonNode, element, options );

					nodeElement.TryGetProperty( "Operator", out var operatorElement );
					comparisonNode.Operator = operatorElement.Deserialize<Comparison.OperatorType>( options );

					newNode = comparisonNode;
				}
				else
				{
					var branchNode = EditorTypeLibrary.Create<Branch>( "Branch" );

					// Copy basic node properties
					//DeserializeObject( branchNode, element, options );
					//branchNode.Graph = this;


					newNode = branchNode;
				}

				var newNodeObject = new JsonObject { { "_class", newNode.GetType().Name } };

				//SerializeObject( newNode, newNodeObject, options, identifiers );

				newNodeArray.Add( newNodeObject );
			}
			else
			{
				newNodeArray.Add( jsonNode.DeepClone() );
			}
		}

		obj.Remove( "nodes" );
		obj.Add( "nodes", newNodeArray );
	}
}
