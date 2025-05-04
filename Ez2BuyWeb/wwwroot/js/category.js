var dataTable;  //to be accessable in sweet alert function
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/category/getall',
        },
        columns: [
            { data: 'name', "width": "50%" },
            { data: 'displayOrder', "width": "20%" },
            //{ data: 'price', "width": "10%" },
            //{ data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/category/edit?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square me-2"></i>Edit</a>
                    <a onClick=Delete("/admin/category/delete?id=${data}") class="btn btn-danger mx-2"><i class="bi bi-trash me-2"></i>Delete</a>
                    </div>`
                },
                "width": "30%"
            },
        ]
    });
}

//sweet alert function for delete product
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);

                },
            });
        }
    });
}
