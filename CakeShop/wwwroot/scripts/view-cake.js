const cakeControllerUri = "/api/cake/";
const reviewControllerUri = "/api/review/";
const transactionControllerUri = "/api/transaction/";

$(document).ready(function () {
    var cakeId = getUrlVars()["cakeid"];
    getCake(cakeId);
    getReviews();
    bindSubmitTransaction(cakeId);
});

    let today = new Date();
    var min = String(today.getMinutes()).padStart(2, '0');
    var hh = String(today.getHours()).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();
    today = hh + ':' + min + ' - ' + mm + '/' + dd + '/' + yyyy;


function displayReviews(reviews) {
    $("#reviews").empty();

    var cakeid = getUrlVars()["cakeid"];
    $.each(reviews, function (key, review) {
        if (review.cakeId == cakeid) {
            $("#reviews").append('<div class="card card-outline-secondary my-4"><div class="card-header">Product Reviews<div class="card-body" id="commentText"><p>' + review.comment + '</p><hr><small class="text-muted"  id="mailText">' + review.email.split(/@yahoo.com/) + " on   " + today + '<div class="card mt-4"><div class="card-body"><span class="text-warning" id="RatingValue"> ' + "Review Rate :  " + review.rating + " of 5 " + '</span></div></div></small><button type="button"  onclick="deleteReview(' + review.reviewId + ');" class="btn btn-danger">X</button></div></div></div>');
        }
    });

}

function getReviews() {
    $.ajax({
        type: "GET",
        url: reviewControllerUri,
        cache: false,
        success: function (reviews) {
            displayReviews(reviews);
        }
    });
}

$(".ratingStar").hover(function () {
    $(".ratingStar").addClass("far").removeClass("fas");
    $(this).addClass("fas").removeClass("far");
    $(this).prevAll(".ratingStar").addClass("fas").removeClass("far");
});

let star;
$(".ratingStar").click(function () {

    star = $(this).attr("data-value");
    return star;
});

function deleteReview(id) {
    $.ajax({
        url: reviewControllerUri + id,
        type: "DELETE",
        success: function (result) {
            alert("review was deleted successfully!");
            location.reload();
        }
    });
}

function bindSubmitTransaction(cakeId) {
    $(".btn-primary").on("click", function () {

        const transaction = {
            cakeId: cakeId,
            Quantity: $("#Quantity").val(),
            Buyer: $("#Buyer").val(),
        };

        $.ajax({
            type: "POST",
            accepts: "application/json",
            url: transactionControllerUri + "PostTransaction",
            contentType: "application/json",
            data: JSON.stringify(transaction),
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Something went wrong!");
            },
            success: function (result) {
                alert("The transaction was executed with succes.");
            }
        });

        return false;
    });
}

function addReview() {
    var cakeId = getUrlVars()["cakeid"];
    var formData = new FormData();

    formData.append("Rating", star);
    formData.append("Comment", $("#Comment").val());
    formData.append("Email", $("#Email").val());
    formData.append("CakeId", cakeId);

    $.ajax({
        type: "POST",
        url: reviewControllerUri + "PostReview",
        processData: false,
        contentType: false,
        data: formData,
        mimeType: "multipart/form-data",
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Something went wrong!");
        },
        success: function (result) {
            alert("review was added successfully!");
            location.reload();
        }
    });

    return false;
}


// Read a page's GET URL variables and return them as an associative array.
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function displayCakeDetails(cake) {
    $('#cake-name').html(cake.name);
    $('#cake-price').html(cake.price + ' lei');
    $('#cake-description').html(cake.description);
    $('#cake-category').html(cake.category);
    $('#cake-image').css('max-height', '400px');
    $('#cake-image').attr("src", cake.base64Image);
}

function getCake(cakeId) {
    $.ajax({
        type: "GET",
        url: cakeControllerUri + cakeId,
        cache: false,
        success: function (data) {
            displayCakeDetails(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Something went wrong!");
        }
    });
}