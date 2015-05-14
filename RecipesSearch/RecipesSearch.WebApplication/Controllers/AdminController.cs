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
using RecipesSearch.SearchEngine.SimilarResults;
using RecipesSearch.SearchEngine.SimilarResults.Builders;
using RecipesSearch.SitePagesImporter.Importer;
using RecipesSearch.WebApplication.Enums;
using RecipesSearch.WebApplication.ImporterService;
using RecipesSearch.WebApplication.ViewModels;

namespace RecipesSearch.WebApplication.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ConfigRepository _configRepository = new ConfigRepository();
        private readonly SiteToCrawlRepository _sitesToCrawlRepository = new SiteToCrawlRepository();
        private readonly CrawlingHistoryRepository _crawlingHistoryRepository = new CrawlingHistoryRepository();
        private readonly SearchSettingsRepository _searchSettingsRepository = new SearchSettingsRepository();
        private readonly SitePageManager _sitePageManager = new SitePageManager();

        private readonly ImporterServiceClient _importer = new ImporterServiceClient();

        public ActionResult Config()
        {
            var tfBuilder = TfBuilder.GetInstance();

            ViewBag.AdminPage = AdminPages.Config;
            var config = _configRepository.GetConfig();
            return View(ConfigViewModel.GetViewModel(config, tfBuilder.GetTfBuilders()));
        }

        public ActionResult Control()
        {
            var sitesInfo = _sitePageManager.GetSitesInfo();

            ViewBag.AdminPage = AdminPages.ControlPanel;
            return View(new CrawlerControlViewModel
            {
                IsCrawlingStarted = _importer.IsImportingInProgress(),
                CrawledPages = _importer.CrawledPages(),
                SitesInfo = sitesInfo ?? new List<SiteInfo>(),
                SitesQueue = _importer.SitesQueue().ToList()
            });
        }

        public ActionResult SearchSettings()
        {
            ViewBag.AdminPage = AdminPages.SearchSettings;
            var searchSettings = _searchSettingsRepository.GetSearchSettings();
            return View(SearchSettingsViewModel.GetViewModel(searchSettings));
        }

        public ActionResult CrawlingHistory()
        {
            ViewBag.AdminPage = AdminPages.CrawlingHistory;
            var crawlingHistory = _crawlingHistoryRepository.GetCrawlingHistory();
            return View(crawlingHistory);
        }

        public ActionResult SitesToCrawlSettings()
        {
            ViewBag.AdminPage = AdminPages.SitesToCrawlSettings;
            var config = _configRepository.GetConfig();
            var sitesToCralw = config.SitesToCrawl.Select(SiteToCrawlViewModel.GetViewModel);
            ViewBag.ConfigId = config.Id;
            return View(sitesToCralw);
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
                return Redirect("/Admin/SitesToCrawlSettings");
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
                    return Redirect("/Admin/SitesToCrawlSettings");
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
                _importer.ImportAllSites();
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
                return Redirect("/Admin/CrawlingHistory");
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
                _importer.ImportSites(new[] { site });
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
                return Redirect("/Admin/SitesToCrawlSettings");
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

        public ActionResult Tasks()
        {
            ViewBag.AdminPage = AdminPages.Tasks;

            var similarResultsBuilder = SimilarResultsBuilder.GetSimilarResultsBuilder();
            var tfIdfBuilder = TfIdfBuilder.GetInstance();
            var pageStatsRepository = new PageStatsRepository();

            return View(new TasksViewModel
            {
                NearestsResultsUpdatingInProgress = similarResultsBuilder.UpdateInProgress,
                NearestsResultsUpdatedCount = similarResultsBuilder.UpdatedPagesCount,
                TfIdfUpdatingInProgress = tfIdfBuilder.UpdateInProgress,
                EmptyNearestResultsCount = pageStatsRepository.GetNearestResultsStatistic(),
                EmptyTfIdfCount = pageStatsRepository.GetTfIdfStatistic(),
                TfUpdatingInProgress = TfBuilder.GetInstance().UpdateInProgress,
                IdfUpdatingInProgress = IdfBuilder.GetInstance().UpdateInProgress
            });
        }

        [HttpPost]
        public ActionResult StartNearestResultsUpdating()
        {
            var similarResultsBuilder = SimilarResultsBuilder.GetSimilarResultsBuilder();
            similarResultsBuilder.FindNearestResults();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StopNearestResultsUpdating()
        {
            var similarResultsBuilder = SimilarResultsBuilder.GetSimilarResultsBuilder();
            similarResultsBuilder.StopUpdating();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartTfIdfUpdating()
        {
            var tfIdfBuilder = TfIdfBuilder.GetInstance();
            tfIdfBuilder.Build();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartTfUpdating()
        {
            var tfBuilder = TfBuilder.GetInstance();
            tfBuilder.Build();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartIdfUpdating()
        {
            var idfBuilder = IdfBuilder.GetInstance();
            idfBuilder.Build();

            return RedirectToAction("Tasks");
        }

        public PartialViewResult TfIdfConfig()
        {
            var tfBuilder = TfBuilder.GetInstance();
            var idfBuilder = IdfBuilder.GetInstance();
            var tfIdfConfigRepository = new TfIdfConfigRepository();

            var tfIdfConfig = tfIdfConfigRepository.GetConfig();

            return PartialView("_TfIdfConfig",TfIdfConfigViewModel.GetViewModel(tfIdfConfig, tfBuilder.GetTfBuilders(), idfBuilder.GetIdfBuilders()));
        }

        [HttpPost]
        public ActionResult UpdateTfIdfConfig(TfIdfConfigViewModel tfIdfConfig)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var tfIdfConfigRepository = new TfIdfConfigRepository();
                    tfIdfConfigRepository.SaveConfig(TfIdfConfigViewModel.GetEntity(tfIdfConfig));
                    return Redirect("/Admin/Tasks");
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                Logger.LogError("UpdateTfIdfConfig error.", ex);
                return View("Error");
            }
        }
    }
}