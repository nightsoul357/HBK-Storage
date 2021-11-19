using HBK.Storage.Dashboard.Shared;

namespace HBK.Storage.Dashboard.Services
{
    public class HBKLayoutComponentService
    {
        private List<HBKLayoutComponentBase> HBKLayoutComponents { get; set; } = new List<HBKLayoutComponentBase>();

        public void RegisterComponent(HBKLayoutComponentBase component)
        {
            if (!this.HBKLayoutComponents.Contains(component))
            {
                this.HBKLayoutComponents.Add(component);
            }
        }

        public void UnregisterComponent(HBKLayoutComponentBase component)
        {
            if (!this.HBKLayoutComponents.Contains(component))
            {
                this.HBKLayoutComponents.Remove(component);
            }
        }

        public void RefreshAllComponent()
        {
            foreach (var component in this.HBKLayoutComponents)
            {
                try
                {
                    component.Refresh();
                }
                catch { }
            }
        }
    }
}
