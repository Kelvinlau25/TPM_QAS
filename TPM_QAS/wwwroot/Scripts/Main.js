$(function ($) {
    $('#make-small-nav').click(function (e) {
        if ($('.RightBar').hasClass("HideLeftBar")) {
            $('.RightBar').removeClass("HideLeftBar");
            $('.LeftBar').removeClass("LeftNone");
        } else {
            $('.RightBar').addClass("HideLeftBar");
            $('.LeftBar').addClass("LeftNone");
        }
    });
});

function ifNull(obj, replace) {
    if (obj == null || obj == undefined || obj == 'null') {
        return replace;
    }
    return obj;
}
//popup notification on top right
//valid = boolean; true = success, false = failed
//msg = string; message return
//id = int; id of noification can be index of data
//isrequireheader = boolean; true = show header, false = hide header
//headerobj = html object must be like : <span>Here is header</span>
//callback = function; callback to function
function callnotification(valid,msg,id,isrequireheader,headerobj,callback) {
    $('#notifycls').show();
    if (isrequireheader == true && (headerobj != null || headerobj != "")) {
        $(".notifyheader").show();
        $(".notifyheader").append(headerobj);
    }

    var jtmpID = id;
    var responetype = "alert-success";
    var titleType = "Success!";
    if (valid == false) {
        responetype = "alert-danger";
        titleType = "Failed!";
    }
    var htmlx = "<li data-id='" + jtmpID + "'><div class='alert " + responetype + " alert-dismissible' data-slide='" + jtmpID + "' role='alert'>";
    htmlx += "<button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
    htmlx += "<strong>" + titleType + "</strong> <span>" + msg + "</span></div></div></li>";
    $("#listNotify").append(htmlx).ready(function () {
        $("#listNotify").find("[data-id='" + jtmpID + "']").delay(2500).fadeOut('fast');
    });

    if (typeof callback === 'function') {
        callback();
    }
}


//JH230712
function loadings(def) {
    def = def === undefined ? true : def;
    if (def) {
        $("div.spanner").addClass("show");
        $("div.overlay").addClass("show");
    } else {
        $("div.spanner").removeClass("show");
        $("div.overlay").removeClass("show");
    }
}

//INPUT VALIDATION
$(document).on('input', '.input-number', function (e) {
    const input = this;
    const cursorPosition = input.selectionStart;
    const numericValue = input.value.replace(/[^0-9]/g, '');
    const numericCursorPosition = cursorPosition - (input.value.length - numericValue.length);
    $(input).val(numericValue);
    const newPosition = numericCursorPosition >= 0 ? numericCursorPosition : 0;
    input.setSelectionRange(newPosition, newPosition);
});