using SpaceClaim.Api.V23.Extensibility;
using CreateBody.Properties;

namespace CreateBody
{
    public class CreateBody : AddIn, IExtensibility, ICommandExtensibility, IRibbonExtensibility
    {
        readonly CommandCapsule[] capsules = new[] {
            new CreateBlockCapsule(),
        };

        public bool Connect()
        {
            return true;
        }

        public void Disconnect()
        {
            
        }

        public void Initialize()
        {
            foreach (CommandCapsule capsule in capsules)
            {
                capsule.Initialize();
            }
        }

        public string GetCustomUI()
        {
            return Resources.Ribbon;
        }
    }
}
    