(function () {

    window.GraphView = function (query, exactMatch) {
        this._query = query;
        this._exactMatch = exactMatch;
    }

    var graphOptions = {
        width: '100%',
        height: 'calc(100vh - 190px)',
        autoResize: true,
        configure: {
            enabled: false,
            filter: 'physics',
            showButton: true
        },
        edges: {
            physics: true,
            smooth: false,
            color: {
                color: 'rgba(0, 0, 0, 0)',
                hover: 'rgba(0, 0, 0, 0)',
                highlight: 'rgba(147, 197, 75, 0.75)',
                inherit: false               
            },
            width: 0.3
        },
        nodes: {
            shape: 'box',
            borderWidth: 0.1
        },
        interaction: {
            dragNodes: false,
            hideEdgesOnDrag: true,
            hover: true,
            navigationButtons: true,
            selectable: false,
            selectConnectedEdges: true
        },
        physics: {
            barnesHut: {
                gravitationalConstant: -20000,
                springLength: 400,
                centralGravity: 0.1,
                springConstant: 0.0002,
                damping: 0.04,
                avoidOverlap: 0.2
            },
            stabilization: {
                enabled: true,
                iterations: 500,
                updateInterval: 1,
                onlyDynamicEdges: false,
                fit: true
            }
        }
    };

    GraphView.prototype = {
        _query: null,
        _exactMatch: null,

        _recipes: null,
        _network: null,
        _canvasContext: null,

        _graphData: null,
        _centralRecipeId: null,

        _currentSelectedNodeId: null,

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

                    var nodeColor = !!isMain ? 'rgba(147, 197, 75, 0.75)' : 'rgba(194, 194, 192, 0.75)';
                    nodes.push({
                        id: recipeToAdd.Id,
                        label: recipeToAdd.Name,
                        color: {
                            background: nodeColor,
                            border: nodeColor,
                            highlight: {
                                border: '#D2E5FF',
                                background: '#D2E5FF'
                            }
                        },
                        tooltip: recipeToAdd.Name
                    });
                }
            }
        },

        _initNetwork: function () {
            var self = this;
            var container = document.getElementById('graphContainer');

            var startTime = performance.now();
            this._network = new vis.Network(container, this._graphData, graphOptions);

            this._canvasContext = $(container).find('canvas').get(0).getContext('2d');

            this._network.on('stabilizationIterationsDone', function () {
                self._network.stopSimulation();
                self._focusOnNode(self._centralRecipeId, 600);

                self._toggleProgress(false);               

                console.log('stabilization time:', performance.now() - startTime);
            });

            this._network.on('click', function (object) {
                self._handleCanvasClick(object);
            });

            this._network.on('dragStart', function () {
                self._hideCustomElements();
            });

            this._network.on('zoom', function () {
                self._hideCustomElements();
            });

            this._network.on('hoverNode', function (object) {
                self._showNodeTooltip(object.node);
            });

            this._network.on('blurNode', function (object) {
                self._removeNodeTooltip(object.node);
            });
        },

        _handleCanvasClick: function (clickEventData) {
            var self = this;
            var node = self._network.getNodeAt(clickEventData.pointer.DOM);

            if (!node) {
                self._network.unselectAll();
                self._currentSelectedNodeId = null;
            } else {
                var boundingBox = node.shape.boundingBox;
                var tooltipHeight = boundingBox.bottom - boundingBox.top;
                var tooltipWidth = tooltipHeight;
                var clickPositon = clickEventData.pointer.canvas;
                var openExpanded = false;

                if (clickPositon.x <= boundingBox.right && clickPositon.x >= boundingBox.right - tooltipWidth
                    && clickPositon.y <= boundingBox.bottom && clickPositon.y >= boundingBox.top) {
                    openExpanded = true;                   
                }

                self._focusOnNode(node.id, 250, openExpanded);
            }           
        },

        _focusOnNode: function (nodeId, animationDuration, openExpanded) {
            var self = this;

            if (nodeId == self._currentSelectedNodeId) {

                if (!!openExpanded) {
                    self._expandNode(nodeId);
                }

                return;
            }

            this._deselectEdges();
            this._hideCustomElements();
            self._removeNodeTooltip(nodeId);

            animationDuration = animationDuration || 1000;
            this._network.focus(nodeId, {
                scale: 0.75,
                locked: false,
                animation: {
                    duration: animationDuration
                }
            });

            this._runAfterAnimation(function() {
                self._network.selectNodes([nodeId], true);
                self._currentSelectedNodeId = nodeId;

                if (!!openExpanded) {
                    self._network.once('afterDrawing', function () {
                        self._expandNode(nodeId);                       
                    });
                }
            });           
        },

        _deselectEdges: function () {
            this._network.selectEdges([]);
        },

        _showNodeTooltip: function (nodeId) {
            var self = this;

            var scale = this._network.getScale();

            if (scale < 0.5) {
                return;
            }

            this._network.on('afterDrawing', function() {
                var nodePosition = self._network.getBoundingBox(nodeId);

                self._canvasContext.fillStyle = "#FF0000";
                var height = nodePosition.bottom - nodePosition.top;
                var width = height;
                self._canvasContext.fillRect(nodePosition.right - width, nodePosition.top, width, height);
            });
        },

        _removeNodeTooltip: function() {
            this._network.off('afterDrawing');
        },

        _expandNode: function (nodeId) {
            var self = this;

            if (this._recipeModalOpenedId == nodeId) {
                this._removeExpandedModal();
                return;
            }

            this._removeExpandedModal();

            var nodePosition = self._network.getBoundingBox(nodeId);
            var domPostionTopLeft = self._network.canvasToDOM({ y: nodePosition.top, x: nodePosition.left });
            var domPostionBottomRight = self._network.canvasToDOM({ y: nodePosition.bottom, x: nodePosition.right });

            var element = $(
                '<div class="recipe-expanded-modal" data-id="' + nodeId + '">' +
                '   recipe content' +
                '</div>'
            );

            element.css({
                position: 'absolute',
                top: domPostionTopLeft.y + $('#graphContainer').offset().top - 200,
                left: domPostionTopLeft.x + $('#graphContainer').offset().left,
            });

            $('body').append(element);
            self._recipeModalOpenedId = nodeId;

            window.setTimeout(function () {
                $('body').on('click.modalOutsideClick', self._handleClickOutsideModal.bind(self));
            }, 0);
            
        },

        _removeExpandedModal: function () {
            $('body').off('click.modalOutsideClick');

            this._recipeModalOpenedId = null;
            $('.recipe-expanded-modal').remove();
        },

        _handleClickOutsideModal: function(event) {
            var target = $(event.target);
            if (target.parents('.recipe-expanded-modal').length > 0 || target.is('.recipe-expanded-modal')) {
                return;
            }
            this._removeExpandedModal();
        },

        _runAfterAnimation: function (callback) {
            var self = this;
            this._network.on('animationFinished', function () {
                self._network.off('animationFinished');
                callback();
            });
        },

        _hideCustomElements: function () {
            this._removeNodeTooltip();
            this._removeExpandedModal();
        },

        dispose: function () {
            this._network.destroy();
            this._network = null;
            $('body').off('click.modalOutsideClick');
        }
    };
})();