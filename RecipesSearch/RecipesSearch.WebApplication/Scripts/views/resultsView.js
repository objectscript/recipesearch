(function () {
    window.ResultsView = function (graphViewContainer, listViewContainer, tabsHolder) {
        this._graphViewContainer = graphViewContainer;
        this._listViewContainer = listViewContainer;
        this._tabsHolder = tabsHolder;
    }

    window.ResultsView.prototype = {
        _graphViewContainer: null,
        _listViewContainer: null,
        _tabsHolder: null,

        _listView: null,
        _graphView: null,
        _itemViews: [],

        initialize: function() {
            this.initListView();
            this.initGraphView();
        },

        initListView: function() {
            this._listView = new window.ListView(this._listViewContainer);
            this._listView.pinRecipeCallback = this.pinRecipe.bind(this);
            this._listView.initialize();
        },

        initGraphView: function () {
            var query = this._getQueryParameterByName('query');
            var exactMatch = this._getQueryParameterByName('exactMatch');

            this._graphView = new window.GraphView(this._graphViewContainer);
            this._graphView.showGraph(query, exactMatch);
        },

        pinRecipe: function(recipeId) {
           for (var i = 0; i < this._itemViews.length; ++i) {
               if (this._itemViews[i].recipeId === recipeId) {
                   return;
               }
           }
            this._addItemView(recipeId);
        },

        unpinRecipe: function (recipeId) {
            for (var i = 0; i < this._itemViews.length; ++i) {
                if (this._itemViews[i].recipeId === recipeId) {
                    this._itemViews.splice(i, 1);
                }
            }
        },

        _addItemView: function(recipeId) {
            var newTab = $(
                '<li role="presentation" class="recipe-item-tab">' +
                '   <a href="#recipeTab_' + recipeId + '" role="tab" data-toggle="tab">' +
                '       <span class="glyphicon glyphicon-refresh spinning"></span>' +
                '   </a>' +
                '</li>');
            var newTabContent = $('<div role="tabpanel" class="tab-pane" id="recipeTab_' + recipeId + '">');

            var itemView = new window.ItemView(newTabContent, newTab, recipeId);
            this._itemViews.push(itemView);
            itemView.removeTabCallback = this.unpinRecipe.bind(this);
            itemView.initialize();

            this._tabsHolder.find('[role=tablist]').prepend(newTab);
            this._tabsHolder.find('.tab-content').append(newTabContent);
        },

        _getQueryParameterByName: function(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };

})();