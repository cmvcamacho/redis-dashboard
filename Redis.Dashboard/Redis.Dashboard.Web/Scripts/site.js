$(document).on("click", ".server-details", function (event) {
    if ($('tr.collapsed').length > 0) {
        ShowDetails(true);
    }
    else if ($('tr.expanded').length > 0){
        ShowDetails(false);
    }
});

function ShowDetails(show) {
    if (show === undefined)
        return;
    if (show) {
        $('tr.collapsed')
            .removeClass('collapsed')
            .addClass('expanded');
        $(".server-details").html('<i class="fa fa-arrows-alt"></i> more details');
    }
    else{
        $('tr.expanded')
            .removeClass('expanded')
            .addClass('collapsed');
        $(".server-details").html('<i class="fa fa-arrows-alt"></i> less details');
    }
}
function LoadData(friendlyUrl) {
    $('#page-wrapper')
        .load(friendlyUrl,
        function (response, status, xhr) {
            ShowDetails(undefined);
        });
}