function SetActiveMenu(menuItem, parentMenuItem) {
    $('.nav-link').removeClass('active');
    $('.nav-item').removeClass('menu-open');
    $('#' + menuItem).addClass('active');
    $('#' + parentMenuItem).addClass('menu-open');
}

function ConfirmDelete(element) {
    if (confirm("Deseja apagar este registo?")) {
        window.location.href = element.href;
    } else {
        return false;
    }
}

var dataTablesLanguage = {
    lengthMenu: 'Mostrar _MENU_ registos por página',
    zeroRecords: 'Sem resultados',
    emptyTable: 'Sem resultados',
    info: 'Registos de _START_ a _END_, num total de _TOTAL_',
    infoEmpty: 'Sem resultados',
    infoFiltered: '(filtrados de um total de _MAX_)',
    search: 'Procurar:',
    processing: 'A processar...',
    loadingRecords: 'A processar...',
    paginate: {
        first: 'Primeira',
        last: 'Última',
        next: 'Próxima',
        previous: 'Anterior'
    },
    aria: {
        sortAscending: ': Ordenar ascendente',
        sortDescending: ': Ordenar descendente'
    }
}