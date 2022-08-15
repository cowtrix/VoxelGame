using UnityEngine;

namespace Interaction.Activities
{
    [CreateAssetMenu(menuName = "Custom/Book Data")]
    public class BookData : ScriptableObject
    {
        public Color Color;
        public string Title;
        public string Author;
        [TextArea(32, 128)]
        public string Text;
    }
}