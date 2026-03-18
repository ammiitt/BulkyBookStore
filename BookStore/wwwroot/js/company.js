var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: { url: '/admin/company/getall' },
        responsive: true,
        columns: [
            { data: "name" },
            { data: "streetAddress" },
            { data: "city" },
            { data: "state" },
            { data: "phoneNumber" },
            {
                data: 'id',
                className: "text-center",
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/company/upsert?id=${data}" 
                               class="btn btn-primary btn-sm mx-1">
                               <i class="bi bi-pencil-square me-1"></i>Edit
                            </a>
                            <a onclick="Delete('/admin/company/delete/${data}')" 
                               class="btn btn-danger btn-sm mx-1">
                               <i class="bi bi-trash-fill me-1"></i>Delete
                            </a>
                        </div>`;
                }
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            });
        }
    });
}