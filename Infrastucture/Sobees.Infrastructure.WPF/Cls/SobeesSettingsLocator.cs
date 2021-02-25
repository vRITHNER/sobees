#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Sobees.Infrastructure.Cls
{
    public class SobeesSettingsLocator
    {
        protected static SobeesSettings _sobeesSettings;

        public static SobeesSettings SobeesSettingsStatic
        {
            get { return _sobeesSettings ?? (_sobeesSettings = new SobeesSettings()); }
            set { _sobeesSettings = value; }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SobeesSettings SobeesSettings => SobeesSettingsStatic;

      public static void SetSettings(SobeesSettings settings)
        {
            _sobeesSettings = settings;
        }
    }
}