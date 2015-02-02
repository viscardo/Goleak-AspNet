$(document).ready(function () {

    $("#btnLeak").click(function () {
        GOLEAK.bloquearTela();
        $.ajax({
            url: "/Leak/Create",
            type: "POST",
            data:
                {
                    LeakText: $("#txtLeak").val(),
                    UserLeakedId: $("#txtLeak").attr('data-id')
                },
            error: function (erro) {
                alert("Error to submit");
                GOLEAK.desbloquearTela();
            },
            success: function (resposta) {
                if (resposta == null)
                    alert("Something went wrong.");
                else {
                    $(".media-list").prepend(resposta);
                    $("#txtLeak").val('');
                }
                GOLEAK.desbloquearTela();
            }
        });
    });

    $(".btnLike").click(function () {
        var spanLeaks = $(this).next();
        var qtdLeaks = $(spanLeaks).text();
        $(spanLeaks).html("<img src='/Images/mini-loader.gif' >");

        $.ajax({
            url: "/LeakOpinion/Like",
            type: "POST",
            data:
                {
                    LeakId: $(this).attr('data-id')
                },
            error: function (erro) {
                alert("Error to submit");
                $(spanLeaks).html(qtdLeaks);
            },
            success: function (resposta) {
                if (resposta.success == false) {
                    alert("You already gave your opinion.");
                    $(spanLeaks).html(qtdLeaks);
                }
                else {
                    var sum = parseInt(qtdLeaks) + 1;
                    $(spanLeaks).html(sum);
                }
            }
        });
    });

    $(".btnDislike").click(function () {
        var spanLeaks = $(this).next();
        var qtdLeaks = $(spanLeaks).text();
        $(spanLeaks).html("<img src='/Images/mini-loader.gif' >");

        $.ajax({
            url: "/LeakOpinion/Dislike",
            type: "POST",
            data:
                {
                    LeakId: $(this).attr('data-id')
                },
            error: function (erro) {
                alert("Error to submit");
                $(spanLeaks).html(qtdLeaks);
            },
            success: function (resposta) {
                if (resposta.success == false) {
                    alert("You already gave your opinion.");
                    $(spanLeaks).html(qtdLeaks);
                }
                else {
                    var sum = parseInt(qtdLeaks) + 1;
                    $(spanLeaks).html(sum);
                }
            }
        });
    });

});