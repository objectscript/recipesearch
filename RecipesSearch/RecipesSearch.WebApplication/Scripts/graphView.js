(function () {

    window.GraphView = function (query, exactMatch) {
        this._query = query;
        this._exactMatch = exactMatch;
    }

    var graphOptions = {
        width: '100%',
        height: 'calc(100vh - 190px)',
        hover: true,
        navigation: true,
        configurePhysics: false,
        nodes: {
            shape: 'box',
            borderWidth: 1,
            radius: 3
            //color: {
            //    background: '#fff'
            //}
        },
        smoothCurves: {
            dynamic: false
        },
        edges: {
            color: {
                color: 'rgba(0,0,0,0)',
                highlight: 'rgba(147, 197, 75, 0.75)',
                hover: 'rgba(0,0,0,0)'
            },
            width: 0.3
        },
        physics: {
            barnesHut: {
                gravitationalConstant: -20000,
                springLength: 400,
                centralGravity: 0.1,
                springConstant: 0.0002,
                damping: 0.04
            }
        },
        stabilize: true,
        stabilizationIterations: 1000
    };

    GraphView.prototype = {
        _query: null,
        _exactMatch: null,

        _recipes: null,
        _network: null,

        _graphData: null,
        _centralRecipeId: null,

        showGraph: function () {
            var self = this;

            if (!!this.network) {
                this.dispose();
            }

            if (!this._query) {
                return;
            }

            this._toggleProgress(true);

            this._fetchData(function () {
                self._prepareGraphData();
                self._initNetwork();

                self._toggleProgress(false);
            });
        },

        _fetchData: function (callback) {
            var self = this;

            $.ajax({
                method: "GET",
                url: "/Home/GetGraphData?query=" + window.encodeURIComponent(this._query) + '&exactMatch=' + this._exactMatch,
            }).done(function (recipes) {
                self._recipes = recipes;
                callback();
            });
        },

        _toggleProgress: function (show) {
            if (show) {
                $('#graph').addClass('loading');
            } else {
                $('#graph').removeClass('loading');
            }
        },

        _prepareGraphData: function () {

            var recipeMap = {};
            var edgeCount = {};
            var nodes = [];
            var edges = [];

            for (var i = 0; i < this._recipes.length; ++i) {
                ensureRecipeAdded(this._recipes[i], true);
            }

            for (var i = 0; i < this._recipes.length; ++i) {
                var recipe = this._recipes[i];

                for (var j = 0; j < recipe.SimilarResults.length; ++j) {
                    var similarRecipe = recipe.SimilarResults[j];
                    ensureRecipeAdded(similarRecipe);

                    edgeCount[recipe.Id] = !edgeCount[recipe.Id] ? 1 : edgeCount[recipe.Id] + 1;
                    edgeCount[similarRecipe.Id] = !edgeCount[similarRecipe.Id] ? 1 : edgeCount[similarRecipe.Id] + 1;

                    edges.push({
                        from: recipe.Id,
                        to: similarRecipe.Id,
                        length: 100 + Math.sqrt(similarRecipe.SimilarRecipeWeight) * 100
                    });
                }
            }

            var maxEdgeCount = -1;
            var maxEdgeCountRecipeId = -1;

            for (var key in edgeCount) {
                if (edgeCount[key] > maxEdgeCount) {
                    maxEdgeCount = edgeCount[key];
                    maxEdgeCountRecipeId = key;
                }
            }

            this._centralRecipeId = maxEdgeCountRecipeId;
            this._graphData = {
                nodes: nodes,
                edges: edges
            };

            function ensureRecipeAdded(recipeToAdd, isMain) {

                if (!recipeMap[recipeToAdd.Id]) {
                    recipeMap[recipeToAdd.Id] = true;

                    nodes.push({
                        id: recipeToAdd.Id,
                        label: recipeToAdd.Name,
                        color: {
                            background: !!isMain ? 'rgba(147, 197, 75, 0.75)' : 'rgba(194, 194, 192, 0.75)'
                        },
                        tooltip: recipeToAdd.Name
                    });
                }
            }
        },

        _initNetwork: function () {
            var self = this;
            var container = document.getElementById('graphContainer');

            this._network = new vis.Network(container, null, graphOptions);

            this._network.on('stabilizationIterationsDone', function () {
                self._network.freezeSimulation(true);
                self._focusOnNode(self._centralRecipeId);
                self._network.selectNodes([self._centralRecipeId]);
            });

            this._network.on('select', function (selected) {
                if (!selected.nodes.length) {
                    self._deselectEdges();
                } else {
                    self._focusOnNode(selected.nodes[0]);
                }
            });

            this._network.on('stabilized', function (iterations) {
                console.log('stabilized', iterations);
            });

            this._network.on('viewChanged', function () {
                self._removeNodeTooltip();
            });

            this._network.on('hoverNode', function (object) {
                self._showNodeTooltip(object.node);
            });

            this._network.on('blurNode', function (object) {
                self._removeNodeTooltip(object.node);
            });

            self._network.setData(this._graphData, false);
        },

        _focusOnNode: function (nodeId) {
            this._network.focusOnNode(nodeId, {
                scale: 0.75,
                locked: false,
                animation: {
                    duration: 1000
                }
            });
        },

        _deselectEdges: function () {
            network.selectEdges([]);
        },

        _showNodeTooltip: function (nodeId) {
            var self = this;

            this._removeNodeTooltip(nodeId);

            var nodePosition = this._network.getBoundingBox(nodeId);
            var domPostionTopLeft = this._network.canvasToDOM({ y: nodePosition.top, x: nodePosition.left });
            var domPostionBottomRight = this._network.canvasToDOM({ y: nodePosition.bottom, x: nodePosition.right });
            var nodeHeight = domPostionBottomRight.y - domPostionTopLeft.y + 2;

            var element = $(
                '<div class="recipe-expand-tooltip" data-id="' + nodeId + '">' +
                '   <i class="glyphicon glyphicon-fullscreen"></i>' +
                '</div>'
            );
            element.find('i').css({ 'font-size': nodeHeight - 6 });
            element.css({
                position: 'absolute',
                top: domPostionTopLeft.y + $('#graphContainer').offset().top - 1,
                left: domPostionBottomRight.x + $('#graphContainer').offset().left - nodeHeight + 1,
                height: nodeHeight
            });

            element.on('click', function () {
                self._expandNode(nodeId);
            });

            $('body').append(element);
        },

        _removeNodeTooltip: function (nodeId) {
            if (!nodeId) {
                $('.recipe-expand-tooltip').remove();
            } else {
                $('.recipe-expand-tooltip[data-id=' + nodeId + ']').remove();
            }
        },

        _expandNode: function (nodeId) {
            $('.recipe-expanded-modal').remove();

            var nodePosition = this._network.getBoundingBox(nodeId);
            var domPostionTopLeft = this._network.canvasToDOM({ y: nodePosition.top, x: nodePosition.left });
            var domPostionBottomRight = this._network.canvasToDOM({ y: nodePosition.bottom, x: nodePosition.right });
            var element = $(
               '<div class="recipe-expanded-modal" data-id="' + nodeId + '">' +
               '   recipe content' +
               '</div>'
            );

            element.css({
                position: 'absolute',
                top: domPostionTopLeft.y + $('#graphContainer').offset().top - 100,
                left: domPostionTopLeft.x + $('#graphContainer').offset().left,
            });

            $('body').append(element);
        },

        dispose: function () {
            this._network.destroy();
            this._network = null;
        }
    };
})();