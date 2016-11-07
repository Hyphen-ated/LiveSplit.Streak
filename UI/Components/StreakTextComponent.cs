namespace LiveSplit.UI.Components
{
    public class StreakTextComponent : InfoTextComponent
    {
        public StreakComponentSettings Settings { get; set; }

        public StreakTextComponent(StreakComponentSettings settings)
            : base("", "")
        {
            Settings = settings;
        }

        public override void PrepareDraw(Model.LiveSplitState state, LayoutMode mode)
        {
            NameMeasureLabel.Font = Settings.OverrideFont1 ? Settings.Font1 : state.LayoutSettings.TextFont;
            ValueLabel.Font = Settings.OverrideFont2 ? Settings.Font2 : state.LayoutSettings.TextFont;
            NameLabel.Font = Settings.OverrideFont1 ? Settings.Font1 : state.LayoutSettings.TextFont;
        }
    }
}
