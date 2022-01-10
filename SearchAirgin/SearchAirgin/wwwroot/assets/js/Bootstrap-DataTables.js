var mSortingString = [];
var disableSortingColumn = 4;
mSortingString.push({ "bSortable": false, "aTargets": [disableSortingColumn] });

$(function() {
        var table = $('#tblFlights').dataTable({
            "paging": true,
            "ordering": true,
            "info": true,
            "aaSorting": [],
            "orderMulti": true,
            "aoColumnDefs": mSortingString

        });
});