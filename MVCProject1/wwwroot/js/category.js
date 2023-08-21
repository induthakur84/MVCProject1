var dataTable;
$(document).ready(function () { //$ is the sign is used for jquery 
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "65%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                            <a class="btn btn-info" href="/Admin/Category/Upsert/${data}">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a class="btn btn-danger" onclick=Delete("/Admin/Category/Delete/${data}")>
                              <i class="fas fa-trash"></i>
                            </a>
                            </div>
                        `;
                }
            }
        ]
    })
}
//AJAX = Asynchronous JavaScript And XML.

//AJAX is not a programming language.

//AJAX just uses a combination of:

//A browser built -in XMLHttpRequest object(to request data from a web server)
//JavaScript and HTML DOM(to display or use the data)

function Delete(url) {
    swal({
        title: "Want to delete data ?",
        icon: "warning",
        text: "Delete Information !!!!",
        buttons: true,
        dangerModel: true
    }).then((willdelete) => {
        if (willdelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}