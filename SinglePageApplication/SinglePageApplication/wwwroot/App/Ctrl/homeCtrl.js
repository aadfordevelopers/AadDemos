(function () {

    // The HTML for this View
    var viewHTML;
    // Instantiate the ADAL AuthenticationContext
    var authContext = new AuthenticationContext(config);

    function registerDataClickHandlers() {

        // View Data Button
        $("#ViewData").click(function (event) {
            clearErrorMessage();

            // Acquire Token for Backend
            authContext.acquireToken(authContext.config.clientId, function (error, token) {

                // Handle ADAL Errors
                if (error || !token) {
                    printErrorMessage('ADAL Error Occurred: ' + error);
                    return;
                }

                // Get values
                $.ajax({
                    type: "GET",
                    url: "/api/Values",
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                }).done(function (data) {
                    $("#lblData").text("values returned from API are: " + data[0] + ", " + data[1]);
                    console.log('Get Call Sucessfull');
                }).fail(function () {
                    console.log('Fail to get values');
                    printErrorMessage('Error in Getting Values');
                });
            });
        });
    }

    function clearErrorMessage() {
        var $errorMessage = $(".app-error");
        $errorMessage.empty();
    };

    function printErrorMessage(mes) {
        var $errorMessage = $(".app-error");
        $errorMessage.html(mes);
    }

    // Module
    window.homeCtrl = {
        requireADLogin: true,
        preProcess: function (html) {

        },
        postProcess: function (html) {
            viewHTML = html;
            registerDataClickHandlers();
        }
    };
}());

