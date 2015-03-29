using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RecipesSearch.Data.Models;

namespace RecipesSearch.ImporterService
{    
    [ServiceContract]
    public interface IImporterService
    {
        [OperationContract]
        void ImportAllSites();

        [OperationContract]
        void ImportSites(IEnumerable<SiteToCrawl> sitesToCrawl);

        [OperationContract]
        bool RemoveFromQueue(int siteId);

        [OperationContract]
        void StopImporting();

        [OperationContract]
        void StopCurrentSiteImporting();

        [OperationContract]
        bool IsImportingInProgress();

        [OperationContract]
        IEnumerable<SiteToCrawl> SitesQueue();

        [OperationContract]
        int CrawledPages();
    }
}
