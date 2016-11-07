using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class StreakComponentFactory : IComponentFactory
    {
        public string ComponentName => "Streak";

        public string Description => "Tracks how many consecutive runs you get below a goal time.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new StreakComponent(state);

        public string UpdateName => ComponentName;

        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Streak.xml";

        public string UpdateURL => "http://livesplit.org/update/";

        public Version Version => Version.Parse("0.0.1");
    }
}
