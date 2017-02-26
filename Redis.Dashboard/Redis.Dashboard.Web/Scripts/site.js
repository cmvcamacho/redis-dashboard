var redisDataView = {};

$(document).ready(function () {
    rivets.configure({

        // Attribute prefix in templates
        prefix: 'rv',

        // Preload templates with initial data on bind
        preloadData: true,

        // Root sightglass interface for keypaths
        rootInterface: '.',

        // Template delimiters for text bindings
        templateDelimiters: ['{', '}'],

        // Alias for index in rv-each binder
        iterationAlias: function (modelName) {
            return '%' + modelName + '%';
        },

        // Augment the event handler of the on-* binder
        handler: function (target, event, binding) {
            this.call(target, event, binding.view.models)
        },

        // Since rivets 0.9 functions are not automatically executed in expressions. If you need backward compatibilty, set this parameter to true
        executeFunctions: false

    });
    redisDataView = rivets.bind($('#page-wrapper'), { redisInfo: {} })

    $('#search').keyup(function (e) {
        e.stopPropagation();
        e.preventDefault();
        e.returnValue = false;
        e.cancelBubble = true;

        clearTimeout($.data(this, 'timer'));
        if (e.keyCode == 13)
            search();
        else
            $(this).data('timer', setTimeout(search, 200));
    });
})


function LoadData(friendlyUrl) {
    $.get(friendlyUrl,
        function (data) {
            redisDataView.update({ redisInfo: data });
        })
        .fail(function () {
            redisDataView.update({ redisInfo: {} });
        })
        .always(function () {
            setTimeout(function () { LoadData(friendlyUrl) }, 1000);
        });
}

function search() {
    var existingString = $("#search").val();
    if (existingString.length == 0) {
        $("#server-info").show();
        return false; //wasn't enter, not > 0 char
    }
    friendlyUrl = $("#search").attr('friendlyUrl');
    $.get('/Search/' + friendlyUrl + '/' + existingString, function (data) {
        $("#server-info").hide();

        $('#search-results').html(data);
        $("#search-results").show();
    });

    return false; 
}
