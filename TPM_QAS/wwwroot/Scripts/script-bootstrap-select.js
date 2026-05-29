$(document).ready(function () 
{
    // Enable Live Search.
    $('#UserList').attr('data-live-search', true);
    $('#ADUserList').attr('data-live-search', true);
    $('#ADUserList').attr('multiple', false);

    //// Enable multiple select.
    $('#UserList').attr('multiple', true);
    $('#UserList').attr('data-selected-text-format', 'count');

    //$('.uesrlstbox').selectpicker(
    //{
    //    width: '100%',
    //    title: '[Select User List]',
    //    //style: 'btn-warning',
    //    style: 'btn-select',
    //    size: 6,
    //    iconBase: 'fa',
    //    tickIcon: 'fa-check'
    //});
});  

