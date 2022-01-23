using Interaction.Items;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public class InventoryAppItem : ExtendedMonoBehaviour
	{
		public RawImage Image;
		public Text Count;
		public Toggle Toggle;

		public void SetData(Item item, int count)
		{
			Image.texture = item.Icon.Texture;
			Count.text = $"x{count}";
		}
	}
}