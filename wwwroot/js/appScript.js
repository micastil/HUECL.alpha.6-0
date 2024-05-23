var dataIdToDelete;
var deliveryToDelete;
var paymentToDelete;
var rowToDelete;
var currentYear = (new Date).getFullYear();

$(function () {

    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

    var selectedYear = localStorage.getItem('selectedYear');

    if (selectedYear) {
        $("#yearFilter").val(selectedYear)
    }
    else {
        $("#yearFilter").val(currentYear)
    }

    $("#btnSaleDelivery").on('click', function () {

        if ($('#frmSaleDelivery').valid()) {
            var loadingIndicator = $('#loadingIndicator');
            loadingIndicator.show();

            var _number = $("#Number").val();
            var _deliveryDate = $("#DeliveryDate").val();
            var _comment = $("#Comment").val();
            var _saleId = $("#SaleId").val();

            var data =
            {
                Number: _number,
                DeliveryDate: _deliveryDate,
                Comment: _comment,
                SaleId: _saleId
            };

            var token = $('input[name="__RequestVerificationToken"]').val();
            data.__RequestVerificationToken = token;

            $.ajax
                ({
                    url: '/Sales/AddSaleDelivery',
                    type: 'POST',
                    data: data,
                    success: function (data) {
                        $('#deliveryContainer').html(data);
                        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                        $('#modalAddSaleDelivery').modal('hide');
                    },
                    error: function () {
                        console.log('Ha ocurrido un error en la aplicacion');
                    },
                    complete: function () {
                        loadingIndicator.hide();
                        $('#frmSaleDelivery').trigger('reset');
                    }
                });
        }
    });

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    $(document).on('click', '#addDeliveryItem', function () {
        var _saleDeliveryId = $(this).data('id');

        var data = {
            SaleDeliveryId: _saleDeliveryId
        };

        $.ajax({
            url: '/Sales/AddSaleDeliveryItems',
            type: 'GET',
            data: data,
            success: function (data) {
                $("#_ContainerSaleDeliveryItemsCreate").html(data);
                const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                $("#modalAddSaleDeliveryItem").modal('show');
            },
            error: function () {
                console.log('ha ocurrido un error en la aplicacion');
            }
        });
    });

    $(document).on('click', 'a[data-idtodelete]', function () {
        dataIdToDelete = $(this).attr('data-idtodelete');
        $("#confirmationModal").modal('show');
    });

    $(document).on('click', 'button[data-deliverydelete]', function () {
        deliveryToDelete = $(this).attr('data-deliverydelete');
        $("#confDeleteDelivery").modal('show');
    });

    $(document).on('click', '#btnAddSaleDeliveryItem', function () {
        if ($('#frmAddSaleDeliveryItem').valid()) {

            var loadingIndicator = $('#loadingIndicator');
            loadingIndicator.show();

            $.ajax
                ({
                    url: '/Sales/AddSaleDeliveryItems',
                    type: 'POST',
                    data: $("#frmAddSaleDeliveryItem").serialize(),
                    success: function (data) {

                        var status = data.status;
                        var partialView = data.partialView;

                        $("#modalAddSaleDeliveryItem").modal('hide');
                        $('#deliveryContainer').html(partialView);
                        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

                        if (status === 'CompleteDelivery') {
                            $("#addDelivery").prop('disabled', true);
                            $("#addDeliveryItem").prop('disabled', true);
                        }

                    },
                    error: function () {
                        console.log('ha ocurrido un error en la aplicacion');
                    },
                    complete: function () {
                        loadingIndicator.hide();
                        $('#frmAddSaleDeliveryItem').trigger('reset');
                    }
                });
        }

    });

    $(document).on('click', '#addSaleInvoice', function () {

        var _data = $(this).attr('data-id');

        $.ajax
            ({
                url: '/SaleInvoices/AddSaleInvoice',
                type: 'GET',
                data: { Id: _data },
                success: function (data) {

                    var status = data.status;
                    var partial = data.partialView;

                    if (status === 1) {
                        $("#_ContainerAddSaleInvoice").html(partial);
                        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                        $("#modalAddSaleInvoice").modal('show');
                    }
                },
                error: function (xhr, status, error) {

                    var _msg = '';

                    _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                        + xhr.status + ' : ' + xhr.statusText +
                        '</strong></div>';

                    $('#_ContainerErrorMessage').html(_msg);
                    $('#modalErrorMessage').modal('show');

                },
                complete: function () {

                }
            });
    });

    $(document).on('click', '#btnAddSaleInvoice', function () {
        if ($('#frmAddSaleInvoice').valid()) {

            $(this).html('<span class="spinner-border spinner-border-sm" aria-hidden="true"></span><span role="status"> Cargando ...</span>');

            $.ajax
                ({
                    url: '/SaleInvoices/AddSaleInvoice',
                    type: 'POST',
                    data: $("#frmAddSaleInvoice").serialize(),
                    success: function (data) {

                        var status = data.status;
                        var partialView = data.partialView;

                        $("#modalAddSaleInvoice").modal('hide');
                        $('#deliveryContainer').html(partialView);
                        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                    },
                    error: function (xhr, status, error) {

                        $("#modalAddSaleInvoice").modal('hide');

                        var _msg = '';

                        if (xhr.status === 500) {
                            _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                                + xhr.responseJSON.message +
                                '</strong></div>';
                        }
                        else {
                            _msg = '<div class="bd-callout bd-callout-danger"><strong>Error inesperado</strong></div>';
                        }

                        $('#_ContainerErrorMessage').html(_msg);
                        $('#modalErrorMessage').modal('show');

                        console.log('ha ocurrido un error en la aplicacion');
                    },
                    complete: function () {
                    }
                });
        }
    });

    $(document).on('click', 'a[data-invoiceId]', function (event) {
        event.preventDefault();
        var _data = $(this).attr('data-invoiceId');

        console.log(_data);

        var loading = '<div class="d-flex align - items - center"><strong role="status">Cargando información ...</strong><div class="spinner-border ms-auto text-success" aria-hidden="true"></div></div>';
        $('#_containerSaleInvoice').html(loading);
        $("#modalSaleInvoice").modal('show');

        $.ajax
            ({
                url: '/SaleInvoices/Detail',
                type: 'GET',
                data: { Id: _data },
                success: function (data) {
                    $('#_containerSaleInvoice').fadeOut(500, function () {
                        $(this).html(data);
                    });

                    $('#_containerSaleInvoice').fadeIn(500, function () {

                    });

                    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

                },
                error: function (xhr, status, error) {

                    $("#modalAddSaleInvoice").modal('hide');

                    var _msg = '';

                    if (xhr.status === 500) {
                        _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                            + xhr.responseJSON.message +
                            '</strong></div>';
                    }
                    else {
                        _msg = '<div class="bd-callout bd-callout-danger"><strong>Error inesperado</strong></div>';
                    }

                    $('#_ContainerErrorMessage').html(_msg);
                    $('#modalErrorMessage').modal('show');

                    console.log('ha ocurrido un error en la aplicacion');
                },
                complete: function () {
                }
            });
    });

    $('#modalSaleInvoice').on('hidden.bs.modal', function () {
        
    });

    $('#confDeleteDeliveryItem').on('click', function () {

        $.ajax({
            url: '/SaleDeliveries/DeleteSaleDeliveryItem',
            type: 'POST',
            data: { Id: dataIdToDelete },
            success: function (data) {

                var status = data.status;
                var partialView = data.partialView;

                $("#confirmationModal").modal('hide');
                $('#deliveryContainer').html(partialView);
                const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

                if (status === 'CompleteDelivery') {
                    $("#addDelivery").prop('disabled', true);
                }
                else {
                    $("#addDelivery").prop('disabled', false);
                }
            },
            error: function () {
                console.log('ha ocurrido un error en la aplicacion');
            }
        });
    });

    $('#btnDeleteDelivery').on('click', function () {
        $.ajax({
            url: '/SaleDeliveries/DeleteSaleDelivery',
            type: 'POST',
            data: { Id: deliveryToDelete },
            success: function (data) {



                var status = data.status;

                var buttonHTML = '<button class="btn btn-primary btn-sm mb-3" id="addDelivery" data-bs-toggle="modal" data-bs-target="#modalAddSaleDelivery">Agregar Despacho</button>';
                var containerHTML = '<div class="bd-callout bd-callout-warning"><strong>No existen Despachos para esta Venta en la base de datos</strong></div>';

                $('#' + deliveryToDelete).fadeOut(800, function () {
                    $(this).remove();

                    var remainingElements = $('#deliveryContainer').children().filter(function () {
                        return this.nodeType === 1;
                    });

                    if (remainingElements.length === 0) {
                        $('#deliveryContainer').html(containerHTML);
                        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                    }
                });

                $("#confDeleteDelivery").modal('hide');

                if (status === '1') {
                    if ($("#addDelivery").length === 0) {
                        $('#btnAddDeliveryContainer').html(buttonHTML);
                    }

                    $("#addDelivery").prop('disabled', false);
                }

            },
            error: function (xhr, status, error) {
                $("#confDeleteDelivery").modal('hide');

                var _msg = '';

                _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                    + xhr.status + ' : ' + xhr.statusText +
                    '</strong></div>';

                $('#_ContainerErrorMessage').html(_msg);
                $('#modalErrorMessage').modal('show');
            }
        });
    });

    $("#confirmationModal").on('hidden.bs.modal', function () {
        dataIdToDelete = undefined;
    });

    $("#confDeleteDelivery").on('hidden.bs.modal', function () {
        deliveryToDelete = undefined;
    });

    /* Invoice Payment helpers ++++++++++++++++++++++++++++++++++++++++++++++++*/

    $(document).on('click', 'button[data-paymentdelete]', function () {
        paymentToDelete = $(this).attr('data-paymentdelete');
        rowrowToDelete = $(this).closest('tr');
        $("#btnDeletePayment").html("Eliminar");

        $('#modalSaleInvoice').modal('hide');
        $('#confDeletePayment').modal('show');
    });

    $(document).on('click', '#addInvoicePayment', function () {
        var _data = $(this).attr('data-id');

        $.ajax
            ({
                url: '/SaleInvoices/AddSaleInvoicePayment',
                type: 'GET',
                data: { Id: _data },
                success: function (data) {
                    $("#_containerAddSaleInvoicePayment").html(data);
                    $("#modalSaleInvoice").modal('hide');
                    $("#modalAddSaleInvoicePayment").modal('show');
                },
                error: function (xhr, status, error) {
                    $("#modalAddSaleInvoicePayment").modal('hide');
                    $("#modalSaleInvoice").modal('hide');

                    var _msg = '';

                    if (xhr.status === 400) {
                        _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                            + xhr.responseJSON.msg +
                            '</strong></div>';
                    }
                    else {
                        _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                            + xhr.status + ' : ' + xhr.statusText +
                            '</strong></div>';
                    }

                    $('#_ContainerErrorMessage').html(_msg);
                    $('#modalErrorMessage').modal('show');
                },
                complete: function () {

                }
            });
    });

    $(document).on('click', '#btnAddSaleInvoicePayment', function () {

        if ($('#frmAddSaleInvoicePayment').valid()) {
            $(this).html('<span class="spinner-border spinner-border-sm" aria-hidden="true"></span><span role="status"> Cargando ...</span>');

            $.ajax
                ({
                    url: '/SaleInvoices/AddSaleInvoicePayment',
                    type: 'POST',
                    data: $("#frmAddSaleInvoicePayment").serialize(),
                    success: function (data) {
                        $('#_containerSaleInvoice').html(data);
                        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

                        $('#modalAddSaleInvoicePayment').modal('hide');
                        $('#modalSaleInvoice').modal('show');
                    },
                    error: function (xhr, status, error) {
                        $('#modalAddSaleInvoicePayment').modal('hide');
                        $('#modalSaleInvoice').modal('hide');

                        if (xhr.status === 400) {
                            _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                                + xhr.responseJSON.msg +
                                '</strong></div>';
                        }
                        else {
                            _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                                + xhr.status + ' : ' + xhr.statusText +
                                '</strong></div>';
                        }

                        $('#_ContainerErrorMessage').html(_msg);
                        $('#modalErrorMessage').modal('show');
                    }
                });
        }
    });

    $('#btnDeletePayment').on('click', function () {

        $(this).html('<span class="spinner-border spinner-border-sm" aria-hidden="true"></span><span role="status"> Cargando ...</span>');
        var containerHTML = '<div class="bd-callout bd-callout-warning"><strong>No existen Pagos para esta Factura</strong></div>';

        $.ajax({
            url: '/SaleInvoices/DeletePayment',
            type: 'POST',
            data: { Id: paymentToDelete },
            success: function (data) {
                $('#confDeletePayment').modal('hide');
                $('#modalSaleInvoice').modal('show');

                rowrowToDelete.remove();

                var remainingElements = $('#paymentsBody').children().filter(function () {
                    return this.nodeType === 1;
                });

                if (remainingElements.length === 0) {
                    $('#invoicePaymentsContainer').html(containerHTML);
                    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                }

                // var status = data.status;

                // var buttonHTML = '<button class="btn btn-primary btn-sm mb-3" id="addDelivery" data-bs-toggle="modal" data-bs-target="#modalAddSaleDelivery">Agregar Despacho</button>';
                // var containerHTML = '<div class="bd-callout bd-callout-warning"><strong>No existen Despachos para esta Venta en la base de datos</strong></div>';

                // $('#' + deliveryToDelete).fadeOut(800, function () {
                //     $(this).remove();

                //     var remainingElements = $('#deliveryContainer').children().filter(function () {
                //         return this.nodeType === 1;
                //     });

                //     if (remainingElements.length === 0) {
                //         $('#deliveryContainer').html(containerHTML);
                //         const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
                //         const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
                //     }
                // });

                // $("#confDeleteDelivery").modal('hide');

                // if (status === '1') {
                //     if ($("#addDelivery").length === 0) {
                //         $('#btnAddDeliveryContainer').html(buttonHTML);
                //     }

                //     $("#addDelivery").prop('disabled', false);
                // }


            },
            error: function (xhr, status, error) {
                console.log(xhr);
                $("#confDeletePayment").modal('hide');
                $('#modalSaleInvoice').modal('hide');
                var _msg = '';

                _msg = '<div class="bd-callout bd-callout-danger"><strong>'
                    + xhr.status + ' : ' + xhr.statusText +
                    '</strong></div>';

                $('#_ContainerErrorMessage').html(_msg);
                $('#modalErrorMessage').modal('show');
                loading.hide();
            }
        });
    });

    $('#btnCancelDeletePayment').on('click', function () {
        $('#confDeletePayment').modal('hide');
        $('#modalSaleInvoice').modal('show');
    });

    $("#confDeletePayment").on('hidden.bs.modal', function () {
        paymentToDelete = undefined;
    });

    $(document).on("keypress", "#TotalPayment", function (event) {
        var charCode = event.which || event.keyCode;
        if (charCode === 46) {
            event.preventDefault();
            return false;
        }
    });
    /***************************************************************************/

    function isNumber(value) {
        return !isNaN(parseFloat(value)) && isFinite(value);
    }

    function validateQuantity(input) {

        var max = parseInt(input.getAttribute('data-max'));

        var min = parseInt(input.getAttribute('data-min'));

        var value = parseInt(input.value);

        var validationMessage = '';

        if (value > max) {
            validationMessage = 'Cantidad no puede ser mayor a: ' + max;
        }
        if (value < min) {
            validationMessage = 'Cantidad no puede ser menor a: ' + min;
        }
        if (!isNumber(value)) {
            validationMessage = 'Debe ingresar un numero valido';
        }
        // Find the closest container div that wraps the input and related elements
        var containerDiv = input.closest('div');

        // Find the validation span within the container div
        var validationSpan = containerDiv.querySelector('span.text-danger');

        if (validationSpan) {
            validationSpan.textContent = validationMessage;

            // Ensure the validation span visibility matches the validation state
            validationSpan.style.display = validationMessage ? 'block' : 'none';
        }

        if (validationMessage) {
            // Optionally, you can disable the submit button or take other actions
            document.getElementById('btnAddSaleDeliveryItem').disabled = true;
        } else {
            // If there is no validation error, enable the submit button
            document.getElementById('btnAddSaleDeliveryItem').disabled = false;
        }
    }
});
