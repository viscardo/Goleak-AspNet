
$(document).ready(function () {
    var tour = new Tour(
    {
        backdrop: true,
        orphan: true,
        onEnd: function (tour) { window.location.href = 'http://www.goleak.com' },
    });


    tour.addSteps([

    {
        element: "#step1",
        title: "<b>First page is the leak feed.</b>",
        content: "Everybody shown on this list is a facebook friend."
    },
    {
        element: "#step2",
        title: "<b>True or False.</b>",
        content: "You can agree or disagree with someone said about your friend"
    },
            {
                element: "#search-form",
                title: "<b>Search for a friend.</b>",
                content: "Here the friend you searched for will appear. Click on his/her name or picture and you will be taken to his/her profile.",
                onShow: function (tour)
                {
                    $("#div-step-1").show();
                    $("#div-step-2").hide();
                    $("#search-form").val("spider"); $("#search-result").show();
                    $(".navbar-fixed-top").css('z-index', '1');
                }
            },
    {
        element: "#step-leak",
        title: "<b>We are now on the User Profile Page.</b>",
        content: "Here you can write anything you want about your friend. <b>Your identity will be preserved </b>.",
        placement: 'top',
        onShow: function (tour)
        {

            $("#div-step-1").hide();
            $("#div-step-2").show();
            $("#search-form").val(""); $("#search-result").hide();
            $("#feed").show();
        }
    },
    {
        element: "#feed-image",
        title: "<b>Last step.</b>",
        content: "The image shows if the person is a man or a woman.",
        placement: 'top',
        onShow: function (tour)
        {
            $("#txtLeak").val("");
            $("#div-last-step").show();
        }
    }


    ]);

    tour.restart();
});