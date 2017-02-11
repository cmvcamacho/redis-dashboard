$(document).on("click", ".server-details", function (event) {
    if($('tr.collapsed').length > 0)
        $('tr.collapsed')
            .removeClass('collapsed')
            .addClass('expanded');
    else if ($('tr.expanded').length > 0)
        $('tr.expanded')
            .removeClass('expanded')
            .addClass('collapsed');
});
