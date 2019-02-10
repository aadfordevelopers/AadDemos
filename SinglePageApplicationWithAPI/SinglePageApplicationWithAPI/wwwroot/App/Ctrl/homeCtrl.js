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
            authContext.acquireToken(authContext.config.resourceId, function (error, token) {

                // Handle ADAL Errors
                if (error || !token) {
                    printErrorMessage('ADAL Error Occurred: ' + error);
                    return;
                }

                jQuery.support.cors = true;

                // Get values
                $.ajax({
                    type: "GET",
                    url: "https://localhost.fiddler:44362/api/values",
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

