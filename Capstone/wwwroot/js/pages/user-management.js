$(document).ready(function () {
    var table = $('#usersTable').DataTable({
        "ajax": {
            "url": "/User/GetUsers",
            "type": "GET",
            "dataSrc": "data",
            "error": function (xhr, error, thrown) {
                console.error("Error fetching users:", thrown);
                alert("An error occurred while fetching user data.");
            }
        },
        "columns": [
            {
                "data": null,
                "render": function (data, type, row, meta) {
                    return meta.row + 1;
                },
                "className": "text-center",
                "responsivePriority": 1
            },
            { "data": "firstName" },
            { "data": "lastName" },
            { "data": "email" },
            {
                "data": "isLocked",
                "render": function(data, type, row) {
                    if (data) {
                        return '<span class="badge bg-danger">Locked</span>';
                    } else {
                        return '<span class="badge bg-success">Active</span>';
                    }
                },
                "className": "text-center",
                "responsivePriority": 2
            },
            {
                "data": "role",
                "render": function(data, type, row) {
                    if (data === "Admin") {
                        return '<span class="badge bg-success">Admin</span>';
                    } else {
                        return '<span class="badge bg-warning">User</span>';
                    }
                },
                "className": "text-center",
                "responsivePriority": 2
            },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "render": function(data, type, row) {
                    return `
                        <div class="dropdown">
                            <button class="btn btn-sm btn-secondary dropdown-toggle" type="button" id="actionMenuButton" data-bs-toggle="dropdown" aria-expanded="false">
                                Actions
                            </button>
                            <ul class="dropdown-menu" aria-labelledby="actionMenuButton">
                                <li>
                                    <button class="dropdown-item lock-user" data-id="${row.id}">
                                        ${row.isLocked ? 'Unlock User' : 'Lock User'}
                                    </button>
                                </li>
                                <li>
                                    <button class="dropdown-item toggle-role" data-id="${row.id}">
                                        ${row.role === 'Admin' ? 'Make User' : 'Make Admin'}
                                    </button>
                                </li>
                                <li>
                                    <button class="dropdown-item delete-user" data-id="${row.id}">
                                        Delete User
                                    </button>
                                </li>
                            </ul>
                        </div>
                    `;
                },
                "className": "text-center",
                "responsivePriority": 3
            }
        ],
        "responsive": {
            "details": false
        },
        "pagingType": "simple_numbers",
        "lengthMenu": [10, 25, 50, 100],
        "autoWidth": false,
        "order": [[1, "asc"]],
        "columnDefs": [
            {
                "targets": -1,
                "orderable": false,
                "searchable": false
            }
        ],
        "language": {
            "emptyTable": "No users available",
            "loadingRecords": "Loading...",
            "processing": "Processing...",
            "search": "Search:",
            "paginate": {
                "first": "First",
                "last": "Last",
                "next": "Next",
                "previous": "Previous"
            },
        }
    });

    var confirmationModal = new bootstrap.Modal(document.getElementById('confirmationModel'));
    
    $(document).on('click', '.lock-user', function() {
        var userId = $(this).data('id');
        $('#deleteUserId').val(userId);
        $('#actionType').val('lock');
        $('#confirmation-question').text('Are you sure you want to lock this user?');
        $('#confirmation-title').text('Lock User');
        $('#confirmationForm button[type="submit"]').removeClass('btn-danger btn-success btn-warning').addClass('btn-warning').text('Lock');
        confirmationModal.show();
    });
    
    $(document).on('click', '.unlock-user', function() {
        var userId = $(this).data('id');
        $('#deleteUserId').val(userId);
        $('#actionType').val('unlock');
        $('#confirmation-question').text('Are you sure you want to unlock this user?');
        $('#confirmation-title').text('Unlock User');
        $('#confirmationForm button[type="submit"]').removeClass('btn-danger btn-success btn-warning').addClass('btn-success').text('Unlock');
        confirmationModal.show();
    });
    
    $(document).on('click', '.delete-user', function() {
        var userId = $(this).data('id');
        $('#deleteUserId').val(userId);
        $('#actionType').val('delete');
        $('#confirmation-question').text('Are you sure you want to delete this user?');
        $('#confirmation-title').text('Delete User');
        $('#confirmationForm button[type="submit"]').removeClass('btn-danger btn-success btn-warning').addClass('btn-danger').text('Delete');
        confirmationModal.show();
    });


    $(document).on('click', '.toggle-role', function() {
        var userId = $(this).data('id');
        $('#deleteUserId').val(userId);
        $('#actionType').val('role-toggle');
        $('#confirmation-question').text('Are you sure you want to change role of this user?');
        $('#confirmation-title').text('Role change');
        $('#confirmationForm button[type="submit"]').removeClass('btn-danger btn-success btn-warning').addClass('btn-danger').text('Change');
        confirmationModal.show();
    });
    
    $('#confirmationForm').submit(function(e) {
        e.preventDefault();
        var userId = $('#deleteUserId').val();
        var actionType = $('#actionType').val();
        var ajaxOptions = {};

        if(actionType === 'lock') {
            ajaxOptions = {
                url: '/User/LockUser',
                type: 'POST',
                data: { id: userId }
            };
        }
        else if(actionType === 'unlock') {
            ajaxOptions = {
                url: '/User/UnlockUser',
                type: 'POST',
                data: { id: userId }
            };
        }
        else if(actionType === 'delete') {
            ajaxOptions = {
                url: '/User/DeleteUser',
                type: 'DELETE',
                data: { id: userId }
            };
        }
        else if(actionType === 'role-toggle') {
            ajaxOptions = {
                url: '/User/ToggleRole',
                type: 'POST',
                data: { id: userId }
            }
        }

        $.ajax(ajaxOptions)
            .done(function(response) {
                if(response.success === undefined) {
                    if(response){
                        confirmationModal.hide();
                        table.ajax.reload(null, false);
                    } else {
                        alert('Action failed.');
                    }
                }
                else {
                    if(response.success){
                        confirmationModal.hide();
                        table.ajax.reload(null, false);
                    } else {
                        alert(response.message || 'Action failed.');
                    }
                }
            })
            .fail(function() {
                alert('An error occurred while performing the action.');
            });
    });
});