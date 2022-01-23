using GameJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public class PhoneAppJobsEntry : ExtendedMonoBehaviour
	{
		public Image JobIcon;
		public Text JobDescription, JobDistance, JobReward;
		public Button Button => GetComponent<Button>();
		public bool Expanded { get; private set; }

		public GameJob Job { get; private set; }

		public void SetData(GameJob job)
		{
			Job = job;
			Invalidate();
		}

		public void Invalidate()
		{
			if(Job == null)
			{
				gameObject.SetActive(false);
				return;
			}
			JobIcon.sprite = Job.Icon;
			JobDescription.text = $"<b>{Job.Name}</b>\n{Job.Description}";
			JobReward.text = Job.Reward;
		}

		public void ToggleExpanded()
		{
			Expanded = !Expanded;
		}

		private void Update()
		{
			if (Job == null)
			{
				gameObject.SetActive(false);
				return;
			}
			Button.enabled = !Expanded;
			JobDistance.text = $"{Mathf.FloorToInt((Job.GetCurrentPosition() - transform.position).magnitude)}m";
		}
	}
}