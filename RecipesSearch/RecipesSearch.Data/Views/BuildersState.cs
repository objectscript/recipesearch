namespace RecipesSearch.Data.Views
{
    public struct BuildersState
    {
        public bool TfBuildInProgress { get; set; }

        public decimal TfBuildProgress { get; set; }

        public bool TfIdfBuildInProgress { get; set; }

        public decimal TfIdfBuildProgress { get; set; }

        public bool IdfBuildInProgress { get; set; }

        public bool SimilarResultsUpdateInProgress { get; set; }

        public int SimilarResultsUpdatedCount { get; set; }

        public decimal SimilarResultsPercentage { get; set; }

        public bool AllTasksBuildInProgress { get; set; }

        public bool TfBuildFailed { get; set; }

        public bool IdfBuildFailed { get; set; }

        public bool TfIdfBuildFailed { get; set; }

        public bool SimilarResultsBuildFailed { get; set; }

        public bool ClustersBuildInProgress { get; set; }

        public bool ClustersBuildFailed { get; set; }
    }
}
