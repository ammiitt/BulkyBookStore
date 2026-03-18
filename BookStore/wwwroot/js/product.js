var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        dom: '<"d-flex justify-content-between mb-3"l f>rtip',
        ajax: { url: '/admin/product/getall' },
        responsive: true,
        autoWidth: false,
        columns: [

            { data: 'title', width: "25%" },

            { data: 'isbn', width: "15%" },

            {
                data: 'listPrice',
                width: "10%",
                className: "text-center fw-semibold",
                render: function (data) {
                    return `$${data}`;
                }
            },

            { data: 'author', width: "15%" },

            {
                data: 'category.name',
                width: "10%",
                render: function (data) {
                    return `<span class="badge bg-info">${data}</span>`;
                }
            },

            {
                data: 'id',
                width: "15%",
                className: "text-center",
                render: function (data) {
                    return `
                        <div class="btn-group">

                            <a href="/admin/product/upsert?id=${data}" 
                               class="btn btn-sm btn-outline-info">
                                <i class="bi bi-pencil-square"></i>
                            </a>

                            <button onClick="Delete('/admin/product/delete/${data}')" 
                                    class="btn btn-sm btn-outline-danger">
                                <i class="bi bi-trash"></i>
                            </button>

                        </div>
                    `;
                }
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This product will be permanently deleted.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#0dcaf0',
        cancelButtonColor: '#dc3545',
        confirmButtonText: 'Delete'
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