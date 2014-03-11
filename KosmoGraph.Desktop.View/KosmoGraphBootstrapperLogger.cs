namespace KosmoGraph.Desktop
{
    using Microsoft.Practices.Prism.Logging;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Export(typeof(ILoggerFacade))]
    public class KosmoGraphBootstrapperLogger : ILoggerFacade
    {
        #region ILoggerFacade Members

        public void Log(string message, Category category, Priority priority)
        {
            return;
        }

        #endregion
    }
}
