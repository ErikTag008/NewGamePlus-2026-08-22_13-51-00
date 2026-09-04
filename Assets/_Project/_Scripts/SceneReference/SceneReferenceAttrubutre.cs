using UnityEngine;

namespace Assets._Project._Scripts.SceneReference
{
    /// <summary>
	/// Uses a CustomPropertyDrawer to draw a string field as a SceneAsset object field.
	/// </summary>
	/// <example><code>
	/// [SceneRef] string mainMenu;
	/// </code></example>
	[System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class SceneReferenceAttribute : PropertyAttribute { }
}
