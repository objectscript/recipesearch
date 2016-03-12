(function () {

    window.GraphView = function (resultsView, container) {
        this._container = container;
        this._resultsView = resultsView;
    }

    var graphOptions = {
        width: '100%',
        height: 'calc(100vh - 172px)',
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
            width: 0.1
        },
        nodes: {
            shape: 'box',
            borderWidth: 0.1
        },
        interaction: {
            dragNodes: false,
            hideEdgesOnDrag: false,
            hover: true,
            navigationButtons: true,
            selectable: false,
            selectConnectedEdges: true
        },
        physics: {
            barnesHut: {
                gravitationalConstant: -20000,
                springLength: 1,
                centralGravity: 0.1,
                springConstant: 0.0002,
                damping: 0.04,
                avoidOverlap: 0.7
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
        _container: null,
        _resultsView: null,

        _recipes: null,
        _network: null,
        _canvasContext: null,

        _graphData: null,
        _centralRecipeId: null,

        _currentSelectedNodeId: null,
        _idToRecipeMap: {},

        _showAllEdges: false,

        _useClusters: false,
        _separateClusters: true,
        _clusters: null,
        _selectedClusterId: null,

        showGraph: function (query, exactMatch, separateClusters) {
            var self = this;

            if (!!this.network) {
                this.dispose();
            }

            if (!query) {
                return;
            }

            this._toggleProgress(true);

            self._setProgressText('Fetching data...');
            this._fetchData(query, exactMatch, function () {

                if (self._useClusters && self._separateClusters) {
                    graphOptions.physics.barnesHut.centralGravity = 0;
                    graphOptions.physics.barnesHut.springConstant = 0.5;
                    graphOptions.physics.barnesHut.avoidOverlap = 0.9;
                    graphOptions.physics.stabilization.iterations = 700;
                }

                self._prepareGraphData();

                self._setProgressText('Preparing graph...');
                self._initNetwork();

                if (self._useClusters) {
                    self._createClustersInfo();
                    self._showClustersControl();
                }
            });
        },

        hasRecipe: function (recipeId) {
            return !!this._idToRecipeMap[+recipeId];
        },

        focusOnRecipe: function (recipeId) {
            return this._focusOnNode(recipeId, 250, false);
        },

        _fetchData: function (query, exactMatch, callback) {
            var self = this;
            exactMatch = exactMatch || 'false';

            $.ajax({
                method: "GET",
                url: "/Home/GetGraphData?query=" + window.encodeURIComponent(query) + '&exactMatch=' + exactMatch,
            }).done(function (response) {
                self._recipes = response.Recipes;
                self._useClusters = response.UseClusters;
                self._separateClusters = response.SeparateClusters;
                callback();
            });
        },

        _toggleProgress: function (show) {
            if (show) {
                this._container.addClass('loading');
            } else {
                this._container.removeClass('loading');
            }
        },

        _setProgressText: function (text) {
            this._container.find('.loading-holder .note').text(text);
        },

        _prepareGraphData: function () {
            var self = this;

            var edgeCount = {};
            var nodes = [];
            var edges = new vis.DataSet();
            var clusterLeads = {};

            // Create nodes for primary recipes
            for (var i = 0; i < this._recipes.length; ++i) {
                var node = ensureRecipeAdded(this._recipes[i], true);

                // If we are using clustering we may need to designate a recipe as a clusters leader
                if (this._useClusters) {

                    // Do not add recipe more than 1
                    var nodeFound = false;
                    for (var key in clusterLeads) {
                        if (clusterLeads[key] === node) {
                            nodeFound = true;
                            break;
                        }
                    }

                    if (nodeFound) {
                        continue;
                    }

                    for (var j = 0; j < this._recipes[i].ClusterIds.length; ++j) {
                        if (!clusterLeads[this._recipes[i].ClusterIds[j]]) {
                            clusterLeads[this._recipes[i].ClusterIds[j]] = node;
                            break;
                        }
                    }
                }
            }

            // Add edges between primary and similar results
            for (var i = 0; i < this._recipes.length; ++i) {
                var recipe = this._recipes[i];

                for (var j = 0; j < recipe.SimilarResults.length; ++j) {
                    var similarRecipe = recipe.SimilarResults[j];
                    ensureRecipeAdded(similarRecipe);

                    edgeCount[recipe.Id] = !edgeCount[recipe.Id] ? 1 : edgeCount[recipe.Id] + 1;
                    edgeCount[similarRecipe.Id] = !edgeCount[similarRecipe.Id] ? 1 : edgeCount[similarRecipe.Id] + 1;

                    edges.add({
                        from: recipe.Id,
                        to: similarRecipe.Id,
                        length: 100 + Math.sqrt(similarRecipe.SimilarRecipeWeight) * 100
                    });
                }
            }

            // If we are using clustering we need to add edeges between primary results if they are belong to the same cluster
            if (this._useClusters) {
                for (var i = 0; i < this._recipes.length; ++i) {
                    for (var j = i + 1; j < this._recipes.length; ++j) {
                        if (hasCommonCluster(this._recipes[i], this._recipes[j])) {
                            edges.add({
                                from: this._recipes[i].Id,
                                to: this._recipes[j].Id,
                                length: 100 + 1000
                            });
                        }
                    }
                }
            }

            // Find recipe with max edges count to focus by default
            var maxEdgeCount = -1;
            var maxEdgeCountRecipeId = -1;

            for (var key in edgeCount) {
                if (edgeCount[key] > maxEdgeCount) {
                    maxEdgeCount = edgeCount[key];
                    maxEdgeCountRecipeId = key;
                }
            }

            for (var i = 0; i < nodes.length; ++i) {
                var nodeId = nodes[i].id;
                var edgesCount = edgeCount[nodeId] || 0;
                if (maxEdgeCountRecipeId == nodeId) {
                    nodes[i].level = 1;
                } else {
                    nodes[i].level = 1 + Math.ceil((maxEdgeCount - edgesCount) / 5);
                }
            }

            // Place clusters lead on fixed positions
            if (this._useClusters && this._separateClusters) {
                var clustersLeadsArray = [];

                for (var key in clusterLeads) {
                    clustersLeadsArray.push(clusterLeads[key]);
                }
                this._shuffleClusterLeads(clustersLeadsArray);
                var d = 360 / clustersLeadsArray.length * Math.PI / 180;

                var x = 0, y = clustersLeadsArray.length * 100;
                for (var i = 0; i < clustersLeadsArray.length; ++i) {
                    var node = clustersLeadsArray[i];
                    node.x = x;
                    node.y = y;

                    node.fixed = true;

                    var x1 = x * Math.cos(d) - y * Math.sin(d);
                    var y1 = x * Math.sin(d) + y * Math.cos(d);
                    x = x1;
                    y = y1;
                }
            }

            this._centralRecipeId = maxEdgeCountRecipeId;
            this._graphData = {
                nodes: nodes,
                edges: edges
            };

            function ensureRecipeAdded(recipeToAdd, isMain) {

                if (!self._idToRecipeMap[recipeToAdd.Id]) {
                    self._idToRecipeMap[recipeToAdd.Id] = recipeToAdd;

                    var nodeColor = !!isMain ? 'rgba(147, 197, 75, 0.75)' : 'rgba(194, 194, 192, 0.75)';
                    var node = {
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
                    };
                    nodes.push(node);

                    return node;
                } else {
                    return nodes.filter(function (node) {
                        return node.id === recipeToAdd.id;
                    })[0];
                }
            }

            function hasCommonCluster(firstRecipe, secondRecipe) {
                for (var i = 0; i < firstRecipe.ClusterIds.length; ++i) {
                    for (var j = 0; j < secondRecipe.ClusterIds.length; ++j) {
                        if (firstRecipe.ClusterIds[i] === secondRecipe.ClusterIds[j]) {
                            return true;
                        }
                    }
                }

                return false;
            }
        },

        _shuffleClusterLeads: function (leads) {
            for (var i = leads.length - 1; i >= 1; --i) {
                var j = Math.floor(Math.random() * i);
                var t = leads[i];
                leads[i] = leads[j];
                leads[j] = t;
            }
        },

        _createClustersInfo: function () {
            var clustersMap = {};
            var distinctClusterIds = [];

            for (var i = 0; i < this._recipes.length; ++i) {
                var clusters = this._recipes[i].ClusterIds;
                for (var j = 0; j < clusters.length; ++j) {
                    var clusterId = clusters[j];

                    // Exclude clusters with only one result
                    if (!clustersMap[clusterId]) {

                        var recipesToCheck = this._recipes[i].SimilarResults.concat(this._recipes);

                        for (var k = 0; k < recipesToCheck.length; ++k) {
                            var similarRecipe = recipesToCheck[k];

                            if (similarRecipe.Id === this._recipes[i].Id) {
                                continue;
                            }

                            var hasCluster = similarRecipe.ClusterIds.some(function (similarClusterId) {
                                return +similarClusterId === +clusterId;
                            });

                            if (hasCluster) {
                                distinctClusterIds.push(clusterId);
                                clustersMap[clusterId] = true;
                                break;
                            }
                        }
                    }
                }
            }

            var colors = this._distinctColors(distinctClusterIds.length);

            this._clusters = {};
            for (var i = 0; i < distinctClusterIds.length; ++i) {
                var clusterId = distinctClusterIds[i];
                this._clusters[clusterId] = {
                    color: 'rgb(' + colors[i][0] + ',' + colors[i][1] + ',' + colors[i][2] + ')'
                }
            }
        },

        _showClustersControl: function () {
            var self = this;

            var controlHtml = '<div class="clusters-control"><span class="clusters-control-title">Clusters: </span>';
            for (var key in this._clusters) {
                var clusterId = +key;
                var color = this._clusters[key].color;
                controlHtml += '<span data-cluster-id="' + clusterId + '" class="clusters-control-item" style="background-color:' + color + '" title="' + clusterId + '"></span>';
            }

            controlHtml += '<span data-cluster-id="-1" class="clusters-control-item all" title="All"></span>';

            controlHtml += '</div>';

            var control = $(controlHtml);

            control.css({
                position: 'absolute',
                top: 10,
                right: 10,
            });

            control.find('.clusters-control-item').on('click', function () {
                var clusterId = $(this).data('clusterId');

                control.find('.clusters-control-item').removeClass('selected');

                if (self._selectedClusterId === +clusterId) {
                    self._hideClusters();
                } else {
                    self._showCluster(+clusterId);
                    $(this).addClass('selected');
                }
            });

            $('#graphContainer').append(control);
        },

        _updateClustersControlState: function () {
            var control = $('#graphContainer').find('.clusters-control');

            control.find('.clusters-control-item').removeClass('selected');
            if (!!this._selectedClusterId) {
                control.find('.clusters-control-item[data-cluster-id=' + this._selectedClusterId + ']').addClass('selected');
            }
        },

        _initNetwork: function () {

            var container = this._container.find('#graphContainer').get(0);

            this.__newtorkStartTime = performance.now();
            this._network = new vis.Network(container, this._graphData, graphOptions);

            this._canvasContext = $(container).find('canvas').get(0).getContext('2d');

            this._addNetworkEvents();
        },

        _addNetworkEvents: function () {
            var self = this;

            this._network.on('stabilizationIterationsDone', function () {
                self._network.stopSimulation();

                if (self._centralRecipeId !== -1) {
                    self._focusOnNode(self._centralRecipeId, 600);
                }

                self._toggleProgress(false);
                self._createShowAllEdgesButton();
                self._resultsView.onGraphViewInitialized();

                console.log('stabilization time:', performance.now() - self.__newtorkStartTime);
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
            var nodeId = self._network.getNodeAt(clickEventData.pointer.DOM);

            if (!nodeId) {
                self._network.unselectAll();
                self._currentSelectedNodeId = null;
            } else {
                var boundingBox = self._network.getBoundingBox(nodeId);
                var tooltipHeight = boundingBox.bottom - boundingBox.top;
                var tooltipWidth = tooltipHeight;
                var clickPositon = clickEventData.pointer.canvas;
                var openExpanded = false;

                if (clickPositon.x <= boundingBox.right && clickPositon.x >= boundingBox.right - tooltipWidth
                    && clickPositon.y <= boundingBox.bottom && clickPositon.y >= boundingBox.top) {
                    openExpanded = true;
                }

                self._focusOnNode(nodeId, 250, openExpanded);
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

            animationDuration = animationDuration || 1000;
            this._network.focus(nodeId, {
                scale: 0.75,
                locked: false,
                animation: {
                    duration: animationDuration
                }
            });

            this._runAfterAnimation(function () {
                self._removeNodeTooltip();
                self._network.selectNodes([nodeId], true);
                self._currentSelectedNodeId = nodeId;
                self._network.redraw();

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

            this._network.on('afterDrawing', function () {
                var nodePosition = self._network.getBoundingBox(nodeId);
                var height = nodePosition.bottom - nodePosition.top;
                var width = height;

                self._canvasContext.fillStyle = "rgba(249, 249, 249, 0.85)";
                self._canvasContext.fillRect(nodePosition.right - width, nodePosition.top, width, height);

                self._canvasContext.font = (width - 4) + 'px \'Glyphicons Halflings\'';
                self._canvasContext.fillStyle = 'black';
                self._canvasContext.fillText(String.fromCharCode(0xe096), nodePosition.right - width + width / 2, nodePosition.top + width / 2);
            });
        },

        _removeNodeTooltip: function () {
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

            var recipe = this._idToRecipeMap[nodeId];

            var elementHtml = '<div class="recipe-expanded-modal" data-id="' + nodeId + '">';

            var clustersHeaderHtml = '';
            if (this._useClusters) {
                clustersHeaderHtml = '<span> Clusters: </span>';
                for (var i = 0; i < recipe.ClusterIds.length; ++i) {
                    var clusterId = recipe.ClusterIds[i];
                    if (!this._clusters[clusterId]) {
                        continue;
                    }
                    clustersHeaderHtml += '<span data-cluster-id="' + clusterId + '" class="clusters-control-item" style="background-color:' + this._clusters[clusterId].color + '" + title="' + clusterId + '"></span>'
                }
            }

            elementHtml +=
                '<div class="header">' +
                    recipe.Name +
                    clustersHeaderHtml +
                    '<i class="glyphicon glyphicon-remove close-icon" title="Close modal"></i>' +
                    '<i class="glyphicon glyphicon-pushpin pin-icon" title="Pin recipe"></i>' +
                '</div>';

            elementHtml += '<div class="content">';

            if (!!recipe.ImageUrl) {
                elementHtml += '<img class="image" src="' + recipe.ImageUrl + '"></img>';
            }
            if (!!recipe.Description) {
                elementHtml += '<div class="recipe-item">' + recipe.Description + '</div>';
            }

            elementHtml += '<b>Ингредиенты:</b><br />';
            elementHtml += '<div class="recipe-item">' + recipe.Ingredients + '</div>';

            elementHtml += '<b>Инструкция по приготовлению:</b><br />';
            elementHtml += '<div class="recipe-item">' + recipe.RecipeInstructions + '</div>';

            if (!!recipe.AdditionalData) {
                elementHtml += '<b>Дополнительная информация:</b><br />';
                elementHtml += '<div class="recipe-item">' + recipe.AdditionalData + '</div>';
            }

            elementHtml += '<a class="recipe-url" target="_blank" href="' + recipe.URL + '">' + recipe.URL + '</a>';

            elementHtml += '</div></div>';

            var element = $(elementHtml);

            element.css({
                position: 'absolute',
                top: domPostionTopLeft.y + $('#graphContainer').offset().top - 200 - 3,
                left: domPostionTopLeft.x + $('#graphContainer').offset().left,
            });

            $('body').append(element);
            self._recipeModalOpenedId = nodeId;

            element.on('mousemove', function () {
                self._removeNodeTooltip();
                self._network.redraw();
            });

            element.find('.close-icon').on('click', function () {
                self._removeExpandedModal();
            });

            element.find('.pin-icon').on('click', function () {
                self._resultsView.pinRecipe(nodeId);
                self._removeExpandedModal(nodeId);
                self._expandNode(nodeId);
            });

            element.find('.clusters-control-item').on('click', function () {
                var clusterId = $(this).data('clusterId');

                element.find('.clusters-control-item').removeClass('selected');

                if (self._selectedClusterId === +clusterId) {
                    self._hideClusters();
                } else {
                    self._showCluster(+clusterId);
                    $(this).addClass('selected');
                }

                self._updateClustersControlState();
            });

            window.setTimeout(function () {
                $('body').on('click.modalOutsideClick', self._handleClickOutsideModal.bind(self));
            }, 0);

        },

        _removeExpandedModal: function () {
            $('body').off('click.modalOutsideClick');

            this._recipeModalOpenedId = null;
            $('.recipe-expanded-modal').remove();
        },

        _handleClickOutsideModal: function (event) {
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

        _createShowAllEdgesButton: function () {
            var allEdgesButton = $('<div class="vis-button showAllEdges" style="touch-action: none; -webkit-user-select: none; -webkit-user-drag: none; -webkit-tap-highlight-color: rgba(0, 0, 0, 0);"></div>');
            this._container.find('.vis-navigation').append(allEdgesButton);

            allEdgesButton.on('click', this._toggleShowAllEdges.bind(this));
        },

        _toggleShowAllEdges: function () {
            if (!this._showAllEdges) {
                this._network.setOptions({
                    edges: {
                        color: {
                            inherit: true
                        }
                    }
                });
                this._network.stopSimulation();
            } else {
                this._network.setOptions({
                    edges: {
                        color: graphOptions.edges.color
                    }
                });
                this._network.stopSimulation();
            }

            this._showAllEdges = !this._showAllEdges;
        },

        _showCluster: function (clusterId) {
            this._selectedClusterId = clusterId;
            var color;

            if (clusterId !== -1) {
                color = this._clusters[clusterId].color;
            }

            var edges = this._graphData.edges.get();
            for (var i = 0; i < edges.length; ++i) {
                var edge = edges[i];
                var fromRecipe = this._idToRecipeMap[edge.from];
                var toRecipe = this._idToRecipeMap[edge.to];

                if (clusterId === -1) {
                    edge.color = this._clusters[fromRecipe.ClusterIds[0]].color;
                    this._graphData.edges.update(edge);
                    continue;
                }

                if (hasCluster(fromRecipe) && hasCluster(toRecipe)) {
                    edge.color = color;
                } else {
                    edge.color = null;
                }
                this._graphData.edges.update(edge);
            }

            this._network.stopSimulation();
            this._deselectEdges();

            function hasCluster(recipe) {
                for (var i = 0; i < recipe.ClusterIds.length; ++i) {
                    if (recipe.ClusterIds[i] === +clusterId) {
                        return true;
                    }
                }

                return false;
            }
        },

        _hideClusters: function () {
            this._selectedClusterId = null;

            var edges = this._graphData.edges.get();
            for (var i = 0; i < edges.length; ++i) {
                var edge = edges[i];
                edge.color = null;
                this._graphData.edges.update(edge);
            }

            this._network.stopSimulation();
            this._deselectEdges();
        },

        _distinctColors: function (count) {
            var colors = [];
            for (hue = 0; hue < 360; hue += 360 / count) {
                colors.push(this._hsvToRgb(hue, 100, 100));
            }
            return colors;
        },

        _hsvToRgb: function (h, s, v) {
            var r, g, b;
            var i;
            var f, p, q, t;

            // Make sure our arguments stay in-range
            h = Math.max(0, Math.min(360, h));
            s = Math.max(0, Math.min(100, s));
            v = Math.max(0, Math.min(100, v));

            // We accept saturation and value arguments from 0 to 100 because that's
            // how Photoshop represents those values. Internally, however, the
            // saturation and value are calculated from a range of 0 to 1. We make
            // That conversion here.
            s /= 100;
            v /= 100;

            if (s == 0) {
                // Achromatic (grey)
                r = g = b = v;
                return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
            }

            h /= 60; // sector 0 to 5
            i = Math.floor(h);
            f = h - i; // factorial part of h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));

            switch (i) {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;

                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;

                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;

                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;

                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;

                default: // case 5:
                    r = v;
                    g = p;
                    b = q;
            }

            return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
        },

        dispose: function () {
            this._network.destroy();
            this._network = null;
            $('body').off('click.modalOutsideClick');
        }
    };
})();