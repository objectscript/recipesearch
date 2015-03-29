using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RecipesSearch.ImporterService.ErrorHandling;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Importer;

namespace RecipesSearch.ImporterService
{
    [ErrorBehavior(typeof(ImporterErrorHandler))]
    public class ImporterService : IImporterService
    {
        private readonly Importer _importer = Importer.GetImporter();

        public void ImportAllSites()
        {
            _importer.ImportData();
        }

        public void ImportSites(IEnumerable<SiteToCrawl> sitesToCraw)
        {
            _importer.ImportData(sitesToCraw);
        }

        public bool RemoveFromQueue(int siteId)
        {
            return _importer.RemoveFromQueue(siteId);
        }

        public void StopImporting()
        {
            _importer.StopImporting();
        }

        public void StopCurrentSiteImporting()
        {
            _importer.StopCurrentSiteImporting();
        }

        public int CrawledPages()
        {
            return _importer.CrawledPages;
        }

        public IEnumerable<SiteToCrawl> SitesQueue()
        {
            return _importer.SitesQueue;
        }

        public bool IsImportingInProgress()
        {
            return _importer.IsImportingInProgress;
        }
    }
}
