using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class StreakComponent : IComponent
    {
        protected InfoTextComponent InternalComponent { get; set; }
        public StreakComponentSettings Settings { get; set; }
        protected LiveSplitState CurrentState { get; set; }

        protected TimeSpan PreviousGoalTime { get; set; }
        protected TimingMethod PreviousTimingMethod { get; set; }

        private String StreakText = "0";

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;
        
        public IDictionary<string, Action> ContextMenuControls => null;
        
        public StreakComponent(LiveSplitState state)
        {
            Settings = new StreakComponentSettings()
            {
                CurrentState = state
            };
            InternalComponent = new InfoTextComponent(Settings.Text1, "0");
            state.OnSplit += state_OnSplit;
            state.OnUndoSplit += state_OnUndoSplit;
            state.OnReset += state_OnReset;
            CurrentState = state;
            CurrentState.RunManuallyModified += CurrentState_RunModified;
            UpdateStreakValue(state);
        }
    
        void CurrentState_RunModified(object sender, EventArgs e)
        {
            UpdateStreakValue(CurrentState);
        }

        void state_OnReset(object sender, TimerPhase e)
        {
            UpdateStreakValue((LiveSplitState)sender);
        }

        void state_OnUndoSplit(object sender, EventArgs e)
        {
            UpdateStreakValue((LiveSplitState)sender);
        }

        void state_OnSplit(object sender, EventArgs e)
        {
            UpdateStreakValue((LiveSplitState)sender);
        }

        void UpdateStreakValue(LiveSplitState state)
        {
            var hist = state.Run.AttemptHistory;
            int currentStreak = -1;
            int streakCounter = 0;
            int bestStreak = 0;
            //go backwards through past runs to see what the current streak is, and also go find the best streak in the history
            for (int i = hist.Count - 1; i >= 0; --i)
            {
                TimeSpan? span;
                if (state.CurrentTimingMethod == TimingMethod.RealTime)
                    span = hist[i].Time.RealTime;
                else
                    span = hist[i].Time.GameTime;

                if (!span.HasValue || span.Value > Settings.GoalTime)
                {
                    //streak failed
                    //if span didn't have a value, that just means it was an incomplete run, which is a failure too.
                    if (currentStreak == -1)                        
                        currentStreak = streakCounter;
                    streakCounter = 0;
                } else
                {
                    ++streakCounter;
                    if (streakCounter > bestStreak)
                        bestStreak = streakCounter;
                }                                                    
            }
            if(Settings.ShowBest)
            {
                StreakText = currentStreak + " (best: " + bestStreak + ")";
            } else
            {
                StreakText = "" + currentStreak;
            }
            PreviousTimingMethod = state.CurrentTimingMethod;
            PreviousGoalTime = Settings.GoalTime;
        }

        private bool CheckIfRunChanged(LiveSplitState state)
        {
            if (PreviousTimingMethod != state.CurrentTimingMethod)
                return true;

            if (PreviousGoalTime != Settings.GoalTime)
                return true;

            return false;
        }

        private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.BackgroundColor.ToArgb() != Color.Transparent.ToArgb()
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.ToArgb() != Color.Transparent.ToArgb())
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawBackground(g, state, width, VerticalHeight);

            InternalComponent.InformationName = Settings.Text1;
            InternalComponent.InformationValue = StreakText;

            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;
            
            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawBackground(g, state, HorizontalWidth, height);

            InternalComponent.InformationName = Settings.Text1;
            InternalComponent.InformationValue = StreakText;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;
            
            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public float VerticalHeight => InternalComponent.VerticalHeight;

        public float MinimumWidth => InternalComponent.MinimumWidth; 

        public float HorizontalWidth => InternalComponent.HorizontalWidth; 

        public float MinimumHeight => InternalComponent.MinimumHeight;

        public string ComponentName => "Streak";

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (CheckIfRunChanged(state))
                UpdateStreakValue(state);

            InternalComponent.InformationValue = StreakText;

            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose()
        {
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
