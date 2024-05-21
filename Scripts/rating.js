$(".ratingstar").hover(function () {
    $(".ratingstar").addClass("fa-star-o").removeClass("fa-star");
    $(this).addClass("fa-star").removeClass("fa-star-o");
    $(this).prevAll(".ratingstar").addClass("fa-star").removeClass("fa-star-o");
});
$(".ratingstar").click(function () {
    debugger;
    var starvalue = $(this).attr("data-value");
    $("#ratingsvalue").val(starvalue);
});
