namespace RecipesSearch.WebApplication.ViewModels
{
    public class TasksViewModel
    {
        public bool NearestsResultsUpdatingInProgress { get; set; }

        public int NearestsResultsUpdatedCount { get; set; }

        public bool TfIdfUpdatingInProgress { get; set; }

        public bool TfUpdatingInProgress { get; set; }

        public bool IdfUpdatingInProgress { get; set; }

        public bool AllTasksBuildInProgress { get; set; }

        public int EmptyTfIdfCount { get; set; }

        public int EmptyTfCount { get; set; }

        public bool IdfGlobalExists { get; set; }

        public int EmptyNearestResultsCount { get; set; }

        public decimal TfProgress { get; set; }

        public decimal TfIdfProgress { get; set; }

        public bool TfBuildFailed { get; set; }

        public bool IdfBuildFailed { get; set; }

        public bool TfIdfBuildFailed { get; set; }

        public bool SimilarResultsBuildFailed { get; set; }

        public decimal SimilarResultsPercentage { get; set; }
    }
}
