(function () {
    window.ResultsView = function (graphViewContainer, listViewContainer) {
        this._graphViewContainer = graphViewContainer;
        this._listViewContainer = listViewContainer;
    }

    window.ResultsView.prototype = {
        _graphViewContainer: null,
        _listViewContainer: null,

        _listView: null,
        _graphView: null,

        initialize: function() {
            this.initListView();
            this.initGraphView();
        },

        initListView: function() {
            this._listView = new window.ListView(this._listViewContainer);
            this._listView.initialize();
        },

        initGraphView: function () {
            var query = this._getQueryParameterByName('query');
            var exactMatch = this._getQueryParameterByName('exactMatch');

            this._graphView = new window.GraphView(this._graphViewContainer);
            this._graphView.showGraph(query, exactMatch);
        },

        _getQueryParameterByName: function(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };

})();