var dataTable;
$(document).ready(function () {
   loadDatatable(); 
});

function loadDatatable() {
   dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/user/getall"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },          
            { "data": "", "width": "15%" },
            { 
                "data": "id", 
                "render": function (data) {
                    return `
                          <div>
                                <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary">
                                    <i class="bi bi-pencil"></i>
                                </a>
                          </div>
                    `
                },
                "width": "15%" 
            }
        ]
    });
}