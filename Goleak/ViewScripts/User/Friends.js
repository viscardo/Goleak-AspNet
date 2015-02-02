$(document).ready(function () {
    var templateItem = Handlebars.compile($("#templateItem").html());

    function friendFilter() {
        GOLEAK.bloquearTela();
        $.ajax({
            url: "/User/SelectedFriendFilter",
            data: {
                partName: $("#search-form").val()
            }
        }).done(function (data) {
            var html = templateItem({ Classes: data });
            $("#search-result").show();
            $('#search-result').html(html);
            GOLEAK.desbloquearTela();
        }).fail(function (response) {
            GOLEAK.desbloquearTela();
            alert("Ooops, something wrong!");
            //SPMA.notificacoes.erro(response.responseText);
        });
    }



    $('html').click(function () {
        $("#search-result").hide();
    });

    $("#search-form").keyup(function () {
        var texto = this.value;

        if (texto.length < 3) {
            $("#search-result").html("");
            return;
        }

        friendFilter();
    });
});