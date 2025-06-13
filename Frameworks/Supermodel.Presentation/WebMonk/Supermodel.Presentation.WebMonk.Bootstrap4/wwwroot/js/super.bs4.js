$(function () {
    $('[data-toggle="tooltip"]').tooltip();

    var selectedRow = document.getElementById("SelectedRow");
    if (selectedRow != null) selectedRow.scrollIntoView();

    $("[data-autocomplete-source]").each(function () 
    {
        $(this).autocomplete({ source: $(this).data("autocomplete-source") });
    });

    $("[data-open-new-for-edit]").click(function () {
        var editTrId = 0;
        $("#"+editTrId).removeClass("d-none");
        $("#"+editTrId+" :input").prop("disabled", false);
        $("#"+editTrId+" :input[data-checked]").removeProp("data-checked").prop("checked", "checked");
        $("[data-read-only-tr-buttons]").addClass("d-none");
        $("[data-open-new-for-edit]").prop("disabled", true);
    });

    $("[data-cancel-new-edit]").click(function () {
        var editTrId = $(this).closest('tr').prop('id');
        $("#"+editTrId).addClass("d-none");
        $("#"+editTrId+" :input").prop("disabled", true);
        $("#"+editTrId+" :input[data-checked]").removeProp("checked").prop("data-checked", "checked");
        $("[data-read-only-tr-buttons]").removeClass("d-none");
        $("[data-open-new-for-edit]").prop("disabled", false);
    });

    $("[data-open-for-edit]").click(function () {
        var readOnlyTrId = $(this).closest('tr').prop('id');
        var editTrId = -readOnlyTrId;
        $("#"+readOnlyTrId).addClass("d-none");
        $("#"+editTrId).removeClass("d-none");
        $("#"+editTrId+" :input").prop("disabled", false);
        $("#"+editTrId+" :input[data-checked]").removeProp("data-checked").prop("checked", "checked");
        $("[data-read-only-tr-buttons]").addClass("d-none");
        $("[data-open-new-for-edit]").prop("disabled", true);
    });

    $("[data-cancel-edit]").click(function () {
        var editTrId = $(this).closest('tr').prop('id');
        var readOnlyTrId = -editTrId;
        $("#"+readOnlyTrId).removeClass("d-none");
        $("#"+editTrId).addClass("d-none");
        $("#"+editTrId+" :input").prop("disabled", true);
        $("#"+editTrId+" :input[data-checked]").removeProp("checked").prop("data-checked", "checked");
        $("[data-read-only-tr-buttons]").removeClass("d-none");
        $("[data-open-new-for-edit]").prop("disabled", false);
    });

    (function () {
        var hasConfirmed = false;
        $("input[type=submit][data-sm-confirm-msg], input[type=button][data-sm-confirm-msg], a[data-sm-confirm-msg], button[data-sm-confirm-msg]").click(function () {
            var $this = $(this);
            if (!hasConfirmed) {
                bootbox.confirm($this.attr('data-sm-confirm-msg'), function (result) {
                    if (result) {
                        hasConfirmed = true;
                        $this.trigger("click");
                        hasConfirmed = false;
                    }
                });
                return false;
            }
            if ($this[0].tagName.toLowerCase() === 'a') window.location.href = $this.attr('href');
            return true;
        });
    })();
});

function supermodel_restfulLinkToUrlWithConfirmation(path, httpOverrideVerb, confirmationMsg) {
    if (confirmationMsg == null) {
        supermodel_restfulLinkToUrl(path, httpOverrideVerb);
    } else {
        bootbox.confirm(confirmationMsg, function (result) {
            if (result) supermodel_restfulLinkToUrl(path, httpOverrideVerb);
        });
    }
}

function supermodel_restfulLinkToUrl(path, httpOverrideVerb) {
    var form = document.createElement("form");
    form.setAttribute("method", "post");
    form.setAttribute("action", path);

    if (httpOverrideVerb.toLowerCase() !== 'post') {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", "X-HTTP-Method-Override");
        hiddenField.setAttribute("value", httpOverrideVerb);
        form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    form.submit();
}