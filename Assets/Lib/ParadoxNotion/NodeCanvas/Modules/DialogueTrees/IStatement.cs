using ParadoxNotion;
using NodeCanvas.Framework;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace NodeCanvas.DialogueTrees
{

    ///<summary>An interface to use for whats being said by a dialogue actor</summary>
    public interface IStatement
    {
        DialogueTree Parent { get; set; }
        string text { get; }
        AudioClip audio { get; }
        string meta { get; }
    }

    ///<summary>Holds data of what's being said usualy by an actor</summary>
    [System.Serializable]
    public class Statement : IStatement
    {
        [SerializeField]
        private string _text = string.Empty;
        [SerializeField]
        private AudioClip _audio;
        [SerializeField]
        private string _meta = string.Empty;

        public DialogueTree Parent { get; set; }

        public string text {
            get { return _text; }
            set { _text = value; }
        }

        public AudioClip audio {
            get { return _audio; }
            set { _audio = value; }
        }

        public string meta {
            get { return _meta; }
            set { _meta = value; }
        }

        //required
        public Statement() { }
        public Statement(string text) {
            this.text = text;
        }

        public Statement(string text, AudioClip audio) {
            this.text = text;
            this.audio = audio;
        }

        public Statement(string text, AudioClip audio, string meta) {
            this.text = text;
            this.audio = audio;
            this.meta = meta;
        }

        ///<summary>Replace the text of the statement found in brackets, with blackboard variables ToString and returns a Statement copy</summary>
        public IStatement BlackboardReplace(IBlackboard bb) {
            var copy = ParadoxNotion.Serialization.JSONSerializer.Clone<Statement>(this);

            copy.text = copy.text.ReplaceWithin('[', ']', (input) =>
            {
                object o = null;
                if ( bb != null ) { //referenced blackboard replace
                    var v = bb.GetVariable(input, typeof(object));
                    if ( v != null ) { o = v.value; }
                }

                if ( input.Contains("/") ) { //global blackboard replace
                    var globalBB = GlobalBlackboard.Find(input.Split('/').First());
                    if ( globalBB != null ) {
                        var v = globalBB.GetVariable(input.Split('/').Last(), typeof(object));
                        if ( v != null ) { o = v.value; }
                    }
                }
                return o != null ? o.ToString() : input;
            });

            return copy;
        }

        public override string ToString() {
            return text;
        }

		public override bool Equals(object obj)
		{
			return obj is Statement statement &&
				   EqualityComparer<DialogueTree>.Default.Equals(Parent, statement.Parent) &&
				   text == statement.text &&
				   EqualityComparer<AudioClip>.Default.Equals(audio, statement.audio) &&
				   meta == statement.meta;
		}

		public override int GetHashCode()
		{
			return System.HashCode.Combine(Parent, text, audio, meta);
		}

		public static bool operator ==(Statement left, Statement right)
		{
			return EqualityComparer<Statement>.Default.Equals(left, right);
		}

		public static bool operator !=(Statement left, Statement right)
		{
			return !(left == right);
		}
	}
}