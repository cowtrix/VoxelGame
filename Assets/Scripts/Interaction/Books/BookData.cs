using UnityEngine;

namespace Interaction.Activities
{
    [CreateAssetMenu(menuName = "Custom/Book Data")]
    public class BookData : ScriptableObject
    {
        public string Title;
        [TextArea(32, 128)]
        public string Text;
    }
}