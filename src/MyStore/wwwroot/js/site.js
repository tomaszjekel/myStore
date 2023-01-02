// Write your JavaScript code.
function addCart(id) {
   
    $.ajax({
        type: "GET",
        url: "../Cart/BuyAsync/",
        data: { id: id },
        methasync: true,
        success: function (html) {
            //$("#more").after(html);
            $(".price").html(html.price+" PLN");
            $("#qtyCart").text(html.qty);
            
        }
    });
}

$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "../Cart/Quantity/",
        //data: { id: id },
        //async: true,
        success: function (html) {
            //$("#more").after(html);
            $(".price").html(html.price + " PLN");
            $("#qtyCart").text(html.qty);

        }
    });
});