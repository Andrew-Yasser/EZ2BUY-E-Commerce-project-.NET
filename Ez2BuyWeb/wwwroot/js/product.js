var dataTable;  //to be accessable in sweet alert function
$(Document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/product/getall',
        },
        columns: [
            { data: 'name',"width" : "25%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'price', "width": "10%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/product/edit?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square me-2"></i>Edit</a>
                    <a onClick=Delete("/admin/product/delete?id=${data}") class="btn btn-danger mx-2"><i class="bi bi-trash me-2"></i>Delete</a>
                    </div>`
                },
                "width": "25%"
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
