// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function doSearch(page) {
    var searchCondition = $("#frmSearchInput").serialize() + '&page=' + page;
    var action = $("#frmSearchInput").prop("action");
    var method = $("#frmSearchInput").prop("method");
    $.ajax({
        url: action,
        type: method,
        data: searchCondition,
        success: function (data) {
            $("#searchResult").html(data);
        }
    });
}
function doSearch_Mobile(page) {
    var searchCondition = $("#frmSearchInput_Mobile").serialize() + '&page=' + page;
    var action = $("#frmSearchInput_Mobile").prop("action");
    var method = $("#frmSearchInput_Mobile").prop("method");
    $.ajax({
        url: action,
        type: method,
        data: searchCondition,
        success: function (data) {
            $("#searchResult").html(data);
        }
    });
}
function showShoppingCart() {
    $.get('/Home/MiniCart', function (data) {
        $('#shoppingCart').html(data);
    });
    // Cập nhật số lượng
    $.get('/Home/GetCartCount', function (count) {
        $('#cartCount').text(count);
    });
}
$(document).ready(function () {
    if ($("#frmSearchInput").length && $("#searchResult").length) {
        doSearch(@Model.Page);
        $("#frmSearchInput").submit(function (e) {
            e.preventDefault();
            doSearch(1);
        });
    }
    if ($("#frmSearchInput_Mobile").length && $("#searchResult").length) {
        doSearch_Mobile(@Model.Page);
        $("#frmSearchInput_Mobile").submit(function (e) {
            e.preventDefault();
            doSearch_Mobile(1);
        });
    }


});
$(document).ready(function () {
    // Load MiniCart content when hovering over the cart icon
    $('#cart-wrapper').hover(function () {
        $.get('/Home/MiniCart', function (data) {
            $('#shoppingCart').html(data);
            $('#shoppingCart').show();
        });
    }, function () {
        // Hide the cart when mouse leaves
        $('#shoppingCart').hide();
    });
});
function formatNumber(input) {
    //Lấy giá trị số hiện tại và xóa tất cả dấu không phải số
    let value = input.value.replace(/\D/g, "");

    //Định dạng lại số với dấu chấm (.) ngăn cách hàng nghìn
    input.value = value.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function removeSeparators() {
    //Lấy các trường cần xử lý
    let minPriceInput = document.querySelector('input[name="@nameof(Model.MinPrice)"]');
    let maxPriceInput = document.querySelector('input[name="@nameof(Model.MaxPrice)"]');

    //Loại bỏ dấu chấm trước khi gửi
    minPriceInput.value = minPriceInput.value.replace(/\./g, "");
    maxPriceInput.value = maxPriceInput.value.replace(/\./g, "");
}