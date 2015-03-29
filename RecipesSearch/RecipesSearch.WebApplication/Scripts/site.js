(function () {
    $(document).ready(function () {
        removeBanners();
        initSearchTypeahed();
    });
})();

function initSearchTypeahed() {
    $('#searchInput').typeahead({
        highlight: true,
        minLenght: 1,
    }, {
        displayKey: 'value',
        source: dataSource
    });

    $('#searchInput').bind('typeahead:selected', postSearchRequest);

    function dataSource(query, callback) {
        $.ajax({
            method: "GET",
            url: "/Home/SuggestRecipe?query=" + window.encodeURIComponent(query),
        }).done(function (results) {
            var data = [];
            results.forEach(function (result) {
                data.push({
                    value: result.trim()
                });
            });
            callback(data);
        });
    }

    function postSearchRequest() {
        $('#searchForm').submit();
    }
}

// For SOMEE hosting
function removeBanners() {
    var centers = $(document).find('center');
    for (var i = 0; i < centers.length; ++i) {
        if (centers.eq(i).children().length == 1 && centers.eq(i).find('a').length == 1)
            centers.eq(i).remove();
    }
    var divs = $('html').find('div');
    for (var i = 0; i < divs.length; ++i) {
        var div = divs.eq(i);
        if (div.attr('style') == "height: 65px;")
            div.remove();
        if (div.attr('style') == "opacity: 0.9; z-index: 2147483647; position: fixed; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: #202020; margin: 0px; padding: 0px;")
            div.remove();
        if (div.attr('style') == "position: fixed; z-index: 2147483647; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: transparent; margin: 0px; padding: 0px;")
            div.remove();
    }
    var scripts = $('html').find('script');
    for (var i = 0; i < scripts.length; ++i) {
        if (scripts.eq(i).attr('src') == "http://ads.mgmt.somee.com/serveimages/ad2/WholeInsert4.js")
            scripts.eq(i).remove();
    }
}