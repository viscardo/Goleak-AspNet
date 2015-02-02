

var GOLEAK = {
   
    util: {
        configurarCaixaDeBusca: function (id) {
            var input = $("#" + id);

            if (!input.val())
                input.val(input.attr("title"));

            input.click(function () {
                if (this.value == input.attr("title"))
                    this.value = "";
            });
        }
    },
    caixasDialogo: {
        confirmacao: jConfirm,
        alerta: jAlert
    },
    bloquearTela: $.blockUI,
    desbloquearTela: $.unblockUI
};

(function () {
    var sessaoExpiradaTimeout;

    function configurarRedirecionamentoSessaoExpirada() {
        sessaoExpiradaTimeout = setTimeout(function () { window.location = "/SessaoExpirada.htm"; }, sessionExpires);
    }

    function configurarReinicioRedirecionamentoSessaoExpirada() {
        $(document).ajaxStart(function () {
            clearTimeout(sessaoExpiradaTimeout);
        });

        $(document).ajaxStop(function () {
            configurarRedirecionamentoSessaoExpirada();
        });
    }



    function configurarAlerts() {
        $.alerts.cancelButton = "Cancelar";
    }



    function configurarBlockUI() {
        $.blockUI.defaults.css.border = null;
        $.blockUI.defaults.css.backgroundColor = null;
        $.blockUI.defaults.message = "<div><img src='/Images/ajax-loader.png' /></div>";
    }


    function obterUrlParameter(name) {
        return decodeURI(
                (RegExp(name + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1]
          );
    }

   

    $(document).ready(function () {

        configurarBlockUI();
        configurarAlerts();

        $("#btnUpdate").click(function () {
            GOLEAK.bloquearTela();
        });

        $("#signOut").click(function () {
            GOLEAK.bloquearTela();
            $.ajax({
                url: "/Home/LogOut",
                type: "POST",
                error: function () {
                    alert("Error to update");
                    GOLEAK.desbloquearTela();
                },
                success: function () {
                    window.location.reload();
                    GOLEAK.desbloquearTela();
                }
            });
        });

});
})();

