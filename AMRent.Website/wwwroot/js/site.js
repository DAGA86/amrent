function SetActiveMenu(menuItem) {
    $('ul.main-nav li').removeClass('active');
    $(menuItem).addClass('active');
}