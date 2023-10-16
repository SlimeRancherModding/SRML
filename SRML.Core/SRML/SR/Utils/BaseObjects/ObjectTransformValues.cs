using UnityEngine;

namespace SRML.SR.Utils.BaseObjects
{
	public struct ObjectTransformValues
	{
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		public ObjectTransformValues(Vector3 position, Vector3 rotation, Vector3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public override string ToString()
		{
			return $"{position.ToString()} | {rotation.ToString()} | {scale.ToString()}";
			//return $"new ObjectTransformValues(new Vector3({position.x.ToString("N2")}f, {position.y.ToString("N2")}f, {position.z.ToString("N2")}f), new Vector3({rotation.x.ToString("N2")}f, {rotation.y.ToString("N2")}f, {rotation.z.ToString("N2")}f), new Vector3({scale.x}f, {scale.y}f, {scale.z}f)),";
		}
	}
}
