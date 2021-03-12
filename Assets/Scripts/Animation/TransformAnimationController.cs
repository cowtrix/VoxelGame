using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransformAnimationController : MonoBehaviour
{
	public float LerpThreshold = .001f;
	public float TransitionSpeed = 1;

	[HideInInspector]
	public string CurrentExpression;
	public Transform[] RecordedObjects;
	[HideInInspector]
	public List<Expression> Expressions = new List<Expression>();

	public Queue<string> ExpressionQueue = new Queue<string>();

	[Serializable]
	public class Expression
	{
		public string Name;

		[Serializable]
		public struct TransformProperties
		{
			public Transform Transform;
			public Vector3 Position;
			public Vector3 Size;
			public Quaternion Rotation;

			public TransformProperties(Transform tr)
			{
				Position = tr.localPosition;
				Size = tr.localScale;
				Rotation = tr.localRotation;
				Transform = tr;
			}
		}
		
		public TransformProperties[] State;
	}
	
	public void RecordCurrent(string name)
	{
		if(string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException(name);
		}
		Expression exp;
		if (Expressions.Any(x => x.Name == name))
		{
			exp = Expressions.Single(x => x.Name == name);
		}
		else
		{
			exp = new Expression();
			exp.Name = name;
			Expressions.Add(exp);
		}
		exp.State = new Expression.TransformProperties[RecordedObjects.Length];
		for (int i = 0; i < RecordedObjects.Length; i++)
		{
			Transform obj = RecordedObjects[i];
			exp.State[i] = new Expression.TransformProperties(obj);
		}
	}

	private void Update()
	{
		if(string.IsNullOrEmpty(CurrentExpression) && ExpressionQueue.Any())
		{
			CurrentExpression = ExpressionQueue.Dequeue();
		}
		var currentExp = Expressions.SingleOrDefault(e => e.Name == CurrentExpression);
		if (currentExp != null)
		{
			if(SetExpression(currentExp, TransitionSpeed * Time.deltaTime))
			{
				CurrentExpression = null;
			}
		}
	}

	public bool SetExpression(Expression currentExp, float lerp)
	{
		var lerpThreshSq = LerpThreshold * LerpThreshold;
		bool done = true;
		foreach(var obj in currentExp.State)
		{
			obj.Transform.localPosition = Vector3.Lerp(obj.Transform.localPosition, obj.Position, lerp);
			obj.Transform.localScale = Vector3.Lerp(obj.Transform.localScale, obj.Size, lerp);
			obj.Transform.localRotation = Quaternion.Lerp(obj.Transform.localRotation, obj.Rotation, lerp);

			if((obj.Transform.localPosition - obj.Position).sqrMagnitude > lerpThreshSq)
			{
				done = false;
			}
			if ((obj.Transform.localScale - obj.Size).sqrMagnitude > lerpThreshSq)
			{
				done = false;
			}
			if(Quaternion.Angle(obj.Transform.localRotation, obj.Rotation) > LerpThreshold)
			{
				done = false;
			}
		}
		return done;
	}
}
