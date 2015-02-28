using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.PageStorage;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.Data.Views;
using RecipesSearch.SitePagesImporter.Importer;
using RecipesSearch.WebApplication.Enums;
using RecipesSearch.WebApplication.ViewModels;

namespace RecipesSearch.WebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly ConfigRepository _configRepository = new ConfigRepository();
        private readonly SiteToCrawlRepository _sitesToCrawlRepository = new SiteToCrawlRepository();
        private readonly CrawlingHistoryRepository _crawlingHistoryRepository = new CrawlingHistoryRepository();
        private readonly SearchSettingsRepository _searchSettingsRepository = new SearchSettingsRepository();
        private readonly SitePageManager _sitePageManager = new SitePageManager();
        private readonly Importer _importer = Importer.GetImporter();

        public ActionResult Config()
        {
            ViewBag.AdminPage = AdminPages.Config;
            var config = _configRepository.GetConfig();
            return View(ConfigViewModel.GetViewModel(config));
        }

        public ActionResult Control()
        {
            var crawlingHistory = _crawlingHistoryRepository.GetCrawlingHistory();
            var sitesInfo = _sitePageManager.GetSitesInfo();

            ViewBag.AdminPage = AdminPages.ControlPanel;
            return View(new CrawlerControlViewModel
            {
                IsCrawlingStarted = _importer.IsImportingInProgress,
                CrawledPages = _importer.CrawledPages,
                CrawlingHistory = crawlingHistory,
                SitesInfo = sitesInfo ?? new List<SiteInfo>(),
                SitesQueue = _importer.SitesQueue
            });
        }

        public ActionResult SearchSettings()
        {
            ViewBag.AdminPage = AdminPages.SearchSettings;
            var searchSettings = _searchSettingsRepository.GetSearchSettings();
            return View(SearchSettingsViewModel.GetViewModel(searchSettings));
        }

        [HttpPost]
        public ActionResult UpdateSearchSettings(SearchSettingsViewModel searchSettings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _searchSettingsRepository.SaveSearchSettings(SearchSettingsViewModel.GetEntity(searchSettings));
                    return Redirect("/Admin/SearchSettings");
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                Logger.LogError("UpdateSearchSettings error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UpdateCrawlerConfig(ConfigViewModel config)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _configRepository.SaveConfig(ConfigViewModel.GetEntity(config));
                    return Redirect("/Admin/Config");
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                Logger.LogError("UpdateCrawlerConfig error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RemoveSiteToCrawl(int siteToCrawlId)
        {
            try
            {
                _sitesToCrawlRepository.RemoveSiteToCrawl(siteToCrawlId);
                return Redirect("/Admin/Config");
            }
            catch (Exception ex)
            {
                Logger.LogError("RemoveSiteToCrawl error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AddSiteToCrawl(SiteToCrawlViewModel siteToCrawl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _sitesToCrawlRepository.SaveSiteToCrawl(SiteToCrawlViewModel.GetEntity(siteToCrawl));
                    return Redirect("/Admin/Config");
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                Logger.LogError("AddSiteToCrawl error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult StartCrawling()
        {
            try
            {
                _importer.ImportData();
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("StartCrawling error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult StopCrawling()
        {
            try
            {
                _importer.StopImporting();
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("StopCrawling error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ClearHistory()
        {
            try
            {
                _crawlingHistoryRepository.Clear();
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("StopCrawling error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ClearSitePages(int siteId)
        {
            try
            {
                _sitePageManager.DeletePages(siteId);
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("ClearSitePages error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ClearAllSites()
        {
            try
            {
                _sitePageManager.DeletePages();
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("ClearAllSites error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult CrawlSite(int siteId)
        {
            try
            {
                var site = _sitesToCrawlRepository.GetSiteToCrawl(siteId);
                _importer.ImportData(new[] { site });
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("ClearSitePages error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RemoveSiteFromCrawlQueue(int siteId)
        {
            try
            {
                _importer.RemoveFromQueue(siteId);
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("RemoveSiteFromCrawlQueue error.", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult StopCurrentSiteImporting()
        {
            try
            {
                _importer.StopCurrentSiteImporting();
                return Redirect("/Admin/Control");
            }
            catch (Exception ex)
            {
                Logger.LogError("StopCurrentSiteImporting error.", ex);
                return View("Error");
            }
        }
    }
}