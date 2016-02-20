using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.PageStorage;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.Data.Views;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders;
using RecipesSearch.WebApplication.BuilderService;
using RecipesSearch.WebApplication.ImporterService;
using RecipesSearch.WebApplication.Enums;
using RecipesSearch.WebApplication.ViewModels;
using RecipesSearch.SearchEngine.Clusters.Base;

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
        private readonly BuilderServiceClient _builder = new BuilderServiceClient();

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
            return View(SearchSettingsViewModel.GetViewModel(searchSettings, _searchSettingsRepository.GetOnlineTfIdfBuilders()));
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
                LoggerWrapper.LogError("UpdateSearchSettings error.", ex);
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
                LoggerWrapper.LogError("UpdateCrawlerConfig error.", ex);
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
                LoggerWrapper.LogError("RemoveSiteToCrawl error.", ex);
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
                LoggerWrapper.LogError("AddSiteToCrawl error.", ex);
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
                LoggerWrapper.LogError("StartCrawling error.", ex);
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
                LoggerWrapper.LogError("StopCrawling error.", ex);
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
                LoggerWrapper.LogError("StopCrawling error.", ex);
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
                LoggerWrapper.LogError("ClearSitePages error.", ex);
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
                LoggerWrapper.LogError("ClearAllSites error.", ex);
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
                LoggerWrapper.LogError("ClearSitePages error.", ex);
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
                LoggerWrapper.LogError("RemoveSiteFromCrawlQueue error.", ex);
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
                LoggerWrapper.LogError("StopCurrentSiteImporting error.", ex);
                return View("Error");
            }
        }

        public ActionResult Tasks()
        {
            ViewBag.AdminPage = AdminPages.Tasks;

            var pageStatsRepository = new PageStatsRepository();
            var builderState = _builder.GetBuildersState();

            return View(new TasksViewModel
            {
                NearestsResultsUpdatingInProgress = builderState.SimilarResultsUpdateInProgress,
                NearestsResultsUpdatedCount = builderState.SimilarResultsUpdatedCount,
                TfIdfUpdatingInProgress = builderState.TfIdfBuildInProgress,
                EmptyNearestResultsCount = pageStatsRepository.GetNearestResultsStatistic(),
                EmptyTfCount = pageStatsRepository.GetTfStatistic(),
                IdfGlobalExists = pageStatsRepository.GetIdfStatistic() == 1,
                TfUpdatingInProgress = builderState.TfBuildInProgress,
                IdfUpdatingInProgress = builderState.IdfBuildInProgress,
                AllTasksBuildInProgress = builderState.AllTasksBuildInProgress,
                TfIdfProgress = builderState.TfIdfBuildProgress,
                TfProgress = builderState.TfBuildProgress,
                TfBuildFailed = builderState.TfBuildFailed,
                IdfBuildFailed = builderState.IdfBuildFailed,
                TfIdfBuildFailed = builderState.TfIdfBuildFailed,
                SimilarResultsBuildFailed = builderState.SimilarResultsBuildFailed,
                SimilarResultsPercentage = builderState.SimilarResultsPercentage,
                ClustersBuildFailed = builderState.ClustersBuildFailed,
                ClustersBuildInProgress = builderState.ClustersBuildInProgress,
                RecipesWithEmptyClusters = pageStatsRepository.GetCountOfRecipesWithEmptyClusters()
            });
        }

        [HttpPost]
        public ActionResult StartNearestResultsUpdating()
        {
            var tfIdfConfigRepository = new TfIdfConfigRepository();
            var tfIdfConfig = tfIdfConfigRepository.GetConfig();

            _builder.BuildSimilarResults(tfIdfConfig.SimilarResultsCount);

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StopNearestResultsUpdating()
        {
            _builder.StopSimilarResultsBuild();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartAllTasksUpdating()
        {
            _builder.BuildAllTasks();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StopAllTasksUpdating()
        {
            _builder.StopAllTasksUpdating();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartTfIdfUpdating()
        {
            _builder.BuildTfIdf();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StopTfIdfUpdating()
        {
            _builder.StopTfIdfBuild();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StTfIdfUpdating()
        {
            _builder.BuildTfIdf();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartTfUpdating()
        {
            _builder.BuildTf();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StopTfUpdating()
        {
            _builder.StopTfBuild();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartIdfUpdating()
        {
            _builder.BuildIdf();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StopClustersBuild()
        {
            _builder.StopClustersBuild();

            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public ActionResult StartClustersBuild()
        {
            var tfIdfConfigRepository = new TfIdfConfigRepository();

            var tfIdfConfig = tfIdfConfigRepository.GetConfig();
            _builder.BuildClusters((BuilderService.ClusterBuilders)tfIdfConfig.ClustersBuilder);

            return RedirectToAction("Tasks");
        }

        public PartialViewResult TfIdfConfig()
        {
            var tfBuilder = TfBuilder.GetInstance();
            var idfBuilder = IdfBuilder.GetInstance();
            var tfIdfConfigRepository = new TfIdfConfigRepository();

            var tfIdfConfig = tfIdfConfigRepository.GetConfig();

            return PartialView(
                "_TfIdfConfig", 
                TfIdfConfigViewModel.GetViewModel(
                    tfIdfConfig, 
                    tfBuilder.GetTfBuilders(), 
                    idfBuilder.GetIdfBuilders(),
                    ClustersBulderFactory.GetClustersBuilders().Select(x => x.ToString()).ToList()));
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
                LoggerWrapper.LogError("UpdateTfIdfConfig error.", ex);
                return View("Error");
            }
        }
    }
}