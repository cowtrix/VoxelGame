using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Voxul.Utilities;

namespace Jerbs.UI
{
    public class JobTooltipUI : Singleton<JobTooltipUI>
    {
        public bool IsToggled { get; private set; }
        public Jerb CurrentJerb { get; private set; }

        public CanvasGroup CanvasGroup;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Instructions;
        public TextMeshProUGUI ToggleTip;
        public LayoutElement InstructionScrollView;

        public float TargetHeight = 350;
        public float ExpandSpeed = 1;
        public float FadeSpeed = 1;
         
        public void SetData(Jerb job)
        {
            CurrentJerb = job;
            if (CurrentJerb)
            {
                Title.text = $"Active Job: {job.JobInfo.Name}";
                Instructions.text = job.JobInfo.Instructions;
            }
        }

        public void OnToggleTooltip(InputAction.CallbackContext cntxt)
        {
            if (cntxt.started)
            {
                IsToggled = !IsToggled;
            }
        }

        private void Update()
        {
            CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, CurrentJerb ? 1 : 0, FadeSpeed * Time.deltaTime);
            InstructionScrollView.minHeight = Mathf.MoveTowards(InstructionScrollView.minHeight, IsToggled ? TargetHeight : 0, ExpandSpeed * Time.deltaTime);
        }
    }
}
