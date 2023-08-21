
var dataTable; 
// in this project we use data tables.

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/ApprovedGetAll"
        },
        "columns": [
            { "data": "id", "width": "15%" },
            { "data": "name", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderDate", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Admin/Order/OrderDetails?id=${data}">
                            
                        </a>
                         <a class="btn btn-success" href="/Admin/Order/ViewDetail/${data}")>View Detail
                             
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
        buttons: true,
        icon: "warning",
        text: "Delete Information !!!",
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