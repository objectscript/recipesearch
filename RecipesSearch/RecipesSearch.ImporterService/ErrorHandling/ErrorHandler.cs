using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;

namespace RecipesSearch.ImporterService.ErrorHandling
{
    public class ImporterErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            LoggerWrapper.LogError("Importer service exception", error);
            return true;
        }
    }
}
