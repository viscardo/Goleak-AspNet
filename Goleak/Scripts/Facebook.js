function InitialiseFacebook(appId) {

    window.fbAsyncInit = function () {
        FB.init({
            appId: appId, // App ID
            status: true, // check login status
            cookie: true, // enable cookies to allow the server to access the session
            xfbml: true  // parse XFBML
        });

        FB.Event.subscribe('auth.login', function (response) {
            var credentials = { uid: response.authResponse.userID, accessToken: response.authResponse.accessToken };
            PassCredenticals(credentials);
        });

        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                var credentials = { uid: response.authResponse.userID, accessToken: response.authResponse.accessToken };
                PassCredenticals(credentials);
                
            }
            else if (response.status === 'not_authorized') { alert("Você não autorizou pelo facebook"); }
            else { alert("Você não esta conectado pelo facebook"); }

        });


        function PassCredenticals(credentials) {
            $.ajax({
                url: "/home/FacebookCredentials",
                type: "POST",
                data: credentials,
                error: function () {
                    alert("Erro ao conectar a sua conta do facebook");
                },
                success: function (data) {
                    $("#FirstName").val(data.FirstName);
                    $("#LastName").val(data.LastName);
                    $("#Email").val(data.Email);
                    $("#Fb").val(data.Fb);
                    $("#Gender").val(data.Gender);
                }
            });
        }

        function SubmitLogin(credentials) {
            $.ajax({
                url: "/home/facebooklogin",
                type: "POST",
                data: credentials,
                error: function () {
                    alert("Erro ao conectar a sua conta do facebook");
                },
                success: function () {
                    window.location.reload();
                }
            });
        }

    };

    (function (d) {
        var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
        if (d.getElementById(id)) {
            return;
        }
        js = d.createElement('script');
        js.id = id;
        js.async = true;
        js.src = "//connect.facebook.net/en_US/all.js";
        ref.parentNode.insertBefore(js, ref);
    } (document));

}