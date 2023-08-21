ar dataTable;
$(document).ready(function () {
    loadDataTable();
})

    
function loadDataTable() {
    var selectedStatus = document.getElementById("orderStatus").value;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/OrderDate"
        },
        "columns": [
            { "data": "id", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                            <a class="btn btn-info" href="/Admin/Order/OrderDate/${data}">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a class="btn btn-danger" onclick=Delete("/Admin/Order/OrderDate/${data}")>
                              <i class="fas fa-trash"></i>
                            </a>
                            </div>
                        `;
                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Want to delete data ?",
        icon: "warning",
        text: "Delete Information !!!!",
        buttons: true,
        dangerMode: true
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